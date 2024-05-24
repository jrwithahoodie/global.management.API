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
            var activityTypeList = _context.ActivityTypes.ToList();

            return activityTypeList;
        }

        public Entities.Models.ActivityType NewActivityType(string newActivityTypeName)
        {
            var activityTypeList = _context.ActivityTypes.Where(at => at.ActivityTypeName == newActivityTypeName).ToList().FirstOrDefault();

            if(activityTypeList != null)
                throw new Exception("Este tipo de actividad ya existe");
            
            var NewActivityTypeObj = new Entities.Models.ActivityType();
            NewActivityTypeObj.ActivityTypeName = newActivityTypeName;

            var newActivityType = _context.ActivityTypes.Add(NewActivityTypeObj);
            _context.SaveChanges();

            return newActivityType.Entity;
        }

        public Entities.Models.ActivityType RemoveActivityType(int id)
        {
            throw new NotImplementedException();
        }
    }
}
