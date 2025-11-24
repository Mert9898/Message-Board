using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MessageBoard.Domain
{
    public class Community
    {
        private static List<Community> _extent = new List<Community>();
        public static IReadOnlyList<Community> Extent => _extent.AsReadOnly();
        
        public static void SetExtent(List<Community> list) => _extent = list ?? new List<Community>();
        public static void ClearExtent() => _extent.Clear();

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Community name required.");
                _name = value;
            }
        }

        // Multi-Value Attribute with Constraint (Max 10 Moderators)
        private List<string> _moderatorUsernames = new List<string>();

        [XmlArray("Moderators")]
        [XmlArrayItem("Username")]
        public List<string> ModeratorList
        {
            get => _moderatorUsernames;
            set => _moderatorUsernames = value;
        }

        public void AddModerator(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username invalid.");
            
            // Requirement: "Each community has a maximum limit of 10 moderators."
            if (_moderatorUsernames.Count >= 10)
                throw new InvalidOperationException("Maximum limit of 10 moderators reached.");

            if (!_moderatorUsernames.Contains(username))
                _moderatorUsernames.Add(username);
        }

        public Community() { }
        public Community(string name)
        {
            Name = name;
            _extent.Add(this);
        }
    }
}
