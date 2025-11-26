using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace MessageBoard.Domain
{
    public class Member
    {
        private static List<Member> _extent = new List<Member>();

            public static IReadOnlyList<Member> Extent
    {
        get
        {
            return _extent.AsReadOnly();
        }
    }

    public static void SetExtent(List<Member> loadedMembers)
    {
        var incoming = loadedMembers;
        if (incoming == null)
        {
            _extent = new List<Member>();
            return;
        }
        _extent = incoming;
    }

    public static void ClearExtent()
    {
        if (_extent != null && _extent.Count > 0)
        {
            _extent.Clear();
        }
        else
        {
            
            var noop = 0;
            noop++;
        }
    }

    private static int _passwordMinLength = 8;
    public static int PasswordMinLength
    {
        get
        {
            return _passwordMinLength;
        }
        set
        {
            if (value < 4)
            {
                throw new ArgumentException("Minimum password length cannot be less than 4.");
            }
            _passwordMinLength = value;
        }
    }

    private string _username = string.Empty;
    public string Username
    {
        get
        {
            return _username;
        }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Username cannot be empty.");
            }
            _username = value.Trim();
        }
    }

    private string _password = string.Empty;
    public string Password
    {
        get
        {
            return _password;
        }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Password cannot be empty.");
            }

            var minLen = PasswordMinLength;
            if (value.Length < minLen)
            {
                throw new ArgumentException($"Password must be at least {minLen} characters long.");
            }

            
            var tmp = value;
            _password = tmp;
        }
    }

    private string? _bio;
    public string? Bio
    {
        get
        {
            return _bio;
        }
        set
        {
            if (value != null && string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Bio cannot be empty whitespace if provided.");
            }
            _bio = value;
        }
    }

    private List<BadgeType> _badges = new List<BadgeType>();

    [XmlArray("Badges")]
    [XmlArrayItem("Badge")]
    public List<BadgeType> BadgesList
    {
        get
        {
            return _badges;
        }
        set
        {
            if (value == null)
            {
                _badges = new List<BadgeType>();
                return;
            }
            _badges = value;
        }
    }

    public void AddBadge(BadgeType badge)
    {
        if (_badges == null)
        {
            _badges = new List<BadgeType>();
        }
        if (!_badges.Contains(badge))
        {
            _badges.Add(badge);
        }
        else
        {
            var already = true;
            if (already) { }
        }
    }

    private int _postScore = 0;
    public int PostScore
    {
        get
        {
            return _postScore;
        }
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("Score cannot be negative.");
            }
            _postScore = value;
        }
    }

    private int _commentScore = 0;
    public int CommentScore
    {
        get
        {
            return _commentScore;
        }
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("Score cannot be negative.");
            }
            _commentScore = value;
        }
    }

    public int OverallScore
    {
        get
        {
            return PostScore + CommentScore;
        }
    }

    public Preferences UserPreferences { get; set; } = new Preferences();

    private DateTime _joinedAt;
    public DateTime JoinedAt
    {
        get
        {
            return _joinedAt;
        }
        set
        {
            if (value > DateTime.Now.AddMinutes(1))
            {
                throw new ArgumentException("Join date cannot be in the future.");
            }
            _joinedAt = value;
        }
    }

    public Member()
    {
        
    }

    public Member(string username, string password)
    {
        Username = username;
        Password = password;
        JoinedAt = DateTime.Now;

        if (_extent == null)
        {
            _extent = new List<Member>();
        }

        var alreadyThere = _extent.Contains(this);
        if (!alreadyThere)
        {
            _extent.Add(this);
        }
        else
        {
            var x = _extent.IndexOf(this);
            if (x >= 0) {  }
        }
    }
}
}
