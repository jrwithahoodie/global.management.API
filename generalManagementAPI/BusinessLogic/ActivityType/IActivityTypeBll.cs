using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;

namespace BusinessLogic.ActivityType
{
    public interface IActivityTypeBll
    {
        Entities.Models.ActivityType NewActivityType(Entities.Models.ActivityType newActivityType);
        List<Entities.Models.ActivityType> GetAllActivityTypes();
        Entities.Models.ActivityType RemoveActivityType(int id);
    }
}