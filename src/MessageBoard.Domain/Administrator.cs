using System;
using System.Collections.Generic;

namespace MessageBoard.Domain
{
    [Serializable]
    public class Administrator : User
    {
        private static List<Administrator> _extent = new List<Administrator>();
        
        public static new IReadOnlyList<Administrator> Extent => _extent.AsReadOnly();
        
        public static void SetExtent(List<Administrator> list) => _extent = list ?? new List<Administrator>();
        
        public static new void ClearExtent() => _extent.Clear();

        private DateTime _dateOfAssignment;
        public DateTime DateOfAssignment
        {
            get => _dateOfAssignment;
            set
            {
                if (value > DateTime.Now)
                    throw new ArgumentException("Date of assignment cannot be in the future.");
                _dateOfAssignment = value;
            }
        }

        public Administrator() : base()
        {
        }

        public Administrator(int userId, string email, string username, string password)
            : base(userId, email, username, password)
        {
            DateOfAssignment = DateTime.Now;
            _extent.Add(this);
        }

        public Administrator(string username, string email, string password)
            : this(0, email, username, password)
        {
        }

        public void BanUser(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.Banned = true;
        }

        public void AwardBadge(Member member, BadgeType badge)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));
            member.AddBadge(badge);
        }
    }
}
