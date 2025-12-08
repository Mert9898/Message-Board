namespace Message_Board;

public class Subscription
{
    private DateTime DateOfJoin {get;set;}

    public Subscription()
    {
        this.DateOfJoin = DateTime.Now;
    }
}