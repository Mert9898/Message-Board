using System;
using System.Collections.Generic;

namespace MessageBoard.Domain
{
    [Serializable]
    public class Moderator : User
    {
        private static List<Moderator> _extent = new List<Moderator>();
        
        public static new IReadOnlyList<Moderator> Extent => _extent.AsReadOnly();
        
        public static void SetExtent(List<Moderator> list) => _extent = list ?? new List<Moderator>();
        
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

        public Moderator() : base()
        {
        }

        public Moderator(int userId, string email, string username, string password, DateTime dateOfAssignment)
            : base(userId, email, username, password)
        {
            DateOfAssignment = dateOfAssignment;
            _extent.Add(this);
        }
    }
}
