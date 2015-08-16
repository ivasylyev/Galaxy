﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Domain.Deployments;
using Nejdb.Bson;
using NLog;
using NuGet;

namespace Codestellation.Galaxy.Infrastructure
{
    public class PackageVersionBoard : IDisposable
    {
        private readonly FeedBoard _feedBoard;
        private readonly DeploymentBoard _deploymentBoard;

        private struct FeedPackageTuple : IEquatable<FeedPackageTuple>
        {
            public readonly ObjectId FeedId;
            public readonly string PackageId;
            public readonly string FeedUri;

            public FeedPackageTuple(NugetFeed feed, string packageId)
            {
                FeedId = feed.Id;
                FeedUri = feed.Uri;
                PackageId = packageId;
            }

            public bool Equals(FeedPackageTuple other)
            {
                return FeedId.Equals(other.FeedId) && string.Equals(PackageId, other.PackageId);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                return obj is FeedPackageTuple && Equals((FeedPackageTuple)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (FeedId.GetHashCode() * 397) ^ PackageId.GetHashCode();
                }
            }
        }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ConcurrentDictionary<FeedPackageTuple, SemanticVersion[]> _cache;
        private readonly Timer _refreshTimer;
        private bool _timerStarted;

        public event Action Refreshed;

        public PackageVersionBoard(FeedBoard feedBoard, DeploymentBoard deploymentBoard)
        {
            _feedBoard = feedBoard;
            _deploymentBoard = deploymentBoard;
            _cache = new ConcurrentDictionary<FeedPackageTuple, SemanticVersion[]>();
            _refreshTimer = new Timer(OnTimerRefresh, null, Timeout.Infinite, Timeout.Infinite);

            //avoid NRE
            Refreshed = delegate { };
        }

        public IEnumerable<Version> GetPackageVersions(ObjectId feedId, string packageId)
        {
            if (string.IsNullOrWhiteSpace(packageId))
            {
                return new Version[0];
            }

            //this method is thread safe because it use concurrent dictionary.
            var feed = _feedBoard.GetFeed(feedId);
            var tuple = new FeedPackageTuple(feed, packageId);

            SemanticVersion[] versions;
            return _cache.TryGetValue(tuple, out versions)
                ? versions.Select(x => x.Version)
                : new Version[0];
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start()
        {
            if (_timerStarted)
            {
                return;
            }
            ForceRefresh();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ForceRefresh()
        {
            _timerStarted = true;

            var refreshPeriod = TimeSpan.FromMinutes(5);
            var startImmediately = TimeSpan.FromSeconds(0);

            _refreshTimer.Change(startImmediately, refreshPeriod);
        }

        private void OnTimerRefresh(object state)
        {
            Func<FeedPackageTuple[]> getTuplesFunc = GetTuples;
            var getTuplesTask = Task.Factory.StartNew(getTuplesFunc, CancellationToken.None, TaskCreationOptions.None, SingleThreadScheduler.Instance);
            getTuplesTask.ContinueWith(prev =>
            {
                if (prev.IsFaulted)
                {
                    return;
                }

                var tuples = prev.Result;
                RefreshCache(tuples);
            });
        }

        private void RefreshCache(FeedPackageTuple[] tuples)
        {
            //This is long running process, mostly waits on blocking interprocess calls. That's why parallelism level increased a prior.
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount << 1 };

            Parallel.ForEach(tuples, parallelOptions, RefreshCacheForPackage);

            Logger.Info("Nuget packages versions cache successfully refreshed.");

            Refreshed();
        }

        private void RefreshCacheForPackage(FeedPackageTuple source)
        {
            try
            {
                var repo = PackageRepositoryFactory.Default.CreateRepository(source.FeedUri);

                SemanticVersion[] versions = repo
                    .FindPackagesById(source.PackageId)
                    .Select(x => x.Version)
                    .ToArray();

                _cache.AddOrUpdate(source, versions, (key, old) => versions);
            }
            catch (Exception ex)
            {
                //TODO: Notify error somehow
                Logger.Error(ex, "Package version cache update error");
            }
        }

        private FeedPackageTuple[] GetTuples()
        {
            //this method should run via single thread scheduler because it uses non-thread safe structures of dashboard.
            var results = _deploymentBoard
                .Deployments
                .Where(x => !string.IsNullOrWhiteSpace(_feedBoard.GetFeed(x.FeedId).Uri) && !string.IsNullOrWhiteSpace(x.PackageId))
                .Select(x => new FeedPackageTuple(_feedBoard.GetFeed(x.FeedId), x.PackageId))
                .Distinct()
                .ToArray();

            return results;
        }

        public void Dispose()
        {
            _refreshTimer.Dispose();
        }
    }
}