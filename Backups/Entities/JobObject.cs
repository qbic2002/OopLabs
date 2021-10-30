using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Backups.Tools;

namespace Backups.Entities
{
    public class JobObject
    {
        public JobObject(string filename)
        {
            Fullname = filename ?? throw new BackupException("empty filename");
            if (!File.Exists(Fullname))
                throw new BackupException("File does not exists");
            NameWithoutExtension = Path.GetFileNameWithoutExtension(Fullname);
            Name = Path.GetFileName(Fullname);
        }

        public string Fullname { get; }
        public string NameWithoutExtension { get; }
        public string Name { get; }
    }
}