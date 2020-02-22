﻿//  ----------------------------------------------------------------------------------
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

namespace TestApplication.Common.Orchestrations
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using DurableTask.Core;

    using TestApplication.Common.OrchestrationTasks;

    public class RecurringOrchestration : TaskOrchestration<int, RecurringOrchestrationInput>
    {
        // HACK: This is just a hack to communicate result of orchestration back to test
        private static int targetOchestrationInvocationsCount = 0;

        public override async Task<int> RunTask(OrchestrationContext context, RecurringOrchestrationInput input)
        {
            var testTasks = context.CreateClient<ITestTasks>();

            if (targetOchestrationInvocationsCount == 0)
            {
                // First time, Reset Generation Count variable.
                await testTasks.ResetGenerationCounter();
            }

            int result = await context.CreateSubOrchestrationInstance<int>(input.TargetOrchestrationType,
                GetTargetOrchestrationVersion(),
                input.TargetOrchestrationInstanceId,
                input.TargetOrchestrationInput);

            await context.CreateTimer(GetNextExecutionTime(context), true, CancellationToken.None);
            if (ShouldRepeatTargetOrchestration())
            {
                context.ContinueAsNew(input);
            }
            else
            {
                // Finally, Reset Generation Count variable.
                await testTasks.ResetGenerationCounter();
            }

            return result;
        }

        public virtual DateTime GetNextExecutionTime(OrchestrationContext context)
        {
            return context.CurrentUtcDateTime.AddSeconds(Math.Pow(2, targetOchestrationInvocationsCount));
        }

        public virtual string GetTargetOrchestrationVersion()
        {
            return string.Empty;
        }

        public virtual bool ShouldRepeatTargetOrchestration()
        {
            targetOchestrationInvocationsCount++;

            return targetOchestrationInvocationsCount < 4;
        }
    }
}