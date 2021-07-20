using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Text.RegularExpressions;
using log4net;
using log4net.Config;
using System.IO;
using System.Reflection;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;

namespace IntroSE.Kanban.Backend.BusinessLayer
{

    /// <summary>
    /// The main User class.
    /// Represents a user in the Kanban system.
    /// </summary>
    public class User
    {
        //Currently readonly as there is no requirement to be able to update users after they are set.
        public readonly string email;
        private readonly string password;
        private UserDTO userDTO;

        

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        ///<summary>Constructor for the user.</summary>
        ///<param name="email">the user's email.</param>
        ///<param name="password">the user's password.</param>
        /// <exception cref="System.Exception">Thrown when the email is invalid.</exception>
        /// <exception cref="System.Exception">Thrown when the password is invalid.</exception>
        public User(string email, string password)
        {
            configLog();
            // Check email is a valid email address
            if (!IsValidEmail(email))
            {
                log.Error("Email is invalid.");
                throw new Exception("Email is invalid.");
            }
            // Check password is valid according to our limitations
            if (!IsValidPassword(password))
            {
                log.Error("Password is invalid");
                throw new Exception("Password is invalid");
            }

            this.email = email;
            this.password = password;

            userDTO = new UserDTO(email, password);
            userDTO.Insert();
        }

        internal User(UserDTO userDTO)
        {
            configLog();

            this.email = userDTO.Email;
            this.password = userDTO.Password;
            this.userDTO = userDTO;
        }

        public void DeletePersistence()
        {
            userDTO.Delete();
        }

        /// <summary>
        /// This method configures the logger.
        /// </summary>
        private void configLog()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        ///<summary>This method checks if a given string is a valid email address. 
        ///This version passes the automatic tests
        ///From Ruben's answer on: https://stackoverflow.com/questions/1365407/c-sharp-code-to-validate-email-address </summary>
        ///<param name="email">the string to be checked.</param>
        /// <returns> true if the string is a valid email address and false if it isn't </returns>
        private bool IsValidEmail(string email)
        {
            string expression = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";

            if (Regex.IsMatch(email, expression))
            {
                if (Regex.Replace(email, expression, string.Empty).Length == 0)
                {
                    return true;
                }
            }
            return false;
        }


        ///<summary>This method checks if a given string is a valid password. 
        /// For a password to be valid it needs to:
        /// <list type="bullet">
        /// <item>Have a length between MIN_PASS_LENGTH and MAX_PASS_LENGTH</item>
        /// <item>Contain at least one uppercase letter</item>
        /// <item>Contain at least one lowercase letter</item>
        /// <item>Contain at least one digit</item>
        /// </summary>
        ///<param name="password">the string to be checked.</param>
        /// <returns> true if the string is a valid password and false if it isn't </returns>
        private bool IsValidPassword(string password)
        {
            const int MIN_PASS_LENGTH = 4;
            const int MAX_PASS_LENGTH = 20;

            char[] UPPERCASE_LETTERS = Enumerable.Range('A', 26).Select(x => (char)x).ToArray();
            char[] LOWERCASE_LETTERS = Enumerable.Range('a', 26).Select(x => (char)x).ToArray();
            char[] DIGITS = Enumerable.Range('0', 10).Select(x => (char)x).ToArray();

            // Checks if the password is too common
            if (CommonPasswords(password))
            {
                return false;
            }

            //Check password fits length
            if (MIN_PASS_LENGTH > password.Length || password.Length > MAX_PASS_LENGTH)
            {
                return false;
            }

            //Check password contains an uppercase letter
            if (password.IndexOfAny(UPPERCASE_LETTERS) == -1)
            {
                return false;
            }

            //Check password contains a lowercase letter
            if (password.IndexOfAny(LOWERCASE_LETTERS) == -1)
            {
                return false;
            }

            //Check password contains a digit
            if (password.IndexOfAny(DIGITS) == -1)
            {
                return false;
            }

            return true;
        }

        ///<summary>This method logs the user into the system if the given password matches the user's password.</summary>
        ///<param name="password">the user's password to be checked.</param>
        /// <exception cref="System.Exception">Thrown when the given password is wrong.</exception>
        public void Login(string password)
        {
            // Check if inputted password matches user's password
            if (this.password != password)
            { 
                log.Warn("Wrong password.");
                throw new Exception("Wrong password.");
            }
        }



        /// <summary>
        /// Check if the password is too simple.
        /// </summary>
        /// <param name="password">the password of the user is to be registered with.</param>
        /// <returns> bool </returns>
        internal bool CommonPasswords(string password)
        {
            switch (password)
            {
                case "123456":
                    return true;
                case "123456789":
                    return true; ;
                case "qwerty":
                    return true;
                case "password":
                    return true;
                case "1111111":
                    return true; ;
                case "12345678":
                    return true;
                case "abc123":
                    return true;
                case "1234567":
                    return true; ;
                case "password1":
                    return true;
                case "12345":
                    return true;
                case "1234567890":
                    return true; ;
                case "123123":
                    return true;
                case "000000":
                    return true;
                case "Iloveyou":
                    return true; ;
                case "1234":
                    return true;
                case "1q2w3e4r5t":
                    return true;
                case "Qwertyuiop":
                    return true; ;
                case "123":
                    return true;
                case "Monkey":
                    return true; ;
                case "Dragon":
                    return true;
                default:
                    return false;
            }
        }
    }
}
