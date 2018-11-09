using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyFaceIdApp
{
    public class FaceApiManager
    {
        private static HttpClient _client;
        private static string _faceApiKey = "";
        private readonly string _uriBase = "https://westeurope.api.cognitive.microsoft.com/face/v1.0/detect";
        public FaceApiManager()
        {
            _client = new HttpClient();
        }

        public async Task<RootObject> MakeAnalysisRequest(byte[] byteData)
        {
            
            _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _faceApiKey);

            
            string requestParameters = "returnFaceId=true&returnFaceLandmarks=false" +
                "&returnFaceAttributes=age,gender";

            
            string uri = _uriBase + "?" + requestParameters;

            HttpResponseMessage response;

            byte[] mBytes;
            FileStream stream = new FileStream("img1.jpg",FileMode.Open);
            BinaryReader binaryReader = new BinaryReader(stream);
            mBytes = binaryReader.ReadBytes((int)stream.Length);

            using (ByteArrayContent content = new ByteArrayContent(mBytes))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await _client.PostAsync(uri, content);
                //Маленький размер фотографии
                RootObject data;
                try
                {
                    string contentString = await response.Content.ReadAsStringAsync();
                    contentString = contentString.TrimStart('[').TrimEnd(']');
                    data = JsonConvert.DeserializeObject<RootObject>(contentString);
                }
                catch (Exception e)
                {
                    data = null;
                    Console.WriteLine(e.Message);
                }
                
                return data;
            }
        }
    }

    #region POCO
    public class FaceRectangle
    {
        public int top { get; set; }
        public int left { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class FaceAttributes
    {
        public string gender { get; set; }
        public double age { get; set; }
    }

    public class RootObject
    {
        public string faceId { get; set; }
        public FaceRectangle faceRectangle { get; set; }
        public FaceAttributes faceAttributes { get; set; }
    }
    #endregion
}
