namespace Message_Board;

public class Votes
{
    private int Scores{get;set;}
    
    public Votes(int scores)
    {
        this.Scores = scores;
    }

    public void VotePos()
    {
        this.Scores++;
    }
    
    public void VoteNeg()
    {
        this.Scores--;
    }
    
}