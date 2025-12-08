namespace Message_Board;

public class Comment
{
    private string Contents {get; set; }
    private DateTime DateOfCreation {get; set; }
    
    public Comment(string contents)
    {
        if (contents == null)
        {
            throw new Exception("Comment content is Empty");
        }

        this.Contents = contents;
        this.DateOfCreation = DateTime.Now;
    }
    
    public void ReportComment(string reason)
    {
        Report.GetReports()
            .Add(
                new Report(
                    Report.GetReports().Last().GetIdentifier() + 1,
                    "comment + " + this.Contents + " was reported for: " + reason
                )
            );
    }
    
    public Comment LeaveComment(string contents)
    {
        return new Comment(contents);
    }
}