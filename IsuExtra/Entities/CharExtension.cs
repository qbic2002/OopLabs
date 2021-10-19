namespace IsuExtra.Entities
{
    public static class CharExtension
    {
        public static Faculty? GetFaculty(this char literal)
        {
            switch (literal)
            {
                case 'B':
                    return Faculty.PT;
                case 'D':
                    return Faculty.IDP;
                case 'K':
                    return Faculty.TIT;
                case 'L':
                    return Faculty.PT;
                case 'M':
                    return Faculty.TIT;
                case 'N':
                    return Faculty.CTM;
                case 'P':
                    return Faculty.CTM;
                case 'R':
                    return Faculty.CTM;
                case 'T':
                    return Faculty.LS;
                case 'U':
                    return Faculty.TMI;
                case 'V':
                    return Faculty.PT;
                case 'W':
                    return Faculty.BLTS;
                case 'Z':
                    return Faculty.PT;
            }

            return null;
        }
    }
}