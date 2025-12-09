using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using MessageBoard.Domain;

namespace MessageBoard.Persistence
{
    [XmlRoot("MessageBoardData")]
    public class DataContainer
    {
        public List<User> Users { get; set; } = new List<User>();
        public List<Member> Members { get; set; } = new List<Member>();
        public List<Administrator> Administrators { get; set; } = new List<Administrator>();
        public List<Moderator> Moderators { get; set; } = new List<Moderator>();
        public List<Post> Posts { get; set; } = new List<Post>();
        public List<Community> Communities { get; set; } = new List<Community>();
    }

    public static class PersistenceManager
    {
        private const string DataFilePath = "messageboard_data.xml";
        
        private static XmlSerializer GetSerializer() => new XmlSerializer(typeof(DataContainer));

        public static void SaveData()
        {
            var usersSnapshot = new List<User>(User.Extent ?? Array.Empty<User>());
            var membersSnapshot = new List<Member>(Member.Extent ?? Array.Empty<Member>());
            var adminsSnapshot = new List<Administrator>(Administrator.Extent ?? Array.Empty<Administrator>());
            var modsSnapshot = new List<Moderator>(Moderator.Extent ?? Array.Empty<Moderator>());
            var communitiesSnapshot = new List<Community>(Community.Extent ?? Array.Empty<Community>());
            var postsSnapshot = new List<Post>(Post.Extent ?? Array.Empty<Post>());

            var container = new DataContainer
            {
                Users = usersSnapshot,
                Members = membersSnapshot,
                Administrators = adminsSnapshot,
                Moderators = modsSnapshot,
                Communities = communitiesSnapshot,
                Posts = postsSnapshot
            };

            var serializer = GetSerializer();
            var tempPath = Path.GetTempFileName();
            
            try
            {
                using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var writer = new StreamWriter(fs, System.Text.Encoding.UTF8))
                {
                    serializer.Serialize(writer, container);
                    writer.Flush();
                    try
                    {
                        fs.Flush(true);
                    }
                    catch
                    {
                    }
                }

                if (File.Exists(DataFilePath))
                {
                    try
                    {
                        var backup = DataFilePath + ".bak";
                        File.Replace(tempPath, DataFilePath, backup, ignoreMetadataErrors: true);
                        if (File.Exists(backup)) File.Delete(backup);
                    }
                    catch
                    {
                        try
                        {
                            File.Copy(tempPath, DataFilePath, overwrite: true);
                            File.Delete(tempPath);
                        }
                        catch
                        {
                            File.Delete(DataFilePath);
                            File.Move(tempPath, DataFilePath);
                        }
                    }
                }
                else
                {
                    File.Move(tempPath, DataFilePath);
                }
            }
            catch
            {
                if (File.Exists(tempPath))
                {
                    try { File.Delete(tempPath); } catch { }
                }
                throw;
            }
        }

        public static void LoadData()
        {
            if (!File.Exists(DataFilePath))
                return;

            var serializer = GetSerializer();
            DataContainer container;

            try
            {
                using (var reader = new StreamReader(DataFilePath, System.Text.Encoding.UTF8))
                {
                    container = (DataContainer)serializer.Deserialize(reader);
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Failed to load data. File may be corrupted. Current extents preserved.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unexpected error loading data.", ex);
            }

            if (container == null)
                throw new InvalidOperationException("Deserialized container is null.");

            User.SetExtent(container.Users ?? new List<User>());
            Member.SetExtent(container.Members ?? new List<Member>());
            Administrator.SetExtent(container.Administrators ?? new List<Administrator>());
            Moderator.SetExtent(container.Moderators ?? new List<Moderator>());
            Community.SetExtent(container.Communities ?? new List<Community>());
            Post.SetExtent(container.Posts ?? new List<Post>());
        }

        public static bool BackupData(string backupPath)
        {
            if (!File.Exists(DataFilePath))
                return false;

            try
            {
                var backupDir = Path.GetDirectoryName(backupPath);
                if (!string.IsNullOrEmpty(backupDir) && !Directory.Exists(backupDir))
                {
                    Directory.CreateDirectory(backupDir);
                }

                File.Copy(DataFilePath, backupPath, overwrite: true);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
