﻿using Codestellation.Galaxy.Domain;
using System.Linq;
using System.ServiceProcess;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public abstract class WinServiceOperation : OperationBase
    {
        public WinServiceOperation(string basePath, Deployment deployment, NugetFeed feed) :
            base(basePath, deployment, feed)
        {
            
        }

        protected bool IsServiceExists(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            return services.Any(item => item.ServiceName == serviceName);
        }
    }
}
