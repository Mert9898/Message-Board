using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MessageBoard.Domain
{
    [Serializable]
    public class LinkPost : Post
    {
        private List<string> _links = new List<string>();

        [XmlArray("Links")]
        [XmlArrayItem("Link")]
        public List<string> Links
        {
            get => _links;
            set => _links = value ?? new List<string>();
        }

        public void AddLink(string link)
        {
            if (string.IsNullOrWhiteSpace(link))
                throw new ArgumentException("Link cannot be empty.");
            
            if (!_links.Contains(link))
            {
                _links.Add(link);
            }
        }

        public LinkPost() : base()
        {
        }

        public LinkPost(int postId, string title, bool isMature)
            : base(postId, title, isMature)
        {
        }
    }
}
