using IsuExtra.Tools;

namespace IsuExtra.Entities
{
    public class JTGroupsOfStudent
    {
        public JoinTrainingGroup FirstJTG { get; private set; }
        public JoinTrainingGroup SecondJTG { get; private set; }

        public void AddJTG(JoinTrainingGroup jtgroup)
        {
            if (jtgroup is null)
                throw new IsuExtraException("Incorrect JTGroup");

            if (FirstJTG is null)
            {
                FirstJTG = jtgroup;
                return;
            }

            if (SecondJTG is null)
            {
                SecondJTG = jtgroup;
                return;
            }

            throw new IsuExtraException("Already consists two JTGroups");
        }

        public bool IsEmpty()
        {
            if (FirstJTG is null && SecondJTG is null)
                return true;
            return false;
        }
    }
}