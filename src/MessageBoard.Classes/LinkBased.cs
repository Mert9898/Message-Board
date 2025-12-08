namespace Message_Board;

public class LinkBased : Post
{
    private List<string> links {get; set; }

    public LinkBased(int postId, string title, bool isMature, params string[] links) : base(postId, title, isMature)
    {
        this.links = links.ToList();
    }
}