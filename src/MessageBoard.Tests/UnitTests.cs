using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MessageBoard.Domain;
using MessageBoard.Persistence;
using NUnit.Framework; // Assuming NUnit

namespace MessageBoard.Tests
{
    [TestFixture]
    public class MessageBoardTests
    {
        [SetUp]
        public void Setup()
        {
            // Reset state before each test
            Member.ClearExtent();
            Community.ClearExtent();
            Post.ClearExtent();
            Member.PasswordMinLength = 8; // Reset default
        }

        // --------------------------------------------------------
        // ATTRIBUTE VALIDATION TESTS (Requirement 2c)
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
            Assert.Throws<ArgumentException>(() => new Member("JohnDoe", "Short")); // 5 chars
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
        // DERIVED ATTRIBUTE TESTS
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
        // MULTI-VALUE & CONSTRAINTS TESTS
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
        // CLASS EXTENT & ENCAPSULATION TESTS
        // --------------------------------------------------------

        [Test]
        public void Member_Constructor_AddsToExtentAutomatically()
        {
            new Member("User1", "Pass1234");
            new Member("User2", "Pass1234");

            Assert.AreEqual(2, Member.Extent.Count);
        }

        [Test]
        public void Extent_IsReadOnly()
        {
            // Check if the list returned is castable to ICollection to attempt modification
            // Or simply verify the signature returns IReadOnlyList
            Assert.IsInstanceOf<IReadOnlyList<Member>>(Member.Extent);
        }

        // --------------------------------------------------------
        // PERSISTENCE TESTS (Requirement 3 & 4)
        // --------------------------------------------------------

        [Test]
        public void Persistence_SaveAndLoad_RetainsData()
        {
            // Arrange
            new Member("SavedUser", "Password123") { Bio = "I persist!" };
            new Community("SavedCommunity");

            // Act
            PersistenceManager.SaveData();
            
            // Clear memory to simulate restart
            Member.ClearExtent();
            Community.ClearExtent();

            // Load
            PersistenceManager.LoadData();

            // Assert
            Assert.AreEqual(1, Member.Extent.Count);
            Assert.AreEqual("SavedUser", Member.Extent[0].Username);
            Assert.AreEqual("I persist!", Member.Extent[0].Bio);
            Assert.AreEqual(1, Community.Extent.Count);
        }
    }
}
