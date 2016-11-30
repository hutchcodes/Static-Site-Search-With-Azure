#load "Settings.csx"
#load "Page.csx"

using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

public static class SearchHelper 
{
    public static string Search(string search = null, string facet = null, int start = 0)
    {
        var indexClient = GetIndexClient();

        string facetFilter = "";
        if (!string.IsNullOrEmpty(facet))
        {
            facetFilter = $"Categories/any(c: c eq '{facet}')";
        }

        var searchParams = new SearchParameters();
        searchParams.IncludeTotalResultCount = true;
        searchParams.QueryType = QueryType.Full;
        searchParams.SearchMode = SearchMode.Any;
        searchParams.Top = 10000;
        searchParams.Select = new[] { "Url", "Type", "Title", "Excerpt", "Categories", "PublishDate" };
        searchParams.Filter = facetFilter;
        searchParams.Facets = new[] { "Categories" };
        var searchResults = indexClient.Documents.Search<Page>(search, searchParams);

        string response = "";

        foreach (var r in searchResults.Results)
        {
            if (r.Document.Type == "post") {

                var categories = "";
                foreach (var c in r.Document.Categories)
                {
                    categories += $"<a href='/archives/#{c}'>{c}</a>&nbsp;";
                }
                if (!string.IsNullOrEmpty(categories))
                {
                    categories = " in " + categories;
                }

                response += $"<article class='post'> <header class='jumbotron'> <h2 class='postTitle'><a href='{r.Document.Url}'>{r.Document.Title}</a></h2> <abbr class='postDate' title='{r.Document.PublishDate}'>{r.Document.PublishDate?.ToString("MMMM dd, yyyy")}</abbr>{categories}</header> <div class='articleBody'>{r.Document.Excerpt}</div><div><a href='{r.Document.Url}'>Continue Reading</a></div></article>";
            }
            else
            {
                response += $"<article class='post'> <header class='jumbotron'> <h2 class='postTitle'><a href='{r.Document.Url}'>{r.Document.Title}</a></h2></header> <div class='articleBody'>{r.Document.Excerpt}</div><div><a href='{r.Document.Url}'>Continue Reading</a></div></article>";
            }
        }
        return response;
    }

    public static void UpdateSearch(IEnumerable<Page> searchTopics)
    {
        DeleteIndex();
        SearchIndexClient indexClient = GetIndexClient();

        var actions = new List<IndexAction<Page>>();
        var topicsCount = searchTopics.Count();
        var updateCount = 0;
        foreach (var ast in searchTopics)
        {
            var action = IndexAction.Upload(ast);

            actions.Add(action);
            updateCount++;
            //Max upload size 1000
            if (actions.Count == 1000 || actions.Count == topicsCount || updateCount == topicsCount)
            {
                var batch = new IndexBatch<Page>(actions);
                indexClient.Documents.Index(batch);

                actions = new List<IndexAction<Page>>();
            }
        }

    }

    private static SearchIndexClient GetIndexClient()
    {
        var queryApiKey = Settings.GetSetting("SearchApiKey"); 
        var searchServiceName = Settings.GetSetting("SearchServiceName");
        var searchIndexName = Settings.GetSetting("SearchIndexName");

        var searchServiceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(queryApiKey));

        if (!searchServiceClient.Indexes.Exists(searchIndexName))
        {
            CreateIndex(searchServiceClient);
        }

        var indexClient = searchServiceClient.Indexes.GetClient(searchIndexName);

        return indexClient;
    }

    private static void CreateIndex(SearchServiceClient searchServiceClient)
    {
        var searchIndexName = Settings.GetSetting("SearchIndexName");

        if (!searchServiceClient.Indexes.Exists(searchIndexName))
        {
            var definition = new Index()
            {
                Name = searchIndexName,
                Fields = new[]
                {
                    new Field("Id", DataType.String)                                { IsFilterable = true, IsKey = true },
                    new Field("Url", DataType.String)                               { IsRetrievable = true },
                    new Field("Type", DataType.String)                              { IsFilterable = true, IsRetrievable = true, IsFacetable = true },
                    new Field("Title", DataType.String)                             { IsSearchable = true, IsRetrievable = true, Analyzer = AnalyzerName.EnMicrosoft },
                    new Field("Categories", DataType.Collection(DataType.String))   { IsFilterable = true, IsRetrievable = true, IsFacetable = true },
                    new Field("Tags", DataType.Collection(DataType.String))         { IsFilterable = true, IsRetrievable = true, IsFacetable = true },
                    new Field("Content", DataType.String)                           { IsSearchable = true, Analyzer = AnalyzerName.EnMicrosoft },
                    new Field("Excerpt", DataType.String)                           { IsSearchable = true, IsRetrievable = true, Analyzer = AnalyzerName.EnMicrosoft },
                    new Field("PublishDate", DataType.String)                       { IsRetrievable = true }
                }
            };
            searchServiceClient.Indexes.Create(definition);
        }
    }

    private static void DeleteIndex()
    {
        var queryApiKey = Settings.GetSetting("SearchApiKey"); 
        var searchServiceName = Settings.GetSetting("SearchServiceName");
        var searchIndexName = Settings.GetSetting("SearchIndexName");

        var searchServiceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(queryApiKey));

        if (!searchServiceClient.Indexes.Exists(searchIndexName))
        {
            searchServiceClient.Indexes.Delete(searchIndexName);
        }
    }
}