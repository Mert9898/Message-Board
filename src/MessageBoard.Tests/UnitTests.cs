using System;
using System.IO;
using System.Linq;
using MessageBoard.Domain;
using MessageBoard.Persistence;
using NUnit.Framework;

namespace MessageBoard.Tests
{
    [TestFixture]
    public class MessageBoardTests
    {
        private const string DataFile = "messageboard_data.xml";

        [SetUp]
        public void Setup()
        {
            User.ClearExtent();
            Member.ClearExtent();
            Administrator.ClearExtent();
            Moderator.ClearExtent();
            Community.ClearExtent();
            Post.ClearExtent();
            Comment.ClearExtent();
            Subscription.ClearExtent();

            User.PasswordMinLength = 8;
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                if (File.Exists(DataFile)) File.Delete(DataFile);
            }
            catch
            {
            }
        }

        [Test]
        public void CreateMember_EmptyUsername_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new Member("", "Password123"));
        }

        [Test]
        public void CreateMember_ShortPassword_ThrowsException()
        {
            Member.PasswordMinLength = 8;
            Assert.Throws<ArgumentException>(() => new Member("JohnDoe", "Short"));
        }

        [Test]
        public void Administrator_ShortPassword_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new Administrator("AdminUser", "admin@test.com", "short"));
        }

        [Test]
        public void CreateMember_FutureJoinDate_ThrowsException()
        {
            var member = new Member("John", "Password123");
            Assert.Throws<ArgumentException>(() => member.JoinedAt = DateTime.Now.AddDays(1));
        }

        [Test]
        public void SetScore_NegativeValue_ThrowsException()
        {
            var member = new Member("John", "Password123");
            Assert.Throws<ArgumentException>(() => member.PostScore = -5);
        }

        [Test]
        public void OverallScore_CalculatesCorrectly()
        {
            var member = new Member("John", "Password123");
            member.PostScore = 10;
            member.CommentScore = 5;

            Assert.That(member.OverallScore, Is.EqualTo(15));
        }

        [Test]
        public void Community_AddModerator_AddsSuccessfully()
        {
            var comm = new Community("TechTalks");
            comm.AddModerator("Alice");

            CollectionAssert.Contains(comm.ModeratorList, "Alice");
        }

        [Test]
        public void Community_MaxModerators_ThrowsException()
        {
            var comm = new Community("PopularClub");
            for (int i = 0; i < 10; i++)
            {
                comm.AddModerator($"Mod{i}");
            }

            Assert.Throws<InvalidOperationException>(() => comm.AddModerator("TooMany"));
        }

        [Test]
        public void Member_Constructor_AddsToExtentAutomatically()
        {
            new Member("User1", "Pass1234");
            new Member("User2", "Pass1234");

            Assert.That(Member.Extent.Count, Is.EqualTo(2));
            var names = Member.Extent.Select(m => m.Username).ToList();
            CollectionAssert.IsSubsetOf(new[] { "User1", "User2" }, names);
        }

        [Test]
        public void Administrator_Constructor_AddsToExtentAutomatically()
        {
            new Administrator("Admin", "admin@test.com", "Secret123");
            Assert.That(Administrator.Extent.Count, Is.EqualTo(1));

            Assert.That(Administrator.Extent[0].Username, Is.EqualTo("Admin"));
        }

        [Test]
        public void Persistence_SaveAndLoad_RetainsAllData()
        {
            new Member("SavedUser", "Password123") { Bio = "I persist!" };
            new Administrator("SavedAdmin", "admin@test.com", "AdminPass123");
            new Community("SavedCommunity");

            PersistenceManager.SaveData();

            Member.ClearExtent();
            Administrator.ClearExtent();
            Community.ClearExtent();
            Post.ClearExtent();

            Assert.That(Member.Extent.Count, Is.EqualTo(0));
            Assert.That(Administrator.Extent.Count, Is.EqualTo(0));
            Assert.That(Community.Extent.Count, Is.EqualTo(0));

            PersistenceManager.LoadData();

            Assert.That(Member.Extent.Count, Is.EqualTo(1));
            Assert.That(Member.Extent[0].Username, Is.EqualTo("SavedUser"));
            Assert.That(Member.Extent[0].Bio, Is.EqualTo("I persist!"));

            Assert.That(Administrator.Extent.Count, Is.EqualTo(1));
            Assert.That(Administrator.Extent[0].Username, Is.EqualTo("SavedAdmin"));

            Assert.That(Community.Extent.Count, Is.EqualTo(1));
            Assert.That(Community.Extent[0].Name, Is.EqualTo("SavedCommunity"));
        }

        [Test]
        public void User_InvalidEmail_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new Member(1, "invalid-email", "User1", "Password123"));
        }

        [Test]
        public void User_StaticPasswordMinLength_WorksCorrectly()
        {
            User.PasswordMinLength = 10;
            Assert.Throws<ArgumentException>(() => new Member(1, "user@test.com", "User1", "Short123"));
            User.PasswordMinLength = 8;
        }

        [Test]
        public void Member_OptionalAttributes_CanBeNull()
        {
            var member = new Member("TestUser", "Password123");
            member.FirstName = null;
            member.LastName = null;
            member.Bio = null;
            
            Assert.That(member.FirstName, Is.Null);
            Assert.That(member.LastName, Is.Null);
            Assert.That(member.Bio, Is.Null);
        }

        [Test]
        public void Member_ComplexAttribute_PreferencesWorks()
        {
            var member = new Member("User1", "Password123");
            member.UserPreferences.ShowMatureContent = true;
            member.UserPreferences.Theme = "Dark";
            
            Assert.That(member.UserPreferences.ShowMatureContent, Is.True);
            Assert.That(member.UserPreferences.Theme, Is.EqualTo("Dark"));
        }

        [Test]
        public void Community_StaticMaxModerators_EnforcesLimit()
        {
            Community.MaxModerators = 3;
            var comm = new Community("SmallCommunity");
            comm.AddModerator("Mod1");
            comm.AddModerator("Mod2");
            comm.AddModerator("Mod3");
            
            Assert.Throws<InvalidOperationException>(() => comm.AddModerator("Mod4"));
            Community.MaxModerators = 10;
        }

        [Test]
        public void Community_OptionalDescription_WorksCorrectly()
        {
            var comm = new Community("TestCommunity");
            comm.Description = null;
            Assert.That(comm.Description, Is.Null);
            
            comm.Description = "A test description";
            Assert.That(comm.Description, Is.EqualTo("A test description"));
        }

        [Test]
        public void TextPost_Inheritance_WorksCorrectly()
        {
            var post = new TextPost(1, "Test Post", false, "This is the content.");
            Assert.That(post.Title, Is.EqualTo("Test Post"));
            Assert.That(post.Text, Is.EqualTo("This is the content."));
            Assert.That(Post.Extent.Count, Is.GreaterThan(0));
        }

        [Test]
        public void LinkPost_MultiValueAttribute_AddsLinks()
        {
            var post = new LinkPost(2, "Link Collection", false);
            post.AddLink("https://example.com");
            post.AddLink("https://test.com");
            
            Assert.That(post.Links.Count, Is.EqualTo(2));
            CollectionAssert.Contains(post.Links, "https://example.com");
        }

        [Test]
        public void ImagePost_MultiValueAttribute_AddsImages()
        {
            var post = new ImagePost(3, "Image Gallery", false);
            post.AddImageUrl("https://example.com/image1.jpg");
            post.AddImageUrl("https://example.com/image2.jpg");
            
            Assert.That(post.ImageUrls.Count, Is.EqualTo(2));
        }

        [Test]
        public void Moderator_Inheritance_WorksCorrectly()
        {
            var mod = new Moderator(1, "mod@test.com", "ModUser", "ModPassword123", DateTime.Now);
            Assert.That(mod.Username, Is.EqualTo("ModUser"));
            Assert.That(mod.Email, Is.EqualTo("mod@test.com"));
            Assert.That(Moderator.Extent.Count, Is.EqualTo(1));
        }

        [Test]
        public void Administrator_CanBanUser()
        {
            var admin = new Administrator("Admin1", "admin@test.com", "AdminPass123");
            var member = new Member("RegularUser", "Password123");
            
            admin.BanUser(member);
            Assert.That(member.Banned, Is.True);
        }

        [Test]
        public void Votes_ComplexAttribute_WorksCorrectly()
        {
            var post = new TextPost(10, "Voted Post", false, "Content here");
            post.PostVotes.VotePos();
            post.PostVotes.VotePos();
            post.PostVotes.VoteNeg();
            
            Assert.That(post.PostVotes.Score, Is.EqualTo(1));
        }

        [Test]
        public void Post_CreatedAt_CannotBeFuture()
        {
            var post = new TextPost(20, "Future Post", false, "Content");
            Assert.Throws<ArgumentException>(() => post.CreatedAt = DateTime.Now.AddDays(1));
        }

        [Test]
        public void Member_DerivedAttribute_OverallScore_Recalculates()
        {
            var member = new Member("ScoreUser", "Password123");
            member.PostScore = 20;
            member.CommentScore = 15;
            
            Assert.That(member.OverallScore, Is.EqualTo(35));
            
            member.PostScore = 10;
            Assert.That(member.OverallScore, Is.EqualTo(25));
        }

        [Test]
        public void BasicAssociation_CommunityAddPost_ReverseConnectionWorks()
        {
            var community = new Community("Tech Community");
            var post = new TextPost(100, "Test Post", false, "Content here");

            community.AddPost(post);

            CollectionAssert.Contains(community.Posts, post);
            
            Assert.That(post.Community, Is.EqualTo(community));
        }

        [Test]
        public void BasicAssociation_PostSetCommunity_ReverseConnectionWorks()
        {
            var community = new Community("Gaming Community");
            var post = new TextPost(101, "Gaming Post", false, "Let's play!");

            post.SetCommunity(community);

            Assert.That(post.Community, Is.EqualTo(community));
            
            CollectionAssert.Contains(community.Posts, post);
        }

        [Test]
        public void BasicAssociation_PreventsDuplicatePosts()
        {
            var community = new Community("Test Community");
            var post = new TextPost(102, "Test Post", false, "Content");

            community.AddPost(post);
            community.AddPost(post);

            Assert.That(community.Posts.Count(p => p == post), Is.EqualTo(1));
        }

        [Test]
        public void Composition_CommentCannotExistWithoutPost_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => 
                new Comment(1, "Test comment", null));
        }

        [Test]
        public void Composition_CommentBelongsToPost_ReverseConnectionWorks()
        {
            var post = new TextPost(200, "Discussion Post", false, "Let's discuss");
            var comment = new Comment(1, "Great post!", post);

            Assert.That(comment.Post, Is.EqualTo(post));
            
            CollectionAssert.Contains(post.Comments, comment);
        }

        [Test]
        public void Composition_DeletePost_CascadeDeletesComments()
        {
            var post = new TextPost(201, "Post to Delete", false, "Content");
            var comment1 = new Comment(10, "Comment 1", post);
            var comment2 = new Comment(11, "Comment 2", post);

            Assert.That(Comment.Extent.Count, Is.GreaterThanOrEqualTo(2));
            Assert.That(post.Comments.Count, Is.EqualTo(2));

            post.Delete();

            CollectionAssert.DoesNotContain(Comment.Extent, comment1);
            CollectionAssert.DoesNotContain(Comment.Extent, comment2);
        }

        [Test]
        public void Composition_CannotMoveCommentToAnotherPost_ThrowsException()
        {
            var post1 = new TextPost(202, "Post 1", false, "First post");
            var post2 = new TextPost(203, "Post 2", false, "Second post");
            var comment = new Comment(12, "My comment", post1);

            Assert.That(comment.Post, Is.EqualTo(post1));
            
            Assert.That(post1.Comments.Contains(comment), Is.True);
        }

        [Test]
        public void AssociationWithAttribute_CreateSubscription_ConnectsBothSides()
        {
            var member = new Member("JohnDoe", "Password123");
            var community = new Community("Book Club");

            var subscription = new Subscription(1, member, community, false);

            Assert.That(subscription.Member, Is.EqualTo(member));
            Assert.That(subscription.Community, Is.EqualTo(community));
            
            CollectionAssert.Contains(member.Subscriptions, subscription);
            CollectionAssert.Contains(community.Subscriptions, subscription);
        }

        [Test]
        public void AssociationWithAttribute_SubscriptionAttributes_StoreJoinDateAndRole()
        {
            var member = new Member("Alice", "Password123");
            var community = new Community("Science Hub");

            var subscription = new Subscription(2, member, community, isModerator: true);

            Assert.That(subscription.IsModerator, Is.True);
            Assert.That(subscription.JoinedAt, Is.LessThanOrEqualTo(DateTime.Now));
        }

        [Test]
        public void AssociationWithAttribute_DuplicateSubscription_ThrowsException()
        {
            var member = new Member("Bob", "Password123");
            var community = new Community("Sports Fan");

            new Subscription(3, member, community);

            Assert.Throws<InvalidOperationException>(() => 
                new Subscription(4, member, community));
        }

        [Test]
        public void AssociationWithAttribute_PromoteToModerator_RespectsLimit()
        {
            Community.MaxModerators = 2;
            var community = new Community("Small Community");
            
            var member1 = new Member("Mod1", "Password123");
            var member2 = new Member("Mod2", "Password123");
            var member3 = new Member("Member3", "Password123");

            var sub1 = new Subscription(10, member1, community);
            var sub2 = new Subscription(11, member2, community);
            var sub3 = new Subscription(12, member3, community);

            sub1.PromoteToModerator();
            sub2.PromoteToModerator();

            Assert.Throws<InvalidOperationException>(() => sub3.PromoteToModerator());
            
            Community.MaxModerators = 10;
        }

        [Test]
        public void AssociationWithAttribute_MemberHelperMethods_Work()
        {
            var member = new Member("Charlie", "Password123");
            var community = new Community("Art Gallery");
            var subscription = new Subscription(20, member, community, isModerator: true);

            Assert.That(member.IsSubscribedTo(community), Is.True);
            Assert.That(member.IsModeratorOf(community), Is.True);
        }

        [Test]
        public void QualifiedAssociation_GetSubscriberByUsername_Works()
        {
            var community = new Community("Developer Hub");
            var member1 = new Member("DevUser1", "Password123");
            var member2 = new Member("DevUser2", "Password123");

            new Subscription(30, member1, community);
            new Subscription(31, member2, community);

            var subscription = community.GetSubscriber("DevUser1");

            Assert.That(subscription, Is.Not.Null);
            Assert.That(subscription.Member.Username, Is.EqualTo("DevUser1"));
        }

        [Test]
        public void QualifiedAssociation_GetNonExistentSubscriber_ReturnsNull()
        {
            var community = new Community("Private Club");

            var subscription = community.GetSubscriber("NonExistentUser");

            Assert.That(subscription, Is.Null);
        }

        [Test]
        public void QualifiedAssociation_IsSubscribed_ChecksCorrectly()
        {
            var community = new Community("Movie Fans");
            var member = new Member("CinemaLover", "Password123");

            Assert.That(community.IsSubscribed("CinemaLover"), Is.False);

            new Subscription(40, member, community);

            Assert.That(community.IsSubscribed("CinemaLover"), Is.True);
        }

        [Test]
        public void ReflexiveAssociation_CommentReplyToComment_ReverseConnectionWorks()
        {
            var post = new TextPost(300, "Discussion", false, "Let's talk");
            var parentComment = new Comment(50, "Parent comment", post);
            var replyComment = new Comment(51, "Reply to parent", post);

            replyComment.SetReplyTo(parentComment);

            Assert.That(replyComment.ReplyTo, Is.EqualTo(parentComment));
            
            CollectionAssert.Contains(parentComment.Replies, replyComment);
        }

        [Test]
        public void ReflexiveAssociation_CommentCannotReplyToItself_ThrowsException()
        {
            var post = new TextPost(301, "Test Post", false, "Content");
            var comment = new Comment(52, "Self-referencing comment", post);

            Assert.Throws<InvalidOperationException>(() => comment.SetReplyTo(comment));
        }

        [Test]
        public void ReflexiveAssociation_NestedReplies_Work()
        {
            var post = new TextPost(302, "Thread", false, "Start thread");
            var comment1 = new Comment(60, "Level 1", post);
            var comment2 = new Comment(61, "Level 2", post);
            var comment3 = new Comment(62, "Level 3", post);

            comment2.SetReplyTo(comment1);
            comment3.SetReplyTo(comment2);

            Assert.That(comment1.Replies.Count, Is.EqualTo(1));
            Assert.That(comment2.Replies.Count, Is.EqualTo(1));
            Assert.That(comment3.Replies.Count, Is.EqualTo(0));
            Assert.That(comment3.ReplyTo.ReplyTo, Is.EqualTo(comment1));
        }

        [Test]
        public void Exception_MaxModeratorsLimit_Enforced()
        {
            Community.MaxModerators = 3;
            var community = new Community("Limited Community");
            
            var member1 = new Member("U1", "Password123");
            var member2 = new Member("U2", "Password123");
            var member3 = new Member("U3", "Password123");
            var member4 = new Member("U4", "Password123");

            var sub1 = new Subscription(100, member1, community);
            var sub2 = new Subscription(101, member2, community);
            var sub3 = new Subscription(102, member3, community);
            var sub4 = new Subscription(103, member4, community);

            sub1.PromoteToModerator();
            sub2.PromoteToModerator();
            sub3.PromoteToModerator();

            Assert.Throws<InvalidOperationException>(() => sub4.PromoteToModerator());
            
            Community.MaxModerators = 10;
        }

        [Test]
        public void Exception_DuplicateSubscription_Prevented()
        {
            var member = new Member("TestUser", "Password123");
            var community = new Community("Test Community");

            new Subscription(200, member, community);

            Assert.Throws<InvalidOperationException>(() => 
                new Subscription(201, member, community));
        }

        [Test]
        public void Exception_CompositionViolation_CommentWithoutPost()
        {
            Assert.Throws<InvalidOperationException>(() => 
                new Comment(999, "Orphan comment", null));
        }

        [Test]
        public void Exception_ReflexiveSelfReference_Prevented()
        {
            var post = new TextPost(400, "Test", false, "Content");
            var comment = new Comment(500, "Comment", post);

            Assert.Throws<InvalidOperationException>(() => 
                comment.SetReplyTo(comment));
        }
    }
}
