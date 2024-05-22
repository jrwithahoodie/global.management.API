using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Entities.AppContext;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLogic.User
{
    public class UserBll : IUserBll
    {
        #region Fields
        private readonly Context _context;
        #endregion

        #region Constructors
        public UserBll()
        {
            _context = new Context();
        }
        #endregion
        public Entities.Models.User DisableUser(string userName)
        {
            throw new NotImplementedException();
        }

        public List<Entities.Models.User> GetAllUsers()
        {
            var userList = _context.Users.ToList();

            return userList;
        }

        public Entities.Models.User LogInUser()
        {
            throw new NotImplementedException();
        }

        public Entities.Models.User NewUser(Entities.Models.User newUserData)
        {
            #region Check data
            
            //Check if there is any user with that username
            var userList = _context.Users.ToList();
            var auxUser = userList.Where(u => u.UserName == newUserData.UserName).FirstOrDefault();

            if (auxUser != null)
                throw new Exception("Este nombre de usuario ya existe");

            //Check if the date of birth is valid
            if(newUserData.DateOfBirth > DateTime.UtcNow)
                throw new Exception("La fecha de cumpleaños no puede ser futura");
            
            //Check if the password is valid
            if(!checkPasswordFormat(newUserData.Password))
                throw new Exception("La contraseña no tiene el formato correcto");
            
            #endregion
            var newUserResult = _context.Users.Add(newUserData);
            _context.SaveChanges();

            return newUserResult.Entity;
        }

        #region Private Methods
        private Boolean checkPasswordFormat(string password)
        {
            //Check lower/high case letters
            Regex letters = new Regex(@"[a-zA-z]");

            //Check numbers
            Regex numbers = new Regex(@"[0-9]");

            //Check characters
            Regex  characters = new Regex("[!\"#\\$%&'()*+,-./:;=?@\\[\\]^_`{|}~]");

            Boolean isValid = false;

            if (letters.IsMatch(password) && numbers.IsMatch(password) && characters.IsMatch(password))
                isValid = true;

            return isValid;
        }
        #endregion
    }
}