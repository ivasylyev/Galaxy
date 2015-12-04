﻿using System.IO;
using Newtonsoft.Json.Linq;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class DeployHostConfig : IOperation
    {
        private readonly string _serviceFolder;

        public DeployHostConfig(string serviceFolder)
        {
            _serviceFolder = serviceFolder;
        }

        public void Execute(DeploymentTaskContext context)
        {
            dynamic hostConfig = new JObject();

            hostConfig.Folders = context.GetValue<object>(DeploymentTaskContext.Folders);
            hostConfig.Consul = new
            {
                Name = context.GetValue<string>(DeploymentTaskContext.ConsulName),
                Address = context.GetValue<string>(DeploymentTaskContext.ConsulAddress)
            };

            var hostConfigString = hostConfig.ToString();

            context.BuildLog.WriteLine("Generated host config");
            context.BuildLog.WriteLine(hostConfigString);

            WriteConfig(context.BuildLog, hostConfigString);
        }

        private void WriteConfig(TextWriter buildLog, string hostConfig)
        {
            var configPath = Path.Combine(_serviceFolder, "config.json");

            buildLog.WriteLine("Write host config to '{0}'", configPath);
            File.WriteAllText(configPath, hostConfig);
            buildLog.WriteLine("Config successfully written");
        }
    }
}