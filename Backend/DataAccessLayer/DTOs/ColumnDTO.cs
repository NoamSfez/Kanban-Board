using IntroSE.Kanban.Backend.DataAccessLayer.DalControllers;
using System;
namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    internal class ColumnDTO : DTO
    {

        public const string ColumnOrdinalColumnName = "ColumnOrdinal";
        public const string NameColumnName = "Name";
        public const string TaskLimitColumnName = "TaskLimit";


        private int _columnOrdinal;
        public int ColumnOrdinal { get => _columnOrdinal; set { _columnOrdinal = value; if (persisted) _controller.Update(this, ColumnOrdinalColumnName, value); } }
        private string _name;
        public string Name { get => _name; set { _name = value; if(persisted) _controller.Update(this, NameColumnName, value); } }
        private int _taskLimit;
        public int TaskLimit { get => _taskLimit; set { _taskLimit = value; if (persisted) _controller.Update(this, TaskLimitColumnName, value); } }
       
        public int ColumnId { get => Int32.Parse(PrimaryKey[0]);  }
        public string EmailCreator { get => PrimaryKey[1]; }
        public string BoardName { get => PrimaryKey[2]; }

        ///<summary>Constructor for the ColumnDTO.</summary>
        ///<param name="columnOrdinal">the ordinal of the column.</param>
        ///<param name="emailCreator">the email's creator.</param>
        ///<param name="boardName">the board's name.</param>
        ///<param name="name">the name of the column.</param>
        ///<param name="taskLimit">the task limit of the column.</param>
        ///<param name="controller">the column dal controller.</param>
        public ColumnDTO(int columnId, int columnOrdinal, string emailCreator, string boardName, string name, int taskLimit, ColumnDalController controller) : base(controller)
        {
            PrimaryKeyColumnNames = new string[] { "ColumnId" ,  "EmailCreator", "BoardName" };
            PrimaryKey = new string[] { "" + columnId, emailCreator, boardName };
            _columnOrdinal = columnOrdinal;
            _name = name;
            _taskLimit = taskLimit;
            persisted = true;
        }

        ///<summary>Constructor for the ColumnDTO.</summary>
        ///<param name="columnOrdinal">the ordinal of the column.</param>
        ///<param name="emailCreator">the email's creator.</param>
        ///<param name="boardName">the board's name.</param>
        ///<param name="name">the name of the column.</param>
        ///<param name="taskLimit">the task limit of the column.</param>
        public ColumnDTO(int columnId,  int columnOrdinal, string emailCreator, string boardName, string name, int taskLimit) : base(new ColumnDalController())
        {
            PrimaryKeyColumnNames = new string[] { "ColumnId", "EmailCreator", "BoardName" };
            PrimaryKey = new string[] { "" + columnId, emailCreator, boardName };
            _columnOrdinal = columnOrdinal;
            _name = name;
            _taskLimit = taskLimit;
        }

        ///<summary>Constructor for the ColumnDTO.</summary>
        ///<param name="columnOrdinal">the ordinal of the column.</param>
        ///<param name="name">the name of the column.</param>
        ///<param name="taskLimit">the task limit of the column.</param>
        ///<param name="controller">the column dal controller.</param>
        public ColumnDTO(int columnId, int columnOrdinal, string name, int taskLimit, ColumnDalController controller) : base(controller)
        {
            PrimaryKeyColumnNames = new string[] { "ColumnId", "EmailCreator", "BoardName" };
            PrimaryKey = new string[] { "" + columnId, null, null };
            _columnOrdinal = columnOrdinal;
            _name = name;
            _taskLimit = taskLimit;
            persisted = true;
        }

        ///<summary>Constructor for the ColumnDTO.</summary>
        ///<param name="columnOrdinal">the ordinal of the column.</param>
        ///<param name="name">the name of the column.</param>
        ///<param name="taskLimit">the task limit of the column.</param>
        public ColumnDTO(int columnId, int columnOrdinal, string name, int taskLimit) : base(new ColumnDalController())
        {
            PrimaryKeyColumnNames = new string[] { "ColumnId", "EmailCreator", "BoardName" };
            PrimaryKey = new string[] { "" + columnId, null, null };
            _columnOrdinal = columnOrdinal;
            _name = name;
            _taskLimit = taskLimit;
        }

        ///<summary>This method inserts a task into the data. 
        /// </summary>
        ///<param name="taskDTO">The taskDTO that is inserted into the data.</param>
        public void AddTask(TaskDTO taskDTO)
        {
            taskDTO.PrimaryKey = new string[] { EmailCreator, BoardName, "" + ColumnOrdinal, "" + taskDTO.Id };
            taskDTO.Insert();
        }
        internal override string GetIdentity()
        {
            return $"column : {Name}";
        }


    }
}
