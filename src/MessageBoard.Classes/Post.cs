namespace Message_Board;

public class Post
{
    protected int PostId {get; set; }
    protected string Title {get; set; }
    protected bool IsMature{get; set; }
    protected DateTime CreatedAt {get; set;}

    public Post(int  postId, string title, bool isMature)
    {
        if (postId < 0)
        {
            throw new Exception("postId cannot be negative");
        }
        if (title.Trim() == null)
        {
            throw new ArgumentException("Title must not be null.");
        }

        this.PostId = postId;
        this.Title = title;
        this.IsMature = isMature;
        
        this.CreatedAt = DateTime.Now;
    }
    
    
    public void ReportPost(string reason)
    {
        Report.GetReports()
            .Add(
                new Report(
                    Report.GetReports().Last().GetIdentifier() + 1,
                    "post + " + this.Title + " was reported for: " + reason
                    )
                );
    }

    public static void RemovePost(Post post)
    {
        post = null;
    }

    public Post CreatePost(int postId, string title, bool isMature)
    {
        return new Post(postId, title, isMature);
    }
}