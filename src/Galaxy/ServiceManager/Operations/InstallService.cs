﻿using Codestellation.Galaxy.ServiceManager.Helpers;
using System.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class InstallService : IOperation
    {
        private readonly string _serviceFolder;
        private readonly string _hostFileName;

        public InstallService(string serviceFolder, string hostFileName)
        {
            _serviceFolder = serviceFolder;
            _hostFileName = hostFileName;
        }

        public void Execute(TextWriter buildLog)
        {
            string exePath = Path.Combine(_serviceFolder, _hostFileName);

            string exeParams = string.Format("install");

            string error;
            var result = ProcessStarter.ExecuteWithParams(exePath, exeParams, out error);

            buildLog.WriteLine("Exe output:");
            buildLog.WriteLine(result);

            buildLog.WriteLine("Exe error:");
            buildLog.WriteLine(error);
        }
    }
}