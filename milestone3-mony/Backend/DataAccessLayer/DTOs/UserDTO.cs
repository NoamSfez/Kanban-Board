using System;
namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    internal class UserDTO : DTO
    {
        public const string UserPasswordColumnName = "Password";

        private string _password;
        public string Password { get => _password; set { _password = value; if (persisted) _controller.Update(this, UserPasswordColumnName, value); } }
        public string Email { get => PrimaryKey[0]; }

        ///<summary>Constructor for the UserDTO.</summary>
        ///<param name="email">the email of the user.</param>
        ///<param name="password">the password of the user.</param>
        ///<param name="controller">the user dal controller.</param>
        public UserDTO(string email, string password , UserDalController controller) :base(controller)
        {
            PrimaryKeyColumnNames = new string[] { "Email" };
            PrimaryKey = new string[]{email};
            _password = password;
            persisted = true;
        }

        ///<summary>Constructor for the UserDTO.</summary>
        ///<param name="email">the email of the user.</param>
        ///<param name="password">the password of the user.</param>
        public UserDTO(string email, string password) : base(new UserDalController())
        {
            PrimaryKeyColumnNames = new string[] {"Email" };
            PrimaryKey = new string[] { email };
            _password = password;
        }
        internal override string GetIdentity()
        {
            return $"user : {Email}";
        }
    }
}
