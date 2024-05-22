using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.User;
using Microsoft.AspNetCore.Mvc;

namespace generalManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        #region Fields
        private readonly IUserBll _userBll;
        #endregion

        #region Builder
        public UserController(IUserBll userBll)
        {
            _userBll = userBll;
        }
        #endregion

        [HttpGet("getAllUsers")]
        public IActionResult GetAllUsers(){
            try
            {
                var result = _userBll.GetAllUsers();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost("register")]
        public IActionResult RegisterNewUser(Entities.Models.User newUserData)
        {
            try
            {
                var result = _userBll.NewUser(newUserData);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}