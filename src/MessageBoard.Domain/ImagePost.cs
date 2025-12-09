using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MessageBoard.Domain
{
    [Serializable]
    public class ImagePost : Post
    {
        private List<string> _imageUrls = new List<string>();

        [XmlArray("ImageUrls")]
        [XmlArrayItem("ImageUrl")]
        public List<string> ImageUrls
        {
            get => _imageUrls;
            set => _imageUrls = value ?? new List<string>();
        }

        public void AddImageUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Image URL cannot be empty.");
            
            if (!_imageUrls.Contains(url))
            {
                _imageUrls.Add(url);
            }
        }

        public ImagePost() : base()
        {
        }

        public ImagePost(int postId, string title, bool isMature)
            : base(postId, title, isMature)
        {
        }
    }
}
