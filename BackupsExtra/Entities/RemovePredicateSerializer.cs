using System;
using System.Net.Http.Headers;
using BackupsExtra.Tools;

namespace BackupsExtra.Entities
{
    public class RemovePredicateSerializer
    {
        public RemovePredicateSerializer()
        {
        }

        public RemovePredicateSerializer(IRemovePredicate removePredicate)
        {
            if (removePredicate is null)
                throw new BackupsExtraException("Incorrect remove predicate");
            Param = removePredicate.Param;
            Type = removePredicate.Type();
        }

        public string Type { get; set; }
        public object Param { get; set; }

        public IRemovePredicate ToRemovePredicate()
        {
            IRemovePredicate removePredicate = null;
            switch (Type)
            {
                case "Count":
                    removePredicate = new CountPredicate(int.Parse(Param.ToString()));
                    break;
                case "Date":
                    removePredicate = new DatePredicate(DateTime.Parse(Param.ToString()));
                    break;
            }

            return removePredicate;
        }
    }
}