using System.Collections.Generic;
using Backups.Entities;
using BackupsExtra.Tools;

namespace BackupsExtra.Entities
{
    public class RemoveAlgorithmSerializer
    {
        public RemoveAlgorithmSerializer()
        {
        }

        public RemoveAlgorithmSerializer(RemoveAlgorithm removeAlgorithm)
        {
            if (removeAlgorithm is null)
                throw new BackupsExtraException("Incorrect remove algorithm");
            AllAlgorithm = removeAlgorithm.AllPredicates;
            RemovePredicateSerializers = new List<RemovePredicateSerializer>();
            removeAlgorithm.RemovePredicates.ForEach(removePredicate => RemovePredicateSerializers.Add(new RemovePredicateSerializer(removePredicate)));
        }

        public bool AllAlgorithm { get; set; }
        public List<RemovePredicateSerializer> RemovePredicateSerializers { get; set; }

        public RemoveAlgorithm ToRemoveAlgorithm()
        {
            var removePredicates = new List<IRemovePredicate>();
            RemovePredicateSerializers.ForEach(serializer => removePredicates.Add(serializer.ToRemovePredicate()));
            return new RemoveAlgorithm(AllAlgorithm, removePredicates.ToArray());
        }
    }
}