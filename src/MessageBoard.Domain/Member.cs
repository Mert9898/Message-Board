using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace MessageBoard.Domain
{
    [Serializable]
    public class Member : User
    {
        private static List<Member> _extent = new List<Member>();

        public static new IReadOnlyList<Member> Extent => _extent.AsReadOnly();

        public static void SetExtent(List<Member> loadedMembers)
        {
            _extent = loadedMembers ?? new List<Member>();
        }

        public static new void ClearExtent() => _extent.Clear();

        private string? _firstName;
        public string? FirstName
        {
            get => _firstName;
            set
            {
                if (value != null && string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("FirstName cannot be empty whitespace if provided.");
                _firstName = value;
            }
        }

        private string? _lastName;
        public string? LastName
        {
            get => _lastName;
            set
            {
                if (value != null && string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("LastName cannot be empty whitespace if provided.");
                _lastName = value;
            }
        }

        private string? _bio;
        public string? Bio
        {
            get => _bio;
            set
            {
                if (value != null && string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Bio cannot be empty whitespace if provided.");
                _bio = value;
            }
        }

        private List<BadgeType> _badges = new List<BadgeType>();

        [XmlArray("Badges")]
        [XmlArrayItem("Badge")]
        public List<BadgeType> BadgesList
        {
            get => _badges;
            set => _badges = value ?? new List<BadgeType>();
        }

        public void AddBadge(BadgeType badge)
        {
            if (!_badges.Contains(badge))
            {
                _badges.Add(badge);
            }
        }

        private int _postScore = 0;
        public int PostScore
        {
            get => _postScore;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Score cannot be negative.");
                _postScore = value;
            }
        }

        private int _commentScore = 0;
        public int CommentScore
        {
            get => _commentScore;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Score cannot be negative.");
                _commentScore = value;
            }
        }

        public int OverallScore => PostScore + CommentScore;

        private DateTime _joinedAt;
        public DateTime JoinedAt
        {
            get => _joinedAt;
            set
            {
                if (value > DateTime.Now.AddMinutes(1))
                    throw new ArgumentException("Join date cannot be in the future.");
                _joinedAt = value;
            }
        }

        public Member() : base()
        {
        }

        public Member(int userId, string email, string username, string password)
            : base(userId, email, username, password)
        {
            JoinedAt = DateTime.Now;
            _extent.Add(this);
        }

        public Member(string username, string password)
            : this(0, "default@example.com", username, password)
        {
        }
    }
}
