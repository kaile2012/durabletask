//  ----------------------------------------------------------------------------------
//  Copyright Microsoft Corporation
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  http://www.apache.org/licenses/LICENSE-2.0
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//  ----------------------------------------------------------------------------------

namespace DurableTask.AzureServiceFabric.Integration.Tests
{
    using System;
    using System.IO;

    using DurableTask.AzureServiceFabric.Integration.Tests.DeploymentUtil;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AssemblySetup
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            //Todo: This will fail if local cluster is not setup, currently the test code does not automatically
            //start a local cluster and that's a manual pre-req step.
            //Todo: Also this will assume that latest SF test application and service code is packaged, if that's not true
            //the issue might be hard to detect :-(
            DeploymentHelper.CleanAsync().Wait();
            DeploymentHelper.DeployAsync(ApplicationRootPath).Wait();
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            DeploymentHelper.CleanAsync().Wait();
        }

        static string ApplicationRootPath
        {
            get
            {
                var applicationPath = Constants.ApplicationPath;
                var packagePath = Path.Combine(applicationPath, "pkg", Constants.DebugOrRelease);

                if (!Directory.Exists(applicationPath))
                {
                    throw new Exception("Could not find test application path, define SourceRoot environment variable to the source path");
                }

                if (!Directory.Exists(packagePath))
                {
                    throw new Exception("Could not find test application package, make sure the test application is built and package generated before running the tests");
                }

                return applicationPath;
            }
        }
    }
}
