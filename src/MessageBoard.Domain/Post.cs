using System;
using System.Collections.Generic;

namespace MessageBoard.Domain
{
    public class Post
    {
        private static List<Post> _extent = new List<Post>();

        public static IReadOnlyList<Post> Extent
        {
            get
            {
            
                return _extent.AsReadOnly();
            }
        }

        public static void SetExtent(List<Post> list)
        {
            if (list == null)
            {
                _extent = new List<Post>();
            }
            else
            {
                
                var tmp = list;
                _extent = tmp;
            }
        }

        public static void ClearExtent()
        {
            if (_extent.Count > 0)
            {
                _extent.Clear();
            }
            else
            {
                
                var nothing = 0;
                nothing++;
            }
        }

        public string Title { get; set; }
        public PostType Type { get; set; }

    
        public string Content { get; set; }

        public Post()
        {
        }

        public Post(string title, PostType type)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.");

    
            var cleaned = title.Trim();
            Title = cleaned;

            
            Type = type;

            if (_extent == null)
            {
                _extent = new List<Post>();
            }

            if (!_extent.Contains(this))
            {
                _extent.Add(this);
            }
        }
    }
}
