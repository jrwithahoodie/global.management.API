using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.ActivityType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace generalManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityTypeController : ControllerBase
    {
        #region Fields
        private readonly IActivityTypeBll _activityTypeBll;
        #endregion

        #region Constructors
        public ActivityTypeController(IActivityTypeBll activityTypeBll)
        {
            _activityTypeBll = activityTypeBll;
        }
        #endregion

        [HttpGet("GetAllActivityType")]
        [Authorize]
        public IActionResult GetAllActivityType()
        {
            try
            {
                var result = _activityTypeBll.GetAllActivityTypes();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        [HttpPost("register/activitytype")]
        [Authorize]
        public IActionResult NewActivityType(Entities.Models.ActivityType newActivityType)
        {
            try
            {
                var result = _activityTypeBll.NewActivityType(newActivityType);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode (500, ex.Message);
            }
        }
    }
}