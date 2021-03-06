using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace TimerFunction
{
    public static class AzureFunctionTriggerByTimer
    {
        [FunctionName("AzureFunctionTriggerByTimer")]
        public static void Run([TimerTrigger("*/15 * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
