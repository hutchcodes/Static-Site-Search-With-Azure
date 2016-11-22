public class Page
{
    public string Id
    {
        get
        {
            if (string.IsNullOrEmpty(Url))
            {
                return Guid.NewGuid().GetHashCode().ToString();
            }
            else
            {
                return Url.GetHashCode().ToString();
            }
        }
    }
    public string Url { get; set; }
    public string Type { get; set; }
    public string Title { get; set; }
    public DateTime? PublishDate { get; set; }
    public string Content { get; set; }
    public string Excerpt { get; set; }
    public List<string> Categories { get; set; }

}