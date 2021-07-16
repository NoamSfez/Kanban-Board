using System;
using IntroSE.Kanban.Backend.DataAccessLayer.DalControllers;
namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    internal class TaskDTO : DTO
    {


        public const string TaskDescription = "Description";
        public const string TaskTitle = "Title";
        public const string TaskEmailAssignee = "EmailAssignee";
        public const string TaskDueDate = "DueDate";
        public const string TaskCreationTime = "CreationTime";


        public string EmailCreator { get => PrimaryKey[0]; }
        public string BoardName { get => PrimaryKey[1]; }
        public int ColumnOrdinal { get => Int32.Parse(PrimaryKey[2]); }
        public int Id { get => Int32.Parse(PrimaryKey[3]); }

        private string _description;
        public string Description { get => _description; set { _description = value; if (persisted) _controller.Update(this, TaskDescription, value); } }

        private string _title;
        public string Title { get => _title; set { _title = value; if (persisted) _controller.Update(this, TaskTitle, value); } }

        private string _emailAssignee;
        public string EmailAssignee { get => _emailAssignee; set { if (persisted) _emailAssignee = value;_controller.Update(this, TaskEmailAssignee, value); } }

        private DateTime _dueDate;
        public DateTime DueDate { get => _dueDate; set { _dueDate = value; if (persisted) _controller.Update(this, TaskDueDate, value); } }

        private DateTime _creationTime;
        public DateTime CreationTime { get => _creationTime;}

        ///<summary>Constructor for the TaskDTO.</summary>
        ///<param name="id">the id of the task.</param>
        ///<param name="columnOrdinal">the ordinal of the column.</param>
        ///<param name="emailCreator">the email's creator.</param>
        ///<param name="boardName">the board's name.</param>
        ///<param name="creationTime">the creation time of the task.</param>
        ///<param name="title">the title of the task.</param>
        ///<param name="description">the description of the task.</param>
        ///<param name="emailAssignee">the email's assignee.</param>
        ///<param name="dueDate">the due date of the task.</param>
        ///<param name="controller">the task dal controller.</param>
        public TaskDTO(int id, int columnOrdinal, string emailCreator,string boardName, DateTime creationTime, string title, string description, string emailAssignee,DateTime dueDate, TaskDalController controller):base(controller)
        {
            PrimaryKeyColumnNames = new string[] { "EmailCreator", "BoardName", "ColumnOrdinal","Id" };
            PrimaryKey = new string[] {emailCreator, boardName, "" + columnOrdinal, "" + id };
            _description = description;
            _title = title;
            _emailAssignee = emailAssignee;
            _dueDate = dueDate;
            _creationTime = creationTime;
            persisted = true;

        }

        ///<summary>Constructor for the TaskDTO.</summary>
        ///<param name="id">the id of the task.</param>
        ///<param name="columnOrdinal">the ordinal of the column.</param>
        ///<param name="emailCreator">the email's creator.</param>
        ///<param name="boardName">the board's name.</param>
        ///<param name="creationTime">the creation time of the task.</param>
        ///<param name="title">the title of the task.</param>
        ///<param name="description">the description of the task.</param>
        ///<param name="emailAssignee">the email's assignee.</param>
        ///<param name="dueDate">the due date of the task.</param>
        public TaskDTO(int id, int columnOrdinal, string emailCreator, string boardName, DateTime creationTime, string title, string description, string emailAssignee, DateTime dueDate) : base(new TaskDalController())
        {
            PrimaryKeyColumnNames = new string[] { "EmailCreator", "BoardName", "ColumnOrdinal", "Id" };
            PrimaryKey = new string[] { emailCreator, boardName, "" + columnOrdinal, "" + id };
            _description = description;
            _title = title;
            _emailAssignee = emailAssignee;
            _dueDate = dueDate;
            _creationTime = creationTime;
        }

        ///<summary>Constructor for the TaskDTO.</summary>
        ///<param name="id">the id of the task.</param>
        ///<param name="creationTime">the creation time of the task.</param>
        ///<param name="title">the title of the task.</param>
        ///<param name="description">the description of the task.</param>
        ///<param name="emailAssignee">the email's assignee.</param>
        ///<param name="dueDate">the due date of the task.</param>
        ///<param name="controller">the task dal controller.</param>
        public TaskDTO(int id, DateTime creationTime, string title, string description, string emailAssignee, DateTime dueDate, TaskDalController controller) : base(controller)
        {
            PrimaryKeyColumnNames = new string[] { "EmailCreator", "BoardName", "ColumnOrdinal", "Id" };
            PrimaryKey = new string[] { null, null, null, "" + id };
            _description = description;
            _title = title;
            _emailAssignee = emailAssignee;
            _dueDate = dueDate;
            _creationTime = creationTime;
            persisted = true;

        }

        ///<summary>Constructor for the TaskDTO.</summary>
        ///<param name="id">the id of the task.</param>
        ///<param name="creationTime">the creation time of the task.</param>
        ///<param name="title">the title of the task.</param>
        ///<param name="description">the description of the task.</param>
        ///<param name="emailAssignee">the email's assignee.</param>
        ///<param name="dueDate">the due date of the task.</param>
        public TaskDTO(int id,  DateTime creationTime, string title, string description, string emailAssignee, DateTime dueDate) : base(new TaskDalController())
        {
            PrimaryKeyColumnNames = new string[] { "EmailCreator", "BoardName", "ColumnOrdinal", "Id" };
            PrimaryKey = new string[] { null, null, null, "" + id };
            _description = description;
            _title = title;
            _emailAssignee = emailAssignee;
            _dueDate = dueDate;
            _creationTime = creationTime;
        }
        internal override string GetIdentity()
        {
            return $"task : {_title}";
        }
    }
}
