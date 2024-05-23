using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.DTO
{
    public class LoginUserDTO
    {
        public string UserOrMail { get; set; }
        public string Password { get; set; }
    }
}