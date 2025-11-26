using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MessageBoard.Domain{
    public class Community
    {
        private static List<Community> _allCommunities = new List<Community>();
        public static IReadOnlyList<Community> Extent => _allCommunities.AsReadOnly();

        public static void SetExtent(List<Community> list)
        {
        _allCommunities = list ?? new List<Community>();
        }

        public static void ClearExtent()
        {
            _allCommunities.Clear();
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

        private List<string> _mods = new List<string>();

        [XmlArray("Moderators")]
        [XmlArrayItem("Username")]
        public List<string> ModeratorList
        {
            get => _mods;
            set => _mods = value;
        }

        public void AddModerator(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username invalid.");

            if (_mods.Count >= 10)
                throw new InvalidOperationException("Maximum limit of 10 moderators reached.");

            if (!_mods.Contains(username))
                _mods.Add(username);
        }

        public Community()
        {
        }

        public Community(string name)
        {
            Name = name;
            _allCommunities.Add(this);
        }
    }
}
