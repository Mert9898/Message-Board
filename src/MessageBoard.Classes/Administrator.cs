namespace Message_Board;

public class Administrator
{
    private User _user;
    private DateTime _dateOfAssingment;

    public Administrator(User user)
    {
        this._dateOfAssingment = DateTime.Now;
        this._user = user;
    }
}