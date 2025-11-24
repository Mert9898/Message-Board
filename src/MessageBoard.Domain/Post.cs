using System;
using System.Collections.Generic;

namespace MessageBoard.Domain
{
    public class Post
    {
        private static List<Post> _extent = new List<Post>();
        public static IReadOnlyList<Post> Extent => _extent.AsReadOnly();
        public static void SetExtent(List<Post> list) => _extent = list ?? new List<Post>();
        public static void ClearExtent() => _extent.Clear();

        public string Title { get; set; }
        public PostType Type { get; set; }
        public string Content { get; set; } // Could be Text, URL, or Image Path

        public Post() { }
        public Post(string title, PostType type)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title cannot be empty.");
            Title = title;
            Type = type;
            _extent.Add(this);
        }
    }
}
