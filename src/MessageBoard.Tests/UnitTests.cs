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
    }
}

