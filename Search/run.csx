#load "..\Shared\SearchHelper.csx"

using System.Net;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    // parse query parameter
    string search = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "search", true) == 0)
        .Value;

    string cat = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "cat", true) == 0)
        .Value;        

	string tag = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "tag", true) == 0)
        .Value; 

    var response = SearchHelper.Search(search, cat, tag);

    return req.CreateResponse(HttpStatusCode.OK, response); 
} 