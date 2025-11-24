using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace MessageBoard.Domain
{
    public class Member
    {
        // ==========================================
        // 1. Class Extent (Static Collection)
        // ==========================================
        private static List<Member> _extent = new List<Member>();

        // Encapsulation: Return ReadOnly wrapper
        public static IReadOnlyList<Member> Extent => _extent.AsReadOnly();

        // Persistence Helpers
        public static void SetExtent(List<Member> loadedMembers) 
        {
            _extent = loadedMembers ?? new List<Member>();
        }
        public static void ClearExtent() => _extent.Clear();

        // ==========================================
        // 2. Class/Static Attribute
        // ==========================================
        private static int _passwordMinLength = 8; // Default per requirements
        public static int PasswordMinLength
        {
            get => _passwordMinLength;
            set
            {
                if (value < 4) throw new ArgumentException("Minimum password length cannot be less than 4.");
                _passwordMinLength = value;
            }
        }

        // ==========================================
        // 3. Basic & Optional Attributes
        // ==========================================
        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Username cannot be empty.");
                _username = value;
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Password cannot be empty.");
                // Validation using Static Attribute
                if (value.Length < PasswordMinLength) 
                    throw new ArgumentException($"Password must be at least {PasswordMinLength} characters long.");
                _password = value;
            }
        }

        // Optional Attribute (Nullable)
        private string? _bio;
        public string? Bio
        {
            get => _bio;
            set
            {
                // If provided (not null), it must not be just whitespace
                if (value != null && string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Bio cannot be empty whitespace if provided.");
                _bio = value;
            }
        }

        // ==========================================
        // 4. Multi-Value Attribute (Badges)
        // ==========================================
        // Using a private list with encapsulation
        private List<BadgeType> _badges = new List<BadgeType>();

        // XML Proxy for Serialization (public property needed for XmlSerializer)
        [XmlArray("Badges")]
        [XmlArrayItem("Badge")]
        public List<BadgeType> BadgesList
        {
            get => _badges;
            set => _badges = value;
        }

        public void AddBadge(BadgeType badge)
        {
            if (!_badges.Contains(badge)) _badges.Add(badge);
        }

        // ==========================================
        // 5. Derived Attribute
        // ==========================================
        private int _postScore = 0;
        public int PostScore
        {
            get => _postScore;
            set 
            { 
                // Although scores can be negative in some systems, let's assume 0 floor for this assignment example
                // or allow negative but ensure it's a valid integer. 
                // Let's enforce non-negative for the sake of "Inputting negative numbers" check in requirements.
                if (value < 0) throw new ArgumentException("Score cannot be negative."); 
                _postScore = value; 
            }
        }

        private int _commentScore = 0;
        public int CommentScore
        {
            get => _commentScore;
            set 
            { 
                if (value < 0) throw new ArgumentException("Score cannot be negative.");
                _commentScore = value; 
            }
        }

        // Derived: Calculated on the fly
        public int OverallScore => PostScore + CommentScore;

        // ==========================================
        // 6. Complex Attribute (Composition)
        // ==========================================
        public Preferences UserPreferences { get; set; } = new Preferences();
        
        // Complex Attribute (Time check)
        private DateTime _joinedAt;
        public DateTime JoinedAt
        {
            get => _joinedAt;
            set
            {
                if (value > DateTime.Now.AddMinutes(1)) throw new ArgumentException("Join date cannot be in the future.");
                _joinedAt = value;
            }
        }

        // ==========================================
        // Constructors
        // ==========================================
        public Member() { } // Required for XML Serialization

        public Member(string username, string password)
        {
            Username = username;
            Password = password;
            JoinedAt = DateTime.Now;
            
            // Auto-add to extent
            _extent.Add(this);
        }
    }
}
