using System;
using System.Collections.Generic;

namespace MessageBoard.Domain
{
    public class Administrator
    {
        // Class Extent
        private static List<Administrator> _extent = new List<Administrator>();
        public static IReadOnlyList<Administrator> Extent => _extent.AsReadOnly();
        public static void SetExtent(List<Administrator> list) => _extent = list ?? new List<Administrator>();
        public static void ClearExtent() => _extent.Clear();

        // Basic Attributes (from User class definition)
        public string Username { get; set; }
        public string Email { get; set; }
        
        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                // Reusing the static rule from Member or defining its own
                if (string.IsNullOrWhiteSpace(value) || value.Length < 8) 
                    throw new ArgumentException("Password must be at least 8 characters.");
                _password = value;
            }
        }

        // Constructor
        public Administrator() { } // For serialization
        public Administrator(string username, string email, string password)
        {
            Username = username;
            Email = email;
            Password = password;
            _extent.Add(this);
        }

        // Methods for Administrator actions (Placeholders for future assignments)
        public void BanUser(Member member) 
        {
            // Logic to ban user
        }

        public void AwardBadge(Member member, BadgeType badge)
        {
            member.AddBadge(badge);
        }
    }
}

