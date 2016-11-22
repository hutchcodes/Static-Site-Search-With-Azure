#load "..\Shared\SearchHelper.csx"

using System.Net;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    // parse query parameter
    string search = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "search", true) == 0)
        .Value;

    string facet = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "facet", true) == 0)
        .Value;        

    var response = SearchHelper.Search(search, facet);

    return req.CreateResponse(HttpStatusCode.OK, response); 
} 