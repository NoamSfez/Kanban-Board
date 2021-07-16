using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    class UserService
    {
        private UserController userController;

        public UserService(UserController userController)
        {
            this.userController = userController;
        }

        ///<summary>This method registers a new user to the system.</summary>
        ///<param name="email">the user e-mail address, used as the username for logging the system.</param>
        ///<param name="password">the user password.</param>
        ///<returns cref="Response">The response of the action</returns>
        public Response Register(string email, string password)
        {
            try
            {
                userController.Register(email, password);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Log in an existing user
        /// </summary>
        /// <param name="email">The email address of the user to login</param>
        /// <param name="password">The password of the user to login</param>
        /// <returns>A response object with a value set to the user, instead the response should contain a error message in case of an error</returns>
        public Response<User> Login(string email, string password)
        {
            try
            {
                userController.Login(email, password);
                return Response<User>.FromValue(new User(email));
            }
            catch (Exception e)
            {
                return Response<User>.FromError(e.Message);
            }
        }

        /// <summary>        
        /// Log out an logged in user. 
        /// </summary>
        /// <param name="email">The email of the user to log out</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response Logout(string email)
        {
            try
            {
                userController.Logout(email);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        ///<summary>This method loads all users from persistent storage to the runtime.</summary>
        ///<returns cref="Response">The response of the action</returns>
        public Response LoadAllUsers()
        {
            try
            {
                userController.LoadAllUsers();
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        ///<summary>This method deletes all users from persistent storage and runtime.</summary>
        ///<returns cref="Response">The response of the action</returns>
        public Response DeleteAllUsers()
        {
            try
            {
                userController.DeleteAllUsers();
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }
    }
}
