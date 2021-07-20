using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    internal class BoardDalController : DalController
    {
        private const string BoardTableName = "Board";

        ///<summary>Constructor for the BoardDalController.</summary>
        public BoardDalController() : base(BoardTableName)
        {
        }
        

        ///<summary>This method selects all boards. 
        /// <returns> List<BoardDTO> </returns>
        public List<BoardDTO> SelectAllBoards()
        {
            List<BoardDTO> result = Select().Cast<BoardDTO>().ToList();

            return result;
        }


        protected override DTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            BoardDTO result = new BoardDTO(reader.GetString(0), reader.GetString(1), reader.GetInt32(2) , reader.GetInt32(3), this);

            return result;
        }

        ///<summary>This method inserts a board into the data. 
        /// </summary>
        ///<param name="DTOObj">the boardDTO.</param>
        ///<param name="command">The data.</param>
        protected override void MakeInsertCommand(DTO DTOObj, SQLiteCommand command)
        {
            BoardDTO boardDTO = (BoardDTO)DTOObj;
            command.CommandText = $"INSERT INTO {BoardTableName} ({boardDTO.PrimaryKeyColumnNames[0]} , {boardDTO.PrimaryKeyColumnNames[1]} ,{BoardDTO.NumOfTasksColumnName} ,{BoardDTO.NumOfColumnsColumnName}) " +
                $"VALUES (@emailCreatorVal,@boardNameVal,@numOfTasksVal,@numOfColumnsVal);";

            command.Parameters.Add(new SQLiteParameter(@"emailCreatorVal", boardDTO.EmailCreator));
            command.Parameters.Add(new SQLiteParameter(@"boardNameVal", boardDTO.BoardName));
            command.Parameters.Add(new SQLiteParameter(@"numOfTasksVal", boardDTO.NumOfTasks));
            command.Parameters.Add(new SQLiteParameter(@"numOfColumnsVal", boardDTO.NumOfTasks));
        }
    }
}
