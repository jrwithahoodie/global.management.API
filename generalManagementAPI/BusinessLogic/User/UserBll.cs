using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BusinessLogic.UserActivity;
using Entities.AppContext;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLogic.User
{
    public class UserBll : IUserBll
    {
        #region Fields
        private readonly Context _context;
        private readonly UserActivityBll _userActivityBll;
        private readonly IConfiguration _configuration;
        #endregion

        #region Constructors
        public UserBll(IConfiguration configuration)
        {
            _context = new Context();
            _userActivityBll = new UserActivityBll();
            _configuration = configuration;
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

        public DTO.LogInResponseDTO LogInUser(DTO.LoginUserDTO loginData)
        {
            var response = new DTO.LogInResponseDTO();
            var user = new Entities.Models.User();

            //Initializing the activity object for the db
            var newUserActivity = new Entities.Models.UsersActivity();
            newUserActivity.StartDate = DateTime.Now;
            newUserActivity.ActivityTypeId = 1;

            //Check if it is using an username or an email
            if(loginData.UserOrMail.Contains("@"))
            {
                //Get the user by mail
                user = _context.Users.Where(u => u.Email == loginData.UserOrMail).ToList().FirstOrDefault();
            }
            else
            {
                //Get the user by username
                user = _context.Users.Where(u => u.UserName.ToUpper() == loginData.UserOrMail.ToUpper()).ToList().FirstOrDefault();
            }

            //Check if there is any user with this email
            if(user == null)
                throw new Exception("No existe ningún usuario con este correo");
                
            //Check if the user is verified
            // if(!user.IsEmailConfirmed)
            //     throw new Exception("Verifica el correo del usuario antes de iniciar sesión");

            //Check the password
            var auxPassword = decryptText(user.Password);

            if(!loginData.Password.Equals(auxPassword))
                throw new Exception("La contraseña es incorrecta");

            //Assign values to the response
            response.Username = user.UserName;
            response.Email = user.Email;
            response.PhoneNumber = user.PhoneNumber;
            response.Name = user.Name;
            response.Surname = user.Surname;
            response.Token = GenerateToken(user.UserId.ToString(), "user").Result;

            //End the activity
            newUserActivity.EndDate = DateTime.UtcNow;
            newUserActivity.UserId = user.UserId;
            newUserActivity.Timestamp = ((int)(newUserActivity.EndDate - newUserActivity.StartDate).TotalSeconds);
            _userActivityBll.NewUserActivity(newUserActivity);

            return response;
        }

        public Entities.Models.User NewUser(Entities.Models.User newUserData)
        {
            //Add new user activity
            var newUserActivity = new Entities.Models.UsersActivity();
            newUserActivity.StartDate = DateTime.Now;
            newUserActivity.ActivityTypeId = 3;

            #region Check data
            var userList = _context.Users.ToList();

            //Check if the mail has a valid format
            if(!checkEmailFormat(newUserData.Email))
                throw new Exception("El formato del correo no es el correcto");

            //Check if the mail already exists
            var auxUserMail = userList.Where(u => u.Email == newUserData.Email).ToList().FirstOrDefault();

            if (auxUserMail != null)
                throw new Exception("Este correo electrónico ya existe");

            //Check if the phone number already exists
            var auxUserPhoneNumber = userList.Where(u => u.PhoneNumber == newUserData.PhoneNumber).ToList().FirstOrDefault();

            if(auxUserPhoneNumber != null)
                throw new Exception("El número de teléfono ya existe");

            //Check if the username already exists
            var auxUserUserName = userList.Where(u => u.UserName == newUserData.UserName).ToList().FirstOrDefault();

            if (auxUserUserName != null)
                throw new Exception("Este nombre de usuario ya existe");

            //Check if the date of birth is valid
            if(newUserData.DateOfBirth > DateTime.UtcNow)
                throw new Exception("La fecha de cumpleaños no puede ser futura");
            
            //Check if the password is valid
            if(!checkPasswordFormat(newUserData.Password))
                throw new Exception("La contraseña no tiene el formato correcto");
            
            #endregion
            
            newUserData.DateOfRegister = DateTime.UtcNow;
            newUserData.IsEmailConfirmed = false;
            newUserData.RoleId = 1;

            //Encrypt password
            newUserData.Password = encryptText(newUserData.Password);

            var newUserResult = _context.Users.Add(newUserData);
            _context.SaveChanges();

            newUserActivity.EndDate = DateTime.UtcNow;
            newUserActivity.UserId = newUserResult.Entity.UserId;
            newUserActivity.Timestamp = 0;
            _userActivityBll.NewUserActivity(newUserActivity);

            return newUserResult.Entity;
        }

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtConfig:Secret"]);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters 
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JwtConfig:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = _configuration["JwtConfig:Audience"],

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero

                }, out SecurityToken validatedToken);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #region Private Methods
        private bool checkPasswordFormat(string password)
        {
            //Check lower/high case letters
            Regex letters = new Regex(@"[a-zA-z]");

            //Check numbers
            Regex numbers = new Regex(@"[0-9]");

            //Check characters
            Regex  characters = new Regex("[!\"#\\$%&'()*+,-./:;=?@\\[\\]^_`{|}~]");

            var isValid = false;

            if (letters.IsMatch(password) && numbers.IsMatch(password) && characters.IsMatch(password))
                isValid = true;

            return isValid;
        }
        
        private bool checkEmailFormat(string email)
        {
            string mailPattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            
            return Regex.IsMatch(email, mailPattern);
        }

        private string encryptText(string textToEncrypt)
        {
            try
            {
                var encryptPassword = "ContrasenaEncriptada";
                var RGBSaltValue = "ContrasenaEncriptada";
                var encryptAlgHash = "MD5";
                var initialVect = "1234567891234567";
                var iterations = 22;
                int keySize = 128;


                byte[] initialVectBytes = Encoding.ASCII.GetBytes(initialVect);
                byte[] RGBSaltValueBytes = Encoding.ASCII.GetBytes(RGBSaltValue);
                byte[] textToEncryptBytes = Encoding.ASCII.GetBytes(textToEncrypt);

                PasswordDeriveBytes password = new PasswordDeriveBytes(encryptPassword, RGBSaltValueBytes, encryptAlgHash, iterations);

                byte[] keyBytes = password.GetBytes(keySize / 8);

                RijndaelManaged symmetricKey = new RijndaelManaged();

                symmetricKey.Mode = CipherMode.CBC;

                ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initialVectBytes);

                MemoryStream memoryStream = new MemoryStream();

                CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

                cryptoStream.Write(textToEncryptBytes, 0, textToEncryptBytes.Length);

                cryptoStream.FlushFinalBlock();

                byte[] encryptedTextBytes =  memoryStream.ToArray();

                memoryStream.Close();
                cryptoStream.Close();

                string encryptedText = Convert.ToBase64String(encryptedTextBytes);

                return encryptedText;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string decryptText(string encryptedText)
        {
            try
            {
                var encryptPassword = "ContrasenaEncriptada";
                var RGBSaltValue = "ContrasenaEncriptada";
                var encryptAlgHash = "MD5";
                var initialVect = "1234567891234567";
                var iterations = 22;
                int keySize = 128;

                byte[] initialVectBytes = Encoding.ASCII.GetBytes(initialVect);
                byte[] RGBSaltvalueBytes = Encoding.ASCII.GetBytes(RGBSaltValue);

                byte[] encryptedTextBytes = Convert.FromBase64String(encryptedText);

                PasswordDeriveBytes password = new PasswordDeriveBytes(encryptPassword, RGBSaltvalueBytes, encryptAlgHash, iterations);

                byte[] keyBytes = password.GetBytes(keySize / 8);

                RijndaelManaged symmetricKey = new RijndaelManaged();

                symmetricKey.Mode = CipherMode.CBC;

                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initialVectBytes);

                MemoryStream memoryStream = new MemoryStream(encryptedTextBytes);

                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

                byte[] decryptedTextBytes = new byte[encryptedTextBytes.Length];

                int decrypedByteCount = cryptoStream.Read(decryptedTextBytes, 0, decryptedTextBytes.Length);

                memoryStream.Close();
                cryptoStream.Close();

                string decryptedText = Encoding.UTF8.GetString(decryptedTextBytes, 0, decrypedByteCount);

                return decryptedText;
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        private async Task<string> GenerateToken(string userId, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),                                 //Sujeto
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),              //Id del token
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),             //Fecha de emisión
                new Claim(JwtRegisteredClaimNames.Iss, _configuration["JwtConfig:Issuer"]),     //Emisor
                new Claim(JwtRegisteredClaimNames.Aud, _configuration["JwtConfig:Audience"]),   //AudienceJwtConfig
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                    issuer: _configuration["JwtConfig:Issuer"],
                    audience: _configuration["JwtConfig:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion
    }
}