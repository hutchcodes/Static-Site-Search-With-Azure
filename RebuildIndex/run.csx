#load "..\Shared\SearchHelper.csx"
#r "Newtonsoft.Json"

using System.Net;
using Newtonsoft.Json;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");
 
    WebClient client = new WebClient();
    client.Encoding = System.Text.Encoding.UTF8;
    string value = client.DownloadString(Settings.GetSetting("SearchJsonUrl");
    var searchItems = JsonConvert.DeserializeObject<List<Page>>(value);

    SearchHelper.UpdateSearch(searchItems);

    return req.CreateResponse(HttpStatusCode.OK, "Updated Search Index");
} 