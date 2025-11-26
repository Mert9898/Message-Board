using System;
using System.Collections.Generic;

namespace MessageBoard.Domain
{
public class Administrator
{
    private static List<Administrator> _adminCache = new List<Administrator>();
   
    public static IReadOnlyList<Administrator> Extent => _adminCache.AsReadOnly();

    public static void SetExtent(List<Administrator> list)
    {
        _adminCache = list ?? new List<Administrator>();
    }

    public static void ClearExtent() => _adminCache.Clear();

    public string Username { get; set; }

    public string Email { get; set; }

    private string _pwd;

    public string Password
    {
        get => _pwd;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters. C’mon, security matters…");

            _pwd = value;
        }
    }

    public Administrator()
    {
        
    }

    public Administrator(string username, string email, string password)
    {
        Username = username;
        Email = email;
        Password = password;

        _adminCache.Add(this);
    }

    public void BanUser(Member member)
    {
        
    }

    public void AwardBadge(Member member, BadgeType badge)
    {
        member.AddBadge(badge);
    }
}

}
