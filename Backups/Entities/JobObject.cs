using System.IO;
using Backups.Tools;

namespace Backups.Entities
{
    public class JobObject
    {
        public JobObject(string filename)
        {
            Fullname = filename ?? throw new BackupException("empty filename");
            /*if (!File.Exists(Fullname))
                throw new BackupException("File does not exists");*/
            NameWithoutExtension = Path.GetFileNameWithoutExtension(Fullname);
            Name = Path.GetFileName(Fullname);
        }

        public string Fullname { get; }
        public string NameWithoutExtension { get; }
        public string Name { get; }
        public override bool Equals(object obj)
        {
            return obj is JobObject jobObject && Fullname == jobObject.Fullname;
        }

        public override int GetHashCode()
        {
            return Fullname != null ? Fullname.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return Fullname;
        }
    }
}