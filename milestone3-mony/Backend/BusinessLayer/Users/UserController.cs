using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using System.IO;
using System.Reflection;
using IntroSE.Kanban.Backend.DataAccessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using System.Runtime.CompilerServices;


namespace IntroSE.Kanban.Backend.BusinessLayer
{

    /// <summary>
    /// This controller class keeps track of users registered in the system.
    /// </summary>
    public class UserController 
    {

        private Dictionary<string, User> users;

        //Internal for testing
        internal Dictionary<string, User> Users
        {
            get => users;
            set
            {
                users = value;
            }
        }

        public virtual User LoggedInUser { get; private set; }

        private UserDalController userDalController;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        /// <summary>
        /// Standard constructor
        /// </summary>
        public UserController()
        {
            users = new Dictionary<string, User>();
            userDalController = new();
            configLog();
        }

        /// <summary>
        /// This method configures the logger.
        /// </summary>
        private void configLog()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        /// <summary>
        /// Gets the user identified by the given email
        /// </summary>
        /// <param name="email">the email of the user to retrieve.</param>
        /// <returns>the user identified by the given email</returns>
        /// <exception cref="System.Exception">Thrown when the email does not match any registered users.</exception>
        public virtual User GetUser(string email)
        {
            User user;
            if (users.TryGetValue(email, out user))
            {
                return user;
            }
            else
            {
                log.Error("User does not exist.");
                throw new Exception("User does not exist.");
            }
        }

        /// <summary>
        /// Register a user
        /// </summary>
        /// <param name="email">the email of the user to be registered with.</param>
        /// <param name="password">the password of the user to be registered with.</param>
        public void Register(string email, string password)
        {
            if (users.ContainsKey(email))
            {
                log.Error($"Tried to register {email} when it was already registered.");
                throw new Exception($"The email {email} is already registered.");
            }

            users.Add(email, new User(email, password));
        }

        /// <summary>
        /// Login a user
        /// </summary>
        /// <param name="email">the email of the user to be logged in.</param>
        /// <param name="password">the password of the user to be logged in.</param>
        public User Login(string email, string password)
        {
            // Check if a user is already logged in
            if (LoggedInUser != null)
            {
                log.Warn("A user is already logged in.");
                throw new Exception("User is already logged in.");
            }

            User user = GetUser(email);
            user.Login(password);
            LoggedInUser = user;
            return user;
        }



        ///<summary>This method logs the user out of the system.</summary>
        ////// <exception cref="System.Exception">Thrown when the user is already logged out.</exception>
        public void Logout(string email)
        {
            // Check if user is currently logged in
            if (LoggedInUser == null || LoggedInUser.email != email)
            {
                log.Warn("User is already logged out.");
                throw new Exception("User is already logged out.");
            }
            else
            {
                LoggedInUser = null;
            }
        }

        ///<summary>This function loads all users from the data.
        public void LoadAllUsers()
        {
            List<UserDTO> userDTOs = userDalController.SelectAllUsers();
            foreach (UserDTO userDTO in userDTOs)
            {
                users.Add(userDTO.Email, new User(userDTO));
            }
        }

        ///<summary>This function deletes all users from the data.
        public void DeleteAllUsers()
        {
            foreach (User user in users.Values)
            {
                user.DeletePersistence();
            }
            users.Clear();
        }
        


    }
}
