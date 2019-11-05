using Ambition.Fetcher;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Threading.Tasks;

namespace Ambition.Pipeline
{
    public class ConsolePipeline : IPipeline
    {
        public Task HandleAsync(FetchResult fetchResult)
        {
            if (fetchResult.ContentType == ContentType.Json)
            {
                if (fetchResult.DeserializedObject != null)
                {
                    var jsonSerializerSetting = new JsonSerializerSettings();
                    jsonSerializerSetting.Converters.Add(new StringEnumConverter());
                    jsonSerializerSetting.Formatting = Formatting.Indented;

                    var json = JsonConvert.SerializeObject(fetchResult.DeserializedObject, jsonSerializerSetting);
                    Console.WriteLine(json);
                }
            }
            else if (fetchResult.ContentType == ContentType.Html)
            {
                Console.WriteLine(fetchResult.Content);
            }
            return Task.CompletedTask;
        }
    }
}