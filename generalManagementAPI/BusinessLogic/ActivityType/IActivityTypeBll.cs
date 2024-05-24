using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;

namespace BusinessLogic.ActivityType
{
    public interface IActivityTypeBll
    {
        List<Entities.Models.ActivityType> GetAllActivityTypes();
        Entities.Models.ActivityType NewActivityType(string newActivityTypeName);
        //Entities.Models.ActivityType RemoveActivityType(int id);
    }
}