using System;
using System.Collections.Generic;
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
        [SetUp]
        public void Setup()
        {
            // Reset state for ALL classes before each test
            Member.ClearExtent();
            Administrator.ClearExtent(); // <--- Added this
            Community.ClearExtent();
            Post.ClearExtent();
            Member.PasswordMinLength = 8;
        }

        // --------------------------------------------------------
        // 1. ATTRIBUTE VALIDATION TESTS
        // --------------------------------------------------------

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
            // Validating Administrator specific logic
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

        // --------------------------------------------------------
        // 2. DERIVED ATTRIBUTE TESTS
        // --------------------------------------------------------

        [Test]
        public void OverallScore_CalculatesCorrectly()
        {
            var member = new Member("John", "Password123");
            member.PostScore = 10;
            member.CommentScore = 5;

            Assert.AreEqual(15, member.OverallScore);
        }

        // --------------------------------------------------------
        // 3. MULTI-VALUE & CONSTRAINTS TESTS
        // --------------------------------------------------------

        [Test]
        public void Community_AddModerator_AddsSuccessfully()
        {
            var comm = new Community("TechTalks");
            comm.AddModerator("Alice");
            Assert.Contains("Alice", comm.ModeratorList);
        }

        [Test]
        public void Community_MaxModerators_ThrowsException()
        {
            var comm = new Community("PopularClub");
            // Add 10 moderators
            for (int i = 0; i < 10; i++)
            {
                comm.AddModerator($"Mod{i}");
            }

            // Attempt 11th
            Assert.Throws<InvalidOperationException>(() => comm.AddModerator("TooMany"));
        }

        // --------------------------------------------------------
        // 4. CLASS EXTENT TESTS
        // --------------------------------------------------------

        [Test]
        public void Member_Constructor_AddsToExtentAutomatically()
        {
            new Member("User1", "Pass1234");
            new Member("User2", "Pass1234");

            Assert.AreEqual(2, Member.Extent.Count);
        }

        [Test]
        public void Administrator_Constructor_AddsToExtentAutomatically()
        {
            new Administrator("Admin", "admin@test.com", "Secret123");
            Assert.AreEqual(1, Administrator.Extent.Count);
        }

        // --------------------------------------------------------
        // 5. PERSISTENCE TESTS
        // --------------------------------------------------------

        [Test]
        public void Persistence_SaveAndLoad_RetainsAllData()
        {
            // Arrange: Create objects in memory
            new Member("SavedUser", "Password123") { Bio = "I persist!" };
            new Administrator("SavedAdmin", "admin@test.com", "AdminPass123");
            new Community("SavedCommunity");

            // Act: Save to XML
            PersistenceManager.SaveData();
            
            // Clear memory to simulate application restart
            Member.ClearExtent();
            Administrator.ClearExtent();
            Community.ClearExtent();

            // Load back from XML
            PersistenceManager.LoadData();

            // Assert: Verify data is restored
            Assert.AreEqual(1, Member.Extent.Count);
            Assert.AreEqual("SavedUser", Member.Extent[0].Username);
            
            Assert.AreEqual(1, Administrator.Extent.Count);
            Assert.AreEqual("SavedAdmin", Administrator.Extent[0].Username);
            
            Assert.AreEqual(1, Community.Extent.Count);
            Assert.AreEqual("SavedCommunity", Community.Extent[0].Name);
        }
    }
}
