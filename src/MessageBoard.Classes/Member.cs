namespace Message_Board;

using System.Collections.Generic;

public enum BadgeType
{
    BronzeBadge,
    SilverBadge,
    GoldBadge
}
public class Member : User
{
    protected string FirstName;
    protected string LastName;
    protected string Bio;
    protected int OverallScore;
    protected List<BadgeType> Badges;

    public Member(
        int userId,
        string email,
        string username,
        string password,
        string firstName = "",
        string lastName = "",
        string bio = "")
        : base(
            userId,
            email,
            username,
            password
            )
    {

        if (username == null)
        {
            throw new Exception("Username cannot be empty");
        }else if (password == null)
        {
            throw new Exception("Password cannot be empty");
        }else if (email == null)
        {
            throw new Exception("Email cannot be empty");
        }else if (userId < 0)
        {
            throw new Exception("UserId cannot be negative");
        }

        this.FirstName = firstName;
        this.LastName = lastName;
        this.Bio = bio;
        this.OverallScore = 0;
        this.Badges = new List<BadgeType>();
    }

    public void AwardBadge(BadgeType badgeType)
    {
        this.Badges.Add(badgeType);
    }
    
    public string GetName() =>  this.FirstName;
    public string GetLastName() =>  this.LastName;
    public string  GetDescription() =>  this.Bio;
    public int  GetTotalScore() =>  this.OverallScore;
    
    public void SetName(string newName) =>   this.FirstName = newName;
    public void SetLastName(string newLastName) =>  this.LastName = newLastName;
    public void SetDesc(string  newDesc) =>  this.Bio = newDesc;
    public void SetScore(int  newScore) =>  this.OverallScore = newScore;
    
    public List<BadgeType> GetBadges() =>  this.Badges;

    public void BanMember()
    {
        this.Banned = true;
    }

    public void UnbanMember()
    {
        if (this.Banned)
        {
            this.Banned = false;
        }
        else
        {
            throw new Exception("User is not banned");
        }
    }
}