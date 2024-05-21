using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.AppContext;

namespace BusinessLogic.ActivityType
{
    public class ActivityTypeBll : IActivityTypeBll
    {
        #region Fields
        private readonly Context _context;
        #endregion

        #region  Constructors
        public ActivityTypeBll()
        {
            _context = new Context();
        }
        #endregion
        public List<Entities.Models.ActivityType> GetAllActivityTypes()
        {
            var activityTypeList = _context.activityTypes.ToList();

            return activityTypeList;
        }

        public Entities.Models.ActivityType NewActivityType(Entities.Models.ActivityType newActivityType)
        {
            var result  = _context.activityTypes.Add(newActivityType);
            _context.SaveChanges();

            return result.Entity;
        }

        public Entities.Models.ActivityType RemoveActivityType(int id)
        {
            throw new NotImplementedException();
        }
    }
}
