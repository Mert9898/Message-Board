using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace MessageBoard.Domain
{
    [Serializable]
    [XmlInclude(typeof(Member))]
    [XmlInclude(typeof(Administrator))]
    [XmlInclude(typeof(Moderator))]
    public abstract class User
    {
        private static List<User> _extent = new List<User>();
        
        public static IReadOnlyList<User> Extent => _extent.AsReadOnly();
        
        public static void SetExtent(List<User> loadedUsers)
        {
            _extent = loadedUsers ?? new List<User>();
        }
        
        public static void ClearExtent() => _extent.Clear();

        private static int _passwordMinLength = 8;
        public static int PasswordMinLength
        {
            get => _passwordMinLength;
            set
            {
                if (value < 4) throw new ArgumentException("Minimum password length cannot be less than 4.");
                _passwordMinLength = value;
            }
        }

        private int _userId;
        public int UserId
        {
            get => _userId;
            set
            {
                if (value < 0) throw new ArgumentException("UserId cannot be negative.");
                _userId = value;
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Email cannot be empty.");
                
                var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(value, emailPattern))
                    throw new ArgumentException("Email format is invalid.");
                
                _email = value;
            }
        }

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Username cannot be empty.");
                _username = value;
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Password cannot be empty.");
                if (value.Length < PasswordMinLength)
                    throw new ArgumentException($"Password must be at least {PasswordMinLength} characters long.");
                _password = value;
            }
        }

        public bool Banned { get; set; }

        public Preferences UserPreferences { get; set; } = new Preferences();

        protected User()
        {
        }

        protected User(int userId, string email, string username, string password)
        {
            UserId = userId;
            Email = email;
            Username = username;
            Password = password;
            Banned = false;
            
            _extent.Add(this);
        }
    }
}
