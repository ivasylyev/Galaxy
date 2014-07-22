﻿using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Helpers;
using Codestellation.Galaxy.ServiceManager.Operations;

namespace Codestellation.Galaxy.ServiceManager
{
    public static class DeploymentTaskServiceExtensions
    {
        public static DeploymentTask InstallPackage(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new InstallPackage(deploymentTask.BasePath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }

        public static DeploymentTask InstallPackage(this DeploymentTask deploymentTask, Deployment deployment, NugetFeed feed)
        {
            deploymentTask.Add
                (
                    new InstallPackage(deploymentTask.BasePath, deployment, feed)
                );
            return deploymentTask;
        }
        public static DeploymentTask InstallHostPackage(this DeploymentTask deploymentTask)
        {
            return deploymentTask.InstallPackage(HostDeployHelper.CreateDeployment(deploymentTask.Deployment), HostDeployHelper.CreateFeed());
        }

        public static DeploymentTask InstallService(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new InstallService(deploymentTask.BasePath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }

        public static DeploymentTask ProvideServiceConfig(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new ProvideHostConfig(deploymentTask.BasePath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }

        public static DeploymentTask StartService(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new StartService(deploymentTask.BasePath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }

        public static DeploymentTask StopService(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new StopService(deploymentTask.BasePath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }

        public static DeploymentTask UninstallPackage(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new UninstallPackage(deploymentTask.BasePath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }

        public static DeploymentTask UninstallService(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new UninstallService(deploymentTask.BasePath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }
        public static DeploymentTask CopyNugetsToRoot(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new CopyNugetsToRoot(deploymentTask.BasePath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }
        public static DeploymentTask DeployUserConfig(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new DeployUserConfig(deploymentTask.BasePath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }

        public static DeploymentTask ConfigurePlatform(this DeploymentTask deploymentTask)
        {
            deploymentTask.Add
                (
                    new ConfigurePlatform(deploymentTask.BasePath, deploymentTask.Deployment, deploymentTask.Feed)
                );
            return deploymentTask;
        }
    }
}
