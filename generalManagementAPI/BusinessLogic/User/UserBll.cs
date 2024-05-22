using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
            
            //Encrypt password
            newUserData.Password = encryptText(newUserData.Password);

            var newUserResult = _context.Users.Add(newUserData);
            _context.SaveChanges();

            return newUserResult.Entity;
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
        
        #endregion
    }
}