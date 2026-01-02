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

        private HashSet<Post> _posts = new HashSet<Post>();

        [XmlIgnore]
        public IReadOnlyCollection<Post> Posts => _posts;

        private Dictionary<string, Subscription> _subscribers = new Dictionary<string, Subscription>();

        [XmlIgnore]
        public IReadOnlyCollection<Subscription> Subscriptions => _subscribers.Values;

        public Community()
        {
        }

        public Community(string name)
        {
            Name = name;
            IsPrivate = false;
            _extent.Add(this);
        }

        public void AddPost(Post post)
        {
            if (post == null)
                throw new ArgumentNullException(nameof(post));

            if (_posts.Contains(post)) return;

            _posts.Add(post);

            if (post.Community != this)
            {
                post.SetCommunity(this);
            }
        }

        internal void RemovePost(Post post)
        {
            if (post == null) return;
            _posts.Remove(post);
        }

        internal void AddSubscriber(Subscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            if (subscription.Member == null)
                throw new InvalidOperationException("Subscription must have a valid member.");

            string username = subscription.Member.Username;

            if (_subscribers.ContainsKey(username))
                throw new InvalidOperationException($"Member '{username}' is already subscribed to this community.");

            _subscribers[username] = subscription;
        }

        public Subscription? GetSubscriber(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty.");

            _subscribers.TryGetValue(username, out var subscription);
            return subscription;
        }

        public bool IsSubscribed(string username)
        {
            return _subscribers.ContainsKey(username);
        }

        internal void RemoveSubscriber(Subscription subscription)
        {
            if (subscription == null || subscription.Member == null) return;
            
            string username = subscription.Member.Username;
            _subscribers.Remove(username);
        }
    }
}
