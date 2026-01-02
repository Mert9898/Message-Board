using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MessageBoard.Domain
{
    [Serializable]
    public class Subscription
    {
        private static List<Subscription> _extent = new List<Subscription>();
        
        public static IReadOnlyList<Subscription> Extent => _extent.AsReadOnly();
        
        public static void SetExtent(List<Subscription> list) => _extent = list ?? new List<Subscription>();
        
        public static void ClearExtent() => _extent.Clear();

        private int _subscriptionId;
        public int SubscriptionId
        {
            get => _subscriptionId;
            set
            {
                if (value < 0) throw new ArgumentException("SubscriptionId cannot be negative.");
                _subscriptionId = value;
            }
        }

        private DateTime _joinedAt;
        public DateTime JoinedAt
        {
            get => _joinedAt;
            set
            {
                if (value > DateTime.Now.AddMinutes(1))
                    throw new ArgumentException("JoinedAt cannot be in the future.");
                _joinedAt = value;
            }
        }

        public bool IsModerator { get; set; }

        private Member _member;
        
        [XmlIgnore]
        public Member Member => _member;

        private Community _community;
        
        [XmlIgnore]
        public Community Community => _community;

        public Subscription()
        {
        }

        public Subscription(int subscriptionId, Member member, Community community, bool isModerator = false)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member), "Member cannot be null.");
            if (community == null)
                throw new ArgumentNullException(nameof(community), "Community cannot be null.");

            SubscriptionId = subscriptionId;
            JoinedAt = DateTime.Now;
            IsModerator = isModerator;

            _extent.Add(this);

            SetMember(member);
            SetCommunity(community);
        }

        internal void SetMember(Member member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            if (_member == member) return;

            _member = member;
            member.AddSubscription(this);
        }

        internal void SetCommunity(Community community)
        {
            if (community == null)
                throw new ArgumentNullException(nameof(community));

            if (_community == community) return;

            _community = community;
            community.AddSubscriber(this);
        }

        public void PromoteToModerator()
        {
            if (IsModerator)
                throw new InvalidOperationException("Member is already a moderator.");

            if (_community == null)
                throw new InvalidOperationException("Cannot promote: subscription has no community.");

            int currentModeratorCount = 0;
            foreach (var sub in _community.Subscriptions)
            {
                if (sub.IsModerator)
                    currentModeratorCount++;
            }

            if (currentModeratorCount >= Community.MaxModerators)
                throw new InvalidOperationException($"Maximum limit of {Community.MaxModerators} moderators reached.");

            IsModerator = true;
        }

        public void DemoteFromModerator()
        {
            if (!IsModerator)
                throw new InvalidOperationException("Member is not a moderator.");

            IsModerator = false;
        }

        public void Delete()
        {
            if (_member != null)
            {
                _member.RemoveSubscription(this);
            }

            if (_community != null)
            {
                _community.RemoveSubscriber(this);
            }

            _extent.Remove(this);
        }
    }
}
