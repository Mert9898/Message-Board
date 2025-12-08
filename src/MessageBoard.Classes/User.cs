namespace Message_Board;

public class User
{
    protected int UserId { get; set; }
    protected string Email { get; set; }
    protected string Username { get; set; }
    protected string Password { get; set; }
    protected bool Banned = false;

    protected static int PasswordLength = 8;

    public User(int userId, string email, string username, string password)
    {

        if (password.Length <= PasswordLength)
        {
            throw new Exception("Password is too short");
            //return;
        }

        this.UserId = userId;
        this.Email = email;
        this.Username = username;
        this.Password = password;
    }

    public void BanUser()
    {
        if (this.Banned == true)
        {
            throw new Exception("User is already banned");
        }

        this.Banned = true;
    }

    public void RemoveBan()
    {
        if (this.Banned == false)
        {
            throw new Exception("User is not banned");
        }

        this.Banned = false;
    }

    public int GetId()
    {
        return this.UserId;
    }

    public string GetUsername()
    {
        return this.Username;
    }

    public string GetEmail()
    {
        return this.Email;
    }

    public string GetPassword()
    {
        return this.Password;
    }

    public bool GetBan()
    {
        return this.Banned;
    }


}