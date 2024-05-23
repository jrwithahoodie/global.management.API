using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.User;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public IActionResult GetAllUsers(){
            try
            {
                var result = _userBll.GetAllUsers();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public IActionResult LoginUser(BusinessLogic.DTO.LoginUserDTO loginData)
        {
            try
            {
                var result = _userBll.LogInUser(loginData);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}