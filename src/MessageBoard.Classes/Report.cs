namespace Message_Board;

public class Report
{
    private int Identifier { get; set; }
    private string Reason { get; set; }

    private static List<Report> _reports = new List<Report>();

    public Report(int identifier, string reason)
    {
        if (identifier < 0)
        {
            throw new Exception("identifier cannot be negative");
        }
        if (reason == null)
        {
            throw new Exception("Specify the reason of the report.");
        }

        this.Identifier = identifier;
        this.Reason = reason;
    }

    public int GetIdentifier()
    {
        return this.Identifier;
    }
    
    public static List<Report> GetReports()
    {
        return _reports;
    }
}