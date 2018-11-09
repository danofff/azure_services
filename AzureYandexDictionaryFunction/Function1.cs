
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net.Http;
using System;
using System.Collections.Generic;

namespace AzureYandexDictionaryFunction
{
    public static class AzureYandexDictionaryFunc
    {
        #region POCO

        public class Head
        {
        }

        public class Syn
        {
            public string text { get; set; }
            public string pos { get; set; }
        }

        public class Tr
        {
            public string text { get; set; }
            public string pos { get; set; }
            public List<Syn> syn { get; set; }
        }

        public class Def
        {
            public string text { get; set; }
            public string pos { get; set; }
            public List<Tr> tr { get; set; }
        }

        public class RootObject
        {
            public Head head { get; set; }
            public List<Def> def { get; set; }
        }
        #endregion

        private static string yandexK = "dict.1.1.20181020T041539Z.98ec98cd3bdd40d0.a4e7f3a80a9569cb350a379556cb5b74543a7206";
        private static string baseUrl = "https://dictionary.yandex.net/api/v1/dicservice.json/lookup?key=";

        [FunctionName("AzureYandexDictionaryFunc")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {

            log.Info("C# HTTP trigger function processed a request.");

            string word = req.Query["word"];
            Uri uri = new Uri(baseUrl);
            HttpClient client = new HttpClient();
            client.BaseAddress = uri;
            string requestUri = $"{yandexK}&lang=ru-ru&text={word}";
            string jsonRes = client.GetStringAsync(baseUrl+requestUri).Result;
            
            RootObject data = JsonConvert.DeserializeObject<RootObject>(jsonRes);
            

            return jsonRes != null
                ? (ActionResult)new OkObjectResult($"{data.def[0].tr[0].text}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
