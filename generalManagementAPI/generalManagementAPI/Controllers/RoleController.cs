using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace generalManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        #region Fields
        private readonly IRoleBll _roleBll;
        #endregion

        #region Builder
        public RoleController(IRoleBll roleBll)
        {
            _roleBll = roleBll;
        }
        #endregion

        [HttpGet("GetAllRoles")]
        [Authorize]
        public IActionResult GetAllRoles(){
            try
            {
                var result = _roleBll.GetAllRoles();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("SetNewRole")]
        [Authorize]
        public IActionResult NewRole([FromQuery]string roleName = null)
        {
            try
            {
                var result = _roleBll.NewRole(roleName);

                return Ok(result);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}