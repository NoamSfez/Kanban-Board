using IntroSE.Kanban.Backend.DataAccessLayer.DalControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    internal class UserJoinedBoardDTO : DTO
    {
        public string JoinedUserEmail { get => PrimaryKey[2]; }
        public string EmailCreator { get => PrimaryKey[0]; }

        public string BoardName { get => PrimaryKey[1]; }

        ///<summary>Constructor for the UserJoinedBoardDTO.</summary>
        ///<param name="emailCreator">the email creator.</param>
        ///<param name="boardName">the board's name.</param>
        ///<param name="joinedUserEmail">the other user's email.</param>
        ///<param name="controller">the user join board dal controller.</param>
        public UserJoinedBoardDTO(string emailCreator, string boardName, string joinedUserEmail, UserJoinedBoardDalController controller) : base(controller)
        {
            PrimaryKeyColumnNames = new string[] { "EmailCreator", "BoardName" , "JoinedUserEmail"};
            PrimaryKey = new string[] { emailCreator, boardName, joinedUserEmail };
            persisted = true;
        }

        ///<summary>Constructor for the UserJoinedBoardDTO.</summary>
        ///<param name="emailCreator">the email creator.</param>
        ///<param name="boardName">the board's name.</param>
        ///<param name="joinedUserEmail">the other user's email.</param
        public UserJoinedBoardDTO(string emailCreator, string boardName, string joinedUserEmail) : base(new UserJoinedBoardDalController())
        {
            PrimaryKeyColumnNames = new string[] { "EmailCreator", "BoardName", "JoinedUserEmail" };
            PrimaryKey = new string[] { emailCreator, boardName, joinedUserEmail };
        }

        internal override string GetIdentity()
        {
            return $"board : {BoardName}";
        }
    }
}
