using System;
namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    internal class BoardDTO : DTO
    {
        public const string NumOfTasksColumnName = "NumOfTasks";
        public const string NumOfColumnsColumnName = "NumOfColumns";

        private int _numOfTasks;
        public int NumOfTasks { get => _numOfTasks; set { _numOfTasks = value; if (persisted) _controller.Update(this, NumOfTasksColumnName, value); } }
        private int _numOfColumns;
        public int NumOfColumns { get => _numOfColumns; set { _numOfColumns = value; if (persisted) _controller.Update(this, NumOfColumnsColumnName, value); } }
        public string EmailCreator { get => PrimaryKey[0]; }

        public string BoardName { get => PrimaryKey[1]; }

        ///<summary>Constructor for the BoardDTO.</summary>
        ///<param name="emailCreator">the email creator.</param>
        ///<param name="boardName">the board's name.</param>
        ///<param name="numOfTasks">the number of tasks.</param>
        ///<param name="controller">the board dal controller.</param>
        public BoardDTO(string emailCreator, string boardName , int numOfTasks, int numOfColumns, BoardDalController controller) : base(controller)
        {
            PrimaryKeyColumnNames = new string[] { "EmailCreator" , "BoardName" };
            PrimaryKey = new string[] { emailCreator, boardName };
            _numOfTasks = numOfTasks;
            _numOfColumns = numOfColumns;
            persisted = true;
        }

        ///<summary>Constructor for the BoardDTO.</summary>
        ///<param name="emailCreator">the email creator.</param>
        ///<param name="boardName">the board's name.</param>
        ///<param name="numOfTasks">the number of tasks.</param>
        public BoardDTO(string emailCreator, string boardName, int numOfTasks, int numOfColumns) : base(new BoardDalController())
        {
            PrimaryKeyColumnNames = new string[] { "EmailCreator", "BoardName" };
            PrimaryKey = new string[] { emailCreator, boardName };
            _numOfTasks = numOfTasks;
            _numOfColumns = numOfColumns;
        }

        ///<summary>This method inserts a column into the data. 
        /// </summary>
        ///<param name="columnDTO">The columnDTO that is inserted into the data.</param>
        public void AddColumn(ColumnDTO columnDTO)
        {
            columnDTO.PrimaryKey = new string[] { "" + columnDTO.ColumnId, EmailCreator, BoardName };
            columnDTO.Insert();
        }

        internal override string GetIdentity()
        {
            return $"board : {BoardName}";
        }

        public void RemoveColumn(ColumnDTO columnDTO)
        {

        }
    }
}
