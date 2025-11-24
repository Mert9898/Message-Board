using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using MessageBoard.Domain;

namespace MessageBoard.Persistence
{
    // Container class for single-file serialization
    [XmlRoot("MessageBoardData")]
    public class DataContainer
    {
        public List<Member> Members { get; set; } = new List<Member>();
        public List<Community> Communities { get; set; } = new List<Community>();
        public List<Post> Posts { get; set; } = new List<Post>();
    }

    public static class PersistenceManager
    {
        private const string FilePath = "messageboard_data.xml";

        public static void SaveData()
        {
            var container = new DataContainer
            {
                Members = new List<Member>(Member.Extent),
                Communities = new List<Community>(Community.Extent),
                Posts = new List<Post>(Post.Extent)
            };

            XmlSerializer serializer = new XmlSerializer(typeof(DataContainer));
            using (StreamWriter writer = new StreamWriter(FilePath))
            {
                serializer.Serialize(writer, container);
            }
        }

        public static void LoadData()
        {
            if (!File.Exists(FilePath)) return;

            XmlSerializer serializer = new XmlSerializer(typeof(DataContainer));
            using (StreamReader reader = new StreamReader(FilePath))
            {
                try 
                {
                    var container = (DataContainer)serializer.Deserialize(reader);
                    
                    // Restore extents
                    Member.SetExtent(container.Members);
                    Community.SetExtent(container.Communities);
                    Post.SetExtent(container.Posts);
                }
                catch (Exception)
                {
                    // Handle corruption or empty files
                    Member.ClearExtent();
                    Community.ClearExtent();
                    Post.ClearExtent();
                }
            }
        }
    }
}
