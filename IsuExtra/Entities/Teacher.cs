using IsuExtra.Tools;

namespace IsuExtra.Entities
{
    public class Teacher
    {
        public Teacher(string name, int id)
        {
            Name = name ?? throw new IsuExtraException("Incorrect name");
            if (id < 0)
                throw new IsuExtraException("Incorrect id");
        }

        public int Id { get; }
        public string Name { get; }
    }
}