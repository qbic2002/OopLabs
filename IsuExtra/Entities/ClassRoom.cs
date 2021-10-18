using IsuExtra.Tools;

namespace IsuExtra.Entities
{
    public class ClassRoom
    {
        public ClassRoom(int number)
        {
            if (number < 100 || number > 500)
                throw new IsuExtraException("Incorrect number of classroom");
            Number = number;
        }

        public int Number { get; }
    }
}