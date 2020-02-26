// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.VisualStudio.Services.Agent.Tests.L1.Worker
{
    public class CoreL1Tests : L1TestBase
    {
        [Fact]
        [Trait("Level", "L1")]
        [Trait("Category", "Worker")]
        public async Task Test_Base()
        {
            // Arrange
            var message = LoadTemplateMessage();

            // Act
            var results = await RunWorker(message);

            // Assert
            AssertJobCompleted();
            Assert.Equal(TaskResult.Succeeded, results.Result);

            var steps = GetSteps();
            var expectedSteps = new[] { "Initialize job", "Checkout MyFirstProject@master to s", "CmdLine", "Post-job: Checkout MyFirstProject@master to s", "Finalize Job" };
            Assert.Equal(5, steps.Count()); // Init, Checkout, CmdLine, Post, Finalize
            for (var idx = 0; idx < steps.Count; idx++)
            {
                Assert.Equal(expectedSteps[idx], steps[idx].Name);
            }
        }
    }
}
