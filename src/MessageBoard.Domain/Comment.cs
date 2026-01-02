using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MessageBoard.Domain
{
    [Serializable]
    public class Comment
    {
        private static List<Comment> _extent = new List<Comment>();
        
        public static IReadOnlyList<Comment> Extent => _extent.AsReadOnly();
        
        public static void SetExtent(List<Comment> list) => _extent = list ?? new List<Comment>();
        
        public static void ClearExtent() => _extent.Clear();

        private int _commentId;
        public int CommentId
        {
            get => _commentId;
            set
            {
                if (value < 0) throw new ArgumentException("CommentId cannot be negative.");
                _commentId = value;
            }
        }

        private string _content;
        public string Content
        {
            get => _content;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Comment content cannot be empty.");
                _content = value;
            }
        }

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

        public Votes CommentVotes { get; set; } = new Votes();

        private Post _post;
        
        [XmlIgnore]
        public Post Post => _post;

        private Comment? _replyTo;
        
        [XmlIgnore]
        public Comment? ReplyTo => _replyTo;

        private List<Comment> _replies = new List<Comment>();
        
        [XmlIgnore]
        public IReadOnlyList<Comment> Replies => _replies.AsReadOnly();

        public Comment()
        {
        }

        public Comment(int commentId, string content, Post post)
        {
            if (post == null)
                throw new InvalidOperationException("A comment must belong to a post (composition rule).");

            CommentId = commentId;
            Content = content;
            CreatedAt = DateTime.Now;
            
            _extent.Add(this);
            
            SetPost(post);
        }

        internal void SetPost(Post post)
        {
            if (post == null)
                throw new InvalidOperationException("Cannot set null post (composition rule).");

            if (_post != null && _post != post)
                throw new InvalidOperationException("Cannot move comment to a different post (composition constraint).");

            if (_post == post) return;

            _post = post;
            post.AddComment(this);
        }

        public void SetReplyTo(Comment parentComment)
        {
            if (parentComment == this)
                throw new InvalidOperationException("A comment cannot reply to itself.");

            if (_replyTo == parentComment) return;

            if (_replyTo != null)
            {
                _replyTo.RemoveReply(this);
            }

            _replyTo = parentComment;

            if (parentComment != null)
            {
                parentComment.AddReply(this);
            }
        }

        internal void AddReply(Comment reply)
        {
            if (reply == null) return;
            if (_replies.Contains(reply)) return;

            _replies.Add(reply);

            if (reply._replyTo != this)
            {
                reply._replyTo = this;
            }
        }

        internal void RemoveReply(Comment reply)
        {
            if (reply == null) return;
            _replies.Remove(reply);
        }

        public void Delete()
        {
            if (_post != null)
            {
                _post.RemoveComment(this);
            }

            if (_replyTo != null)
            {
                _replyTo.RemoveReply(this);
            }

            foreach (var reply in _replies.ToArray())
            {
                reply._replyTo = null;
            }
            _replies.Clear();

            // Remove from extent
            _extent.Remove(this);
        }
    }
}
