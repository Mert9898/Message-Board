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
        public List<Member> Members { get; set; } = new List<Member>();
        public List<Post> Posts { get; set; } = new List<Post>();
        public List<Community> Communities { get; set; } = new List<Community>();
        public List<Administrator> Administrators { get; set; } = new List<Administrator>();
    }

    public static class PersistenceManager
{
    private const string DataFilePath = "messageboard_data.xml";
    private static XmlSerializer GetSerializer() => new XmlSerializer(typeof(DataContainer));
    public static void SaveData()
    {
        var membersSnapshot = new List<Member>(Member.Extent ?? Array.Empty<Member>());
        var adminsSnapshot = new List<Administrator>(Administrator.Extent ?? Array.Empty<Administrator>());
        var communitiesSnapshot = new List<Community>(Community.Extent ?? Array.Empty<Community>());
        var postsSnapshot = new List<Post>(Post.Extent ?? Array.Empty<Post>());

        var container = new DataContainer
        {
            Members = membersSnapshot,
            Administrators = adminsSnapshot,
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
                        if (File.Exists(tempPath))
                        {
                            try
                            {
                                File.Delete(DataFilePath);
                            }
                            catch { }
                            try
                            {
                                File.Move(tempPath, DataFilePath);
                            }
                            catch
                            {
                                throw;
                            }
                        }
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
            try { if (File.Exists(tempPath)) File.Delete(tempPath); } catch { }
            throw;
        }
    }
    public static void LoadData()
    {
        var doesExist = File.Exists(DataFilePath);
        if (!doesExist) return;

        var serializer = GetSerializer();
        DataContainer? container = null;
        try
        {
            using (var fs = new FileStream(DataFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(fs, System.Text.Encoding.UTF8))
            {
                container = serializer.Deserialize(reader) as DataContainer;
            }
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException("Failed to parse persistence file. Current in-memory data preserved.", ex);
        }
        catch (Exception ex)
        {
            throw new IOException("Error while reading persistence file. Current in-memory data preserved.", ex);
        }

        if (container == null)
        {
            throw new InvalidOperationException("Deserialized container was null.");
        }
        Member.SetExtent(new List<Member>(container.Members ?? new List<Member>()));
        Administrator.SetExtent(new List<Administrator>(container.Administrators ?? new List<Administrator>()));
        Community.SetExtent(new List<Community>(container.Communities ?? new List<Community>()));
        Post.SetExtent(new List<Post>(container.Posts ?? new List<Post>()));
    }
    public static bool BackupData(string backupPath)
    {
        try
        {
            if (!File.Exists(DataFilePath))
            {
                return false;
            }

            var dir = Path.GetDirectoryName(backupPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.Copy(DataFilePath, backupPath, overwrite: true);
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

}
