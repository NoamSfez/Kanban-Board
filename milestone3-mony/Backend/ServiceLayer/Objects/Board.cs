using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public struct Board
    {
        public readonly string CreatorEmail;
        public readonly string BoardName;
        public readonly int NumOfTasks;
        public readonly List<int> Columns;
        public readonly List<string> JoinedUsers;

        public Board(string creatorEmail, string boardName, int numOfTasks, List<int> columns, List<string> joinedUsers)
        {
            CreatorEmail = creatorEmail;
            BoardName = boardName;
            NumOfTasks = numOfTasks;
            Columns = columns;
            JoinedUsers = joinedUsers;
        }


        internal Board(BusinessLayer.Board board)
        {
            CreatorEmail = board.CreatorEmail;
            BoardName = board.BoardName;
            NumOfTasks = board.NumOfTasks;
            Columns = board.ColumnOrdinals;
            JoinedUsers = board.JoinedUsersEmails;
        }
    }
}
