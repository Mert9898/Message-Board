using System.Drawing;

namespace Message_Board;

public class ImagePost : Post
{
    private List<Uri> images {get; set; }

    public ImagePost(int postId, string title, bool isMature, params Uri[] images) : base(postId, title, isMature)
    {
        this.images = images.ToList();
    }
}