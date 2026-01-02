using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MessageBoard.Domain
{
    [Serializable]
    [XmlInclude(typeof(TextPost))]
    [XmlInclude(typeof(LinkPost))]
    [XmlInclude(typeof(ImagePost))]
    public abstract class Post
    {
        private static List<Post> _extent = new List<Post>();
        
        public static IReadOnlyList<Post> Extent => _extent.AsReadOnly();
        
        public static void SetExtent(List<Post> list) => _extent = list ?? new List<Post>();
        
        public static void ClearExtent() => _extent.Clear();

        private int _postId;
        public int PostId
        {
            get => _postId;
            set
            {
                if (value < 0) throw new ArgumentException("PostId cannot be negative.");
                _postId = value;
            }
        }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Title cannot be empty.");
                _title = value.Trim();
            }
        }

        public bool IsMature { get; set; }

        private DateTime _createdAt;
        public DateTime CreatedAt
        {
            get => _createdAt;
            set
            {
                if (value > DateTime.Now.AddMinutes(1))
                    throw new ArgumentException("CreatedAt cannot be in the future.");
                _createdAt = value;
            }
        }

        public Votes PostVotes { get; set; } = new Votes();

        private Community? _community;
        
        [XmlIgnore]
        public Community? Community => _community;

        private List<Comment> _comments = new List<Comment>();
        
        [XmlIgnore]
        public IReadOnlyList<Comment> Comments => _comments.AsReadOnly();

        protected Post()
        {
        }

        protected Post(int postId, string title, bool isMature)
        {
            PostId = postId;
            Title = title;
            IsMature = isMature;
            CreatedAt = DateTime.Now;
            
            _extent.Add(this);
        }

        public void SetCommunity(Community community)
        {
            if (_community == community) return;

            _community = community;

            if (community != null && !community.Posts.Contains(this))
            {
                community.AddPost(this);
            }
        }

        internal void AddComment(Comment comment)
        {
            if (comment == null) return;
            if (_comments.Contains(comment)) return;

            _comments.Add(comment);

            if (comment.Post != this)
            {
                comment.SetPost(this);
            }
        }

        internal void RemoveComment(Comment comment)
        {
            if (comment == null) return;
            _comments.Remove(comment);
        }

        public void Delete()
        {
            foreach (var comment in _comments.ToArray())
            {
                Comment.Extent.ToList().Remove(comment);
                
                comment.Delete();
            }
            _comments.Clear();

            if (_community != null)
            {
                _community.RemovePost(this);
            }

            _extent.Remove(this);
        }
    }
}
