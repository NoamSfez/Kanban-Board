using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DalControllers
{
    internal class TaskDalController : DalController
    {
        private const string TaskName = "Task";

        ///<summary>Constructor for the TaskDalController.</summary>
        public TaskDalController() : base(TaskName)
        {
        }

        ///<summary>This method selects all tasks. 
        /// </summary>
        ///<param name="columnDTO">The column from which all the tasks are taken .</param>
        /// <returns> List<TaskDTO> </returns>
        public List<TaskDTO> SelectAllTasks(ColumnDTO columnDTO)
        {
            List<TaskDTO> result = Select(columnDTO).Cast<TaskDTO>().ToList();

            return result;
        }

        ///<summary>This method selects all DTOObj - modified to choose by columnordinal instead of columnid
        /// </summary>
        ///<param name="DTOObj">The DTO From which the information is taken.</param>
        /// <returns> List<DTO> </returns>
        protected List<DTO> Select(ColumnDTO columnDTO)
        {
            List<DTO> results = new List<DTO>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                string selectString = ColumnDTO.ColumnOrdinalColumnName + "=@" + ColumnDTO.ColumnOrdinalColumnName;
                for (int i = 1; i < columnDTO.PrimaryKeyColumnNames.Length; i++)
                {
                    selectString = selectString + " AND " + columnDTO.PrimaryKeyColumnNames[i] + "=@" + columnDTO.PrimaryKeyColumnNames[i];
                }
                var command = new SQLiteCommand
                {

                    Connection = connection,
                    CommandText = $"select * from {_tableName} where {selectString}"
                };
                SQLiteDataReader dataReader = null;
                try
                {
                    command.Parameters.Add(new SQLiteParameter(ColumnDTO.ColumnOrdinalColumnName, columnDTO.ColumnOrdinal));
                    for (int i = 1; i < columnDTO.PrimaryKeyColumnNames.Length; i++)
                    {
                        command.Parameters.Add(new SQLiteParameter(columnDTO.PrimaryKeyColumnNames[i], columnDTO.PrimaryKey[i]));
                    }
                    connection.Open();
                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        results.Add(ConvertReaderToObject(dataReader));

                    }
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            return results;
        }

        protected override DTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            TaskDTO result = new TaskDTO(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetString(3),reader.GetDateTime(4),reader.GetString(5), reader.GetString(6), reader.GetString(7),reader.GetDateTime(8),this);

            return result;
        }

        ///<summary>This method inserts a task into the data. 
        /// </summary>
        ///<param name="DTOObj">the taskDTO.</param>
        ///<param name="command">The data.</param>
        protected override void MakeInsertCommand(DTO DTOObj, SQLiteCommand command)
        {
            TaskDTO taskDTO = (TaskDTO)DTOObj;
            command.CommandText = $"INSERT INTO {TaskName} ({taskDTO.PrimaryKeyColumnNames[0]},{taskDTO.PrimaryKeyColumnNames[1]},{taskDTO.PrimaryKeyColumnNames[2]},{taskDTO.PrimaryKeyColumnNames[3]}," +
                $"{TaskDTO.TaskCreationTime},{TaskDTO.TaskTitle},{TaskDTO.TaskDescription},{TaskDTO.TaskEmailAssignee},{TaskDTO.TaskDueDate}) "+
                $"VALUES (@emailCreatorVal,@boardNameVal,@columnOrdinalVal,@idVal,@creationTimeVal,@titleVal,@descriptionVal,@emailAssigneeVal,@dueDate);";

            command.Parameters.Add(new SQLiteParameter(@"emailCreatorVal", taskDTO.EmailCreator));
            command.Parameters.Add(new SQLiteParameter(@"boardNameVal", taskDTO.BoardName));
            command.Parameters.Add(new SQLiteParameter(@"columnOrdinalVal", taskDTO.ColumnOrdinal));
            command.Parameters.Add(new SQLiteParameter(@"idVal", taskDTO.Id));
            command.Parameters.Add(new SQLiteParameter(@"descriptionVal", taskDTO.Description));
            command.Parameters.Add(new SQLiteParameter(@"titleVal", taskDTO.Title));
            command.Parameters.Add(new SQLiteParameter(@"emailAssigneeVal", taskDTO.EmailAssignee));
            command.Parameters.Add(new SQLiteParameter(@"creationTimeVal", taskDTO.CreationTime));
            command.Parameters.Add(new SQLiteParameter(@"dueDate", taskDTO.DueDate));



        }
    }
}
