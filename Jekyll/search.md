---
title: Searching...
layout: page
includeInSearch: false
permalink: /search/
published: true
---

<script>
var query = getParameterByName('search');
var cat = getParameterByName('cat');
var tag = getParameterByName('tag');

document.getElementById("searchText").value = query;

getSearchResults(query, cat, tag);
</script>


