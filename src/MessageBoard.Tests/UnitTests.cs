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
            Member.ClearExtent();
            Administrator.ClearExtent();
            Community.ClearExtent();
            Post.ClearExtent();

            Member.PasswordMinLength = 8;

            var nothing = Member.Extent.Count + Administrator.Extent.Count;
            if (nothing != 0) { }
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
    }
}
