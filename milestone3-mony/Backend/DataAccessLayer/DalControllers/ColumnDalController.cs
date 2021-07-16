using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DalControllers
{
    internal class ColumnDalController : DalController
    {
        private const string ColumnTableName = "Column";

        ///<summary>Constructor for the ColumnDalController.</summary>
        public ColumnDalController() : base(ColumnTableName)
        {
        }

        ///<summary>This method select all columns. 
        /// </summary>
        ///<param name="boardDTO">The board from which all the columns are taken .</param>
        /// <returns> List<ColumnDTO> </returns>
        public List<ColumnDTO> SelectAllColumns(BoardDTO boardDTO)
        {
            List<ColumnDTO> result = Select(boardDTO).Cast<ColumnDTO>().ToList();

            return result;
        }

        protected override DTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            ColumnDTO result = new ColumnDTO(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetString(3), reader.GetString(4) , reader.GetInt32(5), this);

            return result;
        }

        ///<summary>This method inserts a column into the data. 
        /// </summary>
        ///<param name="DTOObj">the columnDTO.</param>
        ///<param name="command">The data.</param>
        protected override void MakeInsertCommand(DTO DTOObj, SQLiteCommand command)
        {
            ColumnDTO columnDTO = (ColumnDTO)DTOObj;
            command.CommandText = $"INSERT INTO {ColumnTableName} ({columnDTO.PrimaryKeyColumnNames[0]} , {ColumnDTO.ColumnOrdinalColumnName} , {columnDTO.PrimaryKeyColumnNames[1]} , {columnDTO.PrimaryKeyColumnNames[2]}, {ColumnDTO.NameColumnName} , {ColumnDTO.TaskLimitColumnName}) " +
                $"VALUES (@columnIdVal,@columnOrdinalVal,@emailCreatorVal,@boardNameVal,@nameVal,@taskLimitVal);";

            command.Parameters.Add(new SQLiteParameter(@"columnIdVal", columnDTO.ColumnId));
            command.Parameters.Add(new SQLiteParameter(@"columnOrdinalVal", columnDTO.ColumnOrdinal));
            command.Parameters.Add(new SQLiteParameter(@"emailCreatorVal", columnDTO.EmailCreator));
            command.Parameters.Add(new SQLiteParameter(@"boardNameVal", columnDTO.BoardName));
            command.Parameters.Add(new SQLiteParameter(@"nameVal", columnDTO.Name));
            command.Parameters.Add(new SQLiteParameter(@"taskLimitVal", columnDTO.TaskLimit));
        }
    }
}
