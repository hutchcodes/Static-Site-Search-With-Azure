#load "Settings.csx"
#load "Page.csx"

using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

public static class SearchHelper 
{
    public static DocumentSearchResult<Page> Search(string search = null, string cat = null, string tag = null)
    {
        var indexClient = GetIndexClient();

        string facetFilter = "";
        if (!string.IsNullOrEmpty(cat))
        {
            facetFilter = $"Categories/any(c: c eq '{cat}')";
        }
        if (!string.IsNullOrEmpty(tag))
        {
            facetFilter = $"Tags/any(c: c eq '{tag}')";
        }

        var searchParams = new SearchParameters();
        searchParams.IncludeTotalResultCount = true;
        searchParams.QueryType = QueryType.Full;
        searchParams.SearchMode = SearchMode.Any;
        searchParams.Top = 10000;
        searchParams.Select = new[] { "Url", "Type", "Title", "Excerpt", "Categories", "Tags", "PublishDate" };
        searchParams.Filter = facetFilter;
        searchParams.Facets = new[] { "Categories", "Tags" };
        var searchResults = indexClient.Documents.Search<Page>(search, searchParams);

        return searchResults;
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

        if (searchServiceClient.Indexes.Exists(searchIndexName))
        {
            searchServiceClient.Indexes.Delete(searchIndexName);
        }
    }
}