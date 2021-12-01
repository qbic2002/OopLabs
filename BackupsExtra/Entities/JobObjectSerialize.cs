using Backups.Entities;
using BackupsExtra.Tools;

namespace BackupsExtra.Entities
{
    public class JobObjectSerialize
    {
        public JobObjectSerialize(JobObject jobObject)
        {
            if (jobObject is null)
                throw new BackupsExtraException("Incorrect job object");
            Fullname = jobObject.Fullname;
        }

        public JobObjectSerialize()
        {
        }

        public string Fullname { get; set; }

        public JobObject ToObject()
        {
            return new JobObject(Fullname);
        }
    }
}