﻿using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Quarks.IO;
using Codestellation.Quarks.Resources;
using Codestellation.Quarks.Streams;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;

namespace Codestellation.Galaxy.Tests.InfrastructureTests
{
    [TestFixture]
    public class PackageVersionCacheTests
    {
        private const string TestPackageId = "TestNugetPackage";

        private string _nugetFeedFolder;

        [SetUp]
        public void Init()
        {
            _nugetFeedFolder = Path.Combine(Environment.CurrentDirectory, "testnuget");

            Folder.EnsureDeleted(_nugetFeedFolder);
            Folder.EnsureExists(_nugetFeedFolder);

            var testPackage10 = Folder.Combine(_nugetFeedFolder, "TestNugetPackage.1.0.0.nupkg");
            EmbeddedResource
                .EndsWith("TestNugetPackage.1.0.0")
                .ExportTo(testPackage10);

            var testPackage11 = Folder.Combine(_nugetFeedFolder, "TestNugetPackage.1.1.0.nupkg");
            EmbeddedResource
                .EndsWith("TestNugetPackage.1.1.0")
                .ExportTo(testPackage11);
        }

        [Test]
        public void VersionPackageCache_retrieve_package_versions_success()
        {
            //given
            var refreshCompleted = new ManualResetEventSlim(false);

            var  dashBoard = new DashBoard();
            var nugetFeed = new NugetFeed(){Uri = _nugetFeedFolder};

            dashBoard.AddFeed(nugetFeed);
            dashBoard.AddDeployment(new Deployment{FeedId = nugetFeed.Id, PackageId = TestPackageId});
            var versionCache = new PackageVersionCache(dashBoard);

            
            //when
            versionCache.Start();
            versionCache.Refreshed += refreshCompleted.Set;
            Assert.That(refreshCompleted.Wait(TimeSpan.FromSeconds(20)), Is.True, "Cache update timeout");

            //then
            var packageVersions = versionCache.GetPackageVersions(nugetFeed.Id, TestPackageId);
            var sampleVesrions = new[] { new Version(1,1,0,0), new Version(1,0,0,0) };
            Assert.That(packageVersions, Is.EquivalentTo(sampleVesrions));
        }

        [TearDown]
        public void Cleanup()
        {
            Directory.Delete(_nugetFeedFolder, true);
        }
    }
}