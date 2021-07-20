using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DalControllers
{
    class UserJoinedBoardDalController : DalController
    {
        private const string UserJoinedBoardTableName = "UserJoinedBoard";

        ///<summary>Constructor for the UserJoinedBoardDalController.</summary>
        public UserJoinedBoardDalController() : base(UserJoinedBoardTableName)
        {
        }

        ///<summary>This method selects all users from the same board.
        /// </summary>
        ///<param name="boardDTO">The board that all users can use.</param
        /// <returns> List<UserJoinedBoardDTO> </returns>
        public List<UserJoinedBoardDTO> SelectAllFromBoard(BoardDTO boardDTO)
        {
            List<UserJoinedBoardDTO> result = Select(boardDTO).Cast<UserJoinedBoardDTO>().ToList();

            return result;
        }

        protected override DTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            UserJoinedBoardDTO result = new UserJoinedBoardDTO(reader.GetString(0), reader.GetString(1), reader.GetString(2), this);

            return result;
        }

        ///<summary>This method inserts a user to UserJoinedBoard in the data. 
        /// </summary>
        ///<param name="DTOObj">the UserJoinedBoardDTO.</param>
        ///<param name="command">The data.</param>
        protected override void MakeInsertCommand(DTO DTOObj, SQLiteCommand command)
        {
            UserJoinedBoardDTO userJoinedBoardDTO = (UserJoinedBoardDTO)DTOObj;
            command.CommandText = $"INSERT INTO {UserJoinedBoardTableName} ({userJoinedBoardDTO.PrimaryKeyColumnNames[0]} , {userJoinedBoardDTO.PrimaryKeyColumnNames[1]} ,{userJoinedBoardDTO.PrimaryKeyColumnNames[2]}) " +
                $"VALUES (@emailCreatorVal,@boardNameVal,@joinedUserEmailVal);";

            command.Parameters.Add(new SQLiteParameter(@"emailCreatorVal", userJoinedBoardDTO.EmailCreator));
            command.Parameters.Add(new SQLiteParameter(@"boardNameVal", userJoinedBoardDTO.BoardName));
            command.Parameters.Add(new SQLiteParameter(@"joinedUserEmailVal", userJoinedBoardDTO.JoinedUserEmail));
        }
    }
}
