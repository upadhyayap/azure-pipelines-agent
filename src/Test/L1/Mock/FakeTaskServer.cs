using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.VisualStudio.Services.Agent.Tests.L1.Worker
{
    public class FakeTaskServer : AgentService, ITaskServer
    {
        public Task ConnectAsync(VssConnection jobConnection)
        {
            return Task.CompletedTask;
        }

        public Task<Stream> GetTaskContentZipAsync(Guid taskId, TaskVersion taskVersion, CancellationToken token)
        {
            // Path to unzipped folders in externals
            String taskPath = Path.Join(HostContext.GetDirectory(WellKnownDirectory.Externals), "Tasks", taskId.ToString());

            // Zip the folder into the working directory
            String taskZipsPath = Path.Join(HostContext.GetDirectory(WellKnownDirectory.Work), "taskzips");
            String zip = Path.Join(taskZipsPath, taskId.ToString() + ".zip");
            if (Directory.Exists(taskPath) && !File.Exists(zip))
            {
                if (!Directory.Exists(taskZipsPath))
                {
                    Directory.CreateDirectory(taskZipsPath);
                }
                ZipFile.CreateFromDirectory(taskPath, zip);
            }

            if (File.Exists(zip))
            {
                return Task.FromResult<Stream>(File.OpenRead(zip));
            }
            else
            {
                throw new Exception("A step specified a task which does not exist in the L1 test framework. Any tasks used by L1 tests must be added manually.");
            }
        }

        public Task<bool> TaskDefinitionEndpointExist()
        {
            return Task.FromResult(true);
        }
    }
}