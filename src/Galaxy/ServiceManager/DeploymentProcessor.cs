﻿using System;
using System.Collections.Generic;
using Codestellation.Galaxy.ServiceManager.EventParams;
using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Galaxy.ServiceManager.Helpers;
using System.Threading.Tasks;

namespace Codestellation.Galaxy.ServiceManager
{
    public class DeploymentProcessor
    {
        public DeploymentProcessor()
        {
        }

        private void ProcessInternal(DeploymentTask deploymentTask, EventHandler<DeploymentTaskCompletedEventArgs> callback)
        {
            Queue<ServiceOperation> localQueue = new Queue<ServiceOperation>(deploymentTask.Operations);

            OperationResult[] results = new OperationResult[localQueue.Count];

            int index = 0;
            while (localQueue.Count > 0)
            {
                var operation = localQueue.Dequeue();
                operation.Execute();
                results[index++] = operation.Result;
            }

            var deploymentResult = ResultsDescriberHelper.AggregateResults(deploymentTask, results);
            
            callback.Invoke(this,
                new DeploymentTaskCompletedEventArgs(
                    deploymentTask, 
                    deploymentResult));
        }

        public void Process(DeploymentTask deploymentTask, EventHandler<DeploymentTaskCompletedEventArgs> callback)
        {
            new Task(() => ProcessInternal(deploymentTask, callback)).Start();
        }
    }
}