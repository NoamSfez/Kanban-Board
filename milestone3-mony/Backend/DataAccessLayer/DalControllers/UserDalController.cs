using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    internal class UserDalController : DalController
    {
        private const string UserTableName = "User";

        ///<summary>Constructor for the UserDalController.</summary>
        public UserDalController() : base(UserTableName)
        {
        }

        ///<summary>This method selects all Users. 
        /// <returns> List<UserDTO> </returns>
        public List<UserDTO> SelectAllUsers()
        {
            List<UserDTO> result = Select().Cast<UserDTO>().ToList();

            return result;
        }

        protected override DTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            UserDTO result = new UserDTO(reader.GetString(0), reader.GetString(1), this);

            return result;
        }

        ///<summary>This method insert a user into the data. 
        /// </summary>
        ///<param name="DTOObj">the userDTO.</param>
        ///<param name="command">The data.</param>
        protected override void MakeInsertCommand(DTO DTOObj, SQLiteCommand command)
        {
            UserDTO userDTO = (UserDTO)DTOObj;
            command.CommandText = $"INSERT INTO {UserTableName} ({userDTO.PrimaryKeyColumnNames[0]} ,{UserDTO.UserPasswordColumnName}) " +
                $"VALUES (@emailVal,@passwordVal);";

            command.Parameters.Add(new SQLiteParameter(@"emailVal", userDTO.Email));
            command.Parameters.Add(new SQLiteParameter(@"passwordVal", userDTO.Password));
        }

    }
}
