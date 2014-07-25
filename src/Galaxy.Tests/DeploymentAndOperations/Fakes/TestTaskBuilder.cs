﻿using Codestellation.Galaxy.ServiceManager;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations.Fakes
{
    public class TestTaskBuilder
    {
        public static DeploymentTask SequenceTaskSuccess()
        {
            var deployment = GetDeployment();
            var feed = GetFeed();
            var task = new DeploymentTask("TaskSuccess", deployment, feed, string.Empty);
            task.Add(new FakeOpSuccess(string.Empty, deployment));
            return task;
        }

        public static DeploymentTask SequenceTaskFail()
        {
            var deployment = GetDeployment();
            var feed = GetFeed();
            var task = new DeploymentTask("TaskFail", deployment, feed, "");
            task.Add(new FakeOpFail(string.Empty, deployment));
            return task;
        }

        public static DeploymentTask SequenceTaskFailInTheMiddle()
        {
            var deployment = GetDeployment();
            var feed = GetFeed();

            var task = new DeploymentTask("TaskFailInTheMiddle", deployment, feed, "");
            task.Add(new FakeOpSuccess(string.Empty, deployment));
            task.Add(new FakeOpFail(string.Empty, deployment));
            task.Add(new FakeOpSuccess(string.Empty, deployment));
            return task;
        }

        private static Deployment GetDeployment()
        {
            return new Deployment { DisplayName = "FooService" };
        }

        private static NugetFeed GetFeed()
        {
            return new NugetFeed();
        }
    }
}
