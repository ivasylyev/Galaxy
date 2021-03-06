﻿using Codestellation.Emisstar;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Domain.Deployments;
using Codestellation.Galaxy.Infrastructure;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    public class DeploymentTaskCompletedHandler : IHandler<DeploymentTaskCompletedEvent>
    {
        private readonly DeploymentBoard _board;
        private readonly PackageBoard _cache;
        private readonly Repository _repository;

        public DeploymentTaskCompletedHandler(DeploymentBoard board, Repository repository, PackageBoard cache)
        {
            _board = board;
            _repository = repository;
            _cache = cache;
        }

        public void Handle(DeploymentTaskCompletedEvent message)
        {
            Deployment deployment;
            if (_board.TryGetDeployment(message.Task.DeploymentId, out deployment))
            {
                deployment.Status = message.Result.Details;
                SaveDeployment(deployment);
            }
        }

        private void SaveDeployment(Deployment deployment)
        {
            var deployments = _repository.GetCollection<Deployment>();

            using (var tx = deployments.BeginTransaction())
            {
                deployments.Save(deployment, false);
                tx.Commit();
            }
        }
    }
}