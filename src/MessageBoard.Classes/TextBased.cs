namespace Message_Board;

public class TextBased : Post
{
    private string Text {get; set; }

    public TextBased(int postId, string title, bool isMature, string text) : base(postId, title, isMature)
    {
        this.Text = text;
    }
}