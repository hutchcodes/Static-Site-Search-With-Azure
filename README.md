[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fhutchcodes%2FJekyll-Search-With-Azure-Functions%2Fmaster%2FJekyllSearch.json)
<a href="http://armviz.io/#/?load=https%3A%2F%2Fraw.githubusercontent.com%2Fhutchcodes%2FJekyll-Search-With-Azure-Functions%2Fmaster%2FJekyllSearch.json" target="_blank">
    <img src="http://armviz.io/visualizebutton.png"/>
</a>


# Azure Search with Azure Functions

I built these Azure Functions to load a JSON representation of my blog into Azure Search and allow me to search for posts and pages with a bit of client side Javascript. My blog is a static site generated by Jekyll and is hosted on GitHub Pages, so there is no server side available.

Though this was created specifically for my Jekyll based blog I should be re-usable as long as the JSON passed in is an array of the `Page` objects (see [Page.csx](https://github.com/hutchcodes/Azure-Search-with-Azure-Functions/blob/master/Shared/Page.csx)).

# Application Settings

The following Application settings need to be configured in your Function App
- **`SearchJsonUrl`** - The URL of the *search.json* in your Jekyll site
- **`SearchServiceName`** - The name of your Azure Search instance
- **`SearchApiKey`** - The Admin Key of your Azure Search instance
- **`SarchIndexName`** - The name of the Azure Search Index you want to use

The Search Index will be deleted and recreated each time the BlogIndexer Function runs. This is because I have no way to pass deleted posts up to delete them from the Search Index.

# Searching

I've included some of the Jekyll files that drive the search. Missing are the actual search textbox and the category list that does a faceted search. If you need samples for that you can check out the repo for my [blog](https://github.com/hutchcodes/hutchcodes.github.io).

# Scoring Profiles

I haven't setup a Scoring Profile to give greater weight to hits in the title than the body. At this point I don't think I have enough Posts/Pages to justify it.

# Questions

You can find my contact info here https://hutchcodes.net/about/
