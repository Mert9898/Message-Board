using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MessageBoard.Domain
{
    [Serializable]
    public class Community
    {
        private static List<Community> _extent = new List<Community>();
        
        public static IReadOnlyList<Community> Extent => _extent.AsReadOnly();

        public static void SetExtent(List<Community> list)
        {
            _extent = list ?? new List<Community>();
        }

        public static void ClearExtent()
        {
            _extent.Clear();
        }

        private static int _maxModerators = 10;
        public static int MaxModerators
        {
            get => _maxModerators;
            set
            {
                if (value < 1) throw new ArgumentException("MaxModerators must be at least 1.");
                _maxModerators = value;
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Community name required.");
                _name = value;
            }
        }

        private string? _description;
        public string? Description
        {
            get => _description;
            set
            {
                if (value != null && string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Description cannot be empty whitespace if provided.");
                _description = value;
            }
        }

        public bool IsPrivate { get; set; }

        private List<string> _moderators = new List<string>();

        [ XmlArray("Moderators")]
        [XmlArrayItem("Username")]
        public List<string> ModeratorList
        {
            get => _moderators;
            set => _moderators = value ?? new List<string>();
        }

        public void AddModerator(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username invalid.");

            if (_moderators.Count >= MaxModerators)
                throw new InvalidOperationException($"Maximum limit of {MaxModerators} moderators reached.");

            if (!_moderators.Contains(username))
                _moderators.Add(username);
        }

        public Community()
        {
        }

        public Community(string name)
        {
            Name = name;
            IsPrivate = false;
            _extent.Add(this);
        }
    }
}
