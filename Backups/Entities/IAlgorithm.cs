using System.Collections.Generic;

namespace Backups.Entities
{
    public interface IAlgorithm
    {
         List<Storage> DoStrategy(params JobObject[] jobObjects);
    }
}