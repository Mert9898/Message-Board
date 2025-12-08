namespace Message_Board;
    

public class Program
{
    public static void Main(string[] args)
    {
        //User user = new User(1, "test@test.com", "testUser", "password1");
        Member member = new Member(1, "test@test.com", "testUser", "password1");
        member.AwardBadge(BadgeType.SilverBadge);
        Console.WriteLine(member.GetBadges()[0]);
        
        
    }
}