using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DalControllers;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using log4net;
using log4net.Config;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    /// <summary>
    /// The main Board class.
    /// Represents a single Board in the Kanban system.
    /// </summary>
    class Board
    {
        private readonly int minimumOfColumns = 2;
        public string CreatorEmail { get { return creatorEmail; } }
        public string BoardName { get { return boardName; } }

        public List<int> ColumnOrdinals { get { return new List<int>(columns.Keys); } }

        public List<string> JoinedUsersEmails { get { return new List<string>(joinedUsers.Keys); } }

        private Dictionary<int, Column> columns;
        private Dictionary<string, User> joinedUsers;
        private string creatorEmail;
        private string boardName;
        private int numOfTasks;
        public int NumOfTasks { get { return numOfTasks; } set { numOfTasks = value;  boardDTO.NumOfTasks = value; } }


        private int numOfColumns;
        public int NumOfColumns { get { return numOfColumns; } set { numOfColumns = value; boardDTO.NumOfColumns = value; } }



        public BoardDTO boardDTO;
        private ColumnDalController columnDalController;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        ///<summary>Constructor for the board.</summary>
        ///<param name="creator">the creating user.</param>
        ///<param name="boardName">the board's name.</param>
        public Board(User creator, string boardName)
        {
            configLog();
            columnDalController = new();
            joinedUsers = new();
            this.creatorEmail = creator.email;
            this.boardName = boardName;
            columns = new Dictionary<int, Column>();
            numOfTasks = 0;
            numOfColumns = 0;

            boardDTO = new(creatorEmail, boardName,  numOfTasks, numOfColumns);
            boardDTO.Insert();

            AddColumn("backlog", -1, 0);
            AddColumn("in progress", -1, 1);
            AddColumn("done", -1, 2);

            AddUser(creator);
        }


        ///<summary>Constructor for the board.</summary>
        ///<param name="boardName">the board's name.</param>
        public Board(BoardDTO boardDTO)
        {
            configLog();
            columnDalController = new();
            joinedUsers = new();
            columns = new Dictionary<int, Column>();
            this.creatorEmail = boardDTO.EmailCreator;
            this.boardName = boardDTO.BoardName;
            this.numOfTasks = boardDTO.NumOfTasks;
            this.numOfColumns = boardDTO.NumOfColumns;
            this.boardDTO = boardDTO;
            LoadAllColumns();
        }

        private void configLog()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        ///<summary>This method creates a new column. 
        /// </summary>
        ///<param name="name">the name of the column.</param>
        ///<param name="taskLimit">The amount that the column can contain.</param>
        ///<param name="columnOrdinal">The location of the column.</param>
        public void AddColumn(string name, int taskLimit, int columnOrdinal)
        {
            if (columns.ContainsKey(columnOrdinal))
            {
                Column newColumn = new(name, taskLimit, NumOfColumns, columnOrdinal);
                Column currentColumn = newColumn;
                Column temp;

                // Swapping
                for (int i = columnOrdinal; i < columns.Count; i++)
                {
                    temp = columns[i];
                    columns[i] = currentColumn;
                    currentColumn.ColumnOrdinal = i;
                    currentColumn = temp;
                }
                currentColumn.ColumnOrdinal = columns.Count;
                columns.Add(columns.Count, currentColumn);

                boardDTO.AddColumn(newColumn.columnDTO);
                

            }
            else {
                Column column = new(name, taskLimit, NumOfColumns, columnOrdinal);
                boardDTO.AddColumn(column.columnDTO);
                columns.Add(columns.Count, column);
            }
            NumOfColumns += 1;
        }

        ///<summary>This method inserts a task into the column. 
        /// </summary>
        ///<param name="columnOrdinal">the number of the column in the board.</param>
        ///<param name="title">the name of the task.</param>
        ///<param name="description">The description of the task.</param>
        ///<param name="dueDate">The dueDate of the task.</param> 
        /// <returns> Task </returns>
        private Task AddTask(string userEmail, int columnOrdinal , string title, string description, DateTime dueDate)
        {
            Task newTask = columns[columnOrdinal].AddTask(numOfTasks, DateTime.Now, title, description, userEmail, dueDate);
            NumOfTasks = NumOfTasks + 1;
            log.Info($"{creatorEmail} - {boardName} : Added task : " + title + " to column : " + columns[0].Name);
            return newTask;
        }

        ///<summary>This method inserts a task into the default first column. 
        /// </summary>
        ///<param name="title">the name of the task.</param>
        ///<param name="description">The description of the task.</param>
        ///<param name="dueDate">The dueDate of the task.</param> 
        /// <returns> Task </returns>
        public Task AddTask(string userEmail, string title, string description, DateTime dueDate)
        {
            return AddTask(userEmail, 0, title, description, dueDate);
        }

        ///<summary>This method passes a test to the next column. 
        /// </summary>
        ///<param name="columnOrdinal">the number of column in columns.</param>
        ///<param name="taskId">The id of the task.</param>
        public void AdvanceTask(string userEmail, int columnOrdinal, int taskId)
        {
            TestCanUpdateTask(userEmail, columnOrdinal, taskId);
            MoveTask(columnOrdinal, columnOrdinal + 1, taskId);          
        }

        ///<summary>This method passes a test from one column to another. 
        /// </summary>
        ///<param name="columnOrdinal">the number of the column in columns.</param>
        ///<param name="nextColumnOrdinal">The number of the other column in columns.</param>
        ///<param name="taskId">The id of the task.</param>
        private void MoveTask(int currColumnOrdinal, int nextColumnOrdinal, int taskId)
        {
            // GetColumn will throw an exception if the column does not exist.
            // GetTask will throw an exception if the task does not exist.
            Task task = GetColumn(currColumnOrdinal).GetTask(taskId);
            GetColumn(nextColumnOrdinal).AddTask(task.Clone()); // using new Task to avoid issues with RemoveTasks removing the new one.
            GetColumn(currColumnOrdinal).RemoveTask(taskId);
            log.Info("Moved task " + columns[nextColumnOrdinal].GetTask(taskId).Title + " from Column : " + columns[currColumnOrdinal].Name + " to Column " + columns[nextColumnOrdinal].Name);
        }

        ///<summary>This method returns a specific column from columns. 
        /// </summary>
        ///<param name="columnOrdinal">the number of the column in columns.</param>
        /// <returns> Column </returns>
        public Column GetColumn(int columnOrdinal)
        {
            Column column;
            if (columns.TryGetValue(columnOrdinal, out column))
            {
                return column;
            }
            else
            {
                log.Warn("Column number " + columnOrdinal + " does not exist.");
                throw new Exception("Column number " + columnOrdinal + " does not exist.");
            }
        }

        ///<summary>This method inserts a user into the board. 
        /// </summary>
        ///<param name="user">the user that is added.</param>
        public void AddUser(User user)
        {
            try
            {
                joinedUsers.Add(user.email, user);
                UserJoinedBoardDTO userJoinedBoardDTO = new(CreatorEmail, BoardName, user.email);
                userJoinedBoardDTO.Insert();
            }
            catch (ArgumentException)
            {
                String message = "User " + user.email + " already joined the board.";
                log.Warn(message);
                throw new Exception(message);
            }
                
        }

        ///<summary>This method checks if the user's email is already inside in joined users. 
        /// </summary>
        ///<param name="userEmail">the user email that is to be inserted.</param>
        /// <returns> bool </returns>
        public bool CheckJoined (string userEmail)
        {
            return joinedUsers.ContainsKey(userEmail);
        }


        private void TestCanUpdateTask(string userEmail, int columnOrdinal, int taskId)
        {
            if (columnOrdinal == ColumnOrdinals.Max())
            {
                String message = userEmail + " tried to update task " + taskId + " in column " + columnOrdinal + " but the task is done.";
                log.Warn(message);
                throw new Exception(message);
            }
            if (!(GetColumn(columnOrdinal).GetTask(taskId).EmailAssignee == userEmail))
            {
                String message = userEmail + " tried to update task " + taskId + " in column " + columnOrdinal + " but isn't assigned to it.";
                log.Warn(message);
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Update the description of a task
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated</param>
        /// <param name="description">New description for the task</param>
        public void UpdateTaskDescription(string userEmail, int columnOrdinal, int taskId, string description)
        {
            TestCanUpdateTask(userEmail, columnOrdinal, taskId);
            GetColumn(columnOrdinal).GetTask(taskId).Description = description;
        }

        internal IEnumerable<Task> GetAllInProgressTasks(string email)
        {
            int inProgressColumnOrdinal = 1;
            return columns[inProgressColumnOrdinal].GetUserTasks(email);
        }


        /// <summary>
        /// Update the title of a task
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated</param>
        /// <param name="title">New title for the task</param>
        public void UpdateTaskTitle(string userEmail, int columnOrdinal, int taskId, string title)
        {
            TestCanUpdateTask(userEmail, columnOrdinal, taskId);
            GetColumn(columnOrdinal).GetTask(taskId).Title = title;
        }


        /// <summary>
        /// Update the due date of a task
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated</param>
        /// <param name="dueDate">The new due date of the column</param>
        public void UpdateTaskDueDate(string userEmail, int columnOrdinal, int taskId, DateTime dueDate)
        {
            TestCanUpdateTask(userEmail, columnOrdinal, taskId);
            GetColumn(columnOrdinal).GetTask(taskId).DueDate = dueDate;
        }

        /// <summary>
        /// Assigns a task to a user
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated</param>        
        /// <param name="emailAssignee">Email of the user to assign the task to</param>
        public void AssignTask(string userEmail, int columnOrdinal, int taskId, string emailAssignee)
        {
            TestCanUpdateTask(userEmail, columnOrdinal, taskId);
            GetColumn(columnOrdinal).GetTask(taskId).EmailAssignee = emailAssignee;
        }

        ///<summary>This function loads all columns from the data.
        public void LoadAllColumns()
        {
            List<ColumnDTO> columnsDTOs = columnDalController.SelectAllColumns(boardDTO);
            foreach(ColumnDTO columnDTO in columnsDTOs)
            {
                Column column = new Column(columnDTO);
                columns.Add(column.ColumnOrdinal, column);
            }
        }

        ///<summary>This function deletes all boards from the data.
        public void DeletePersistence()
        {
            DeleteAllColumns();
            DeleteAllJoinedUsers();
            boardDTO.Delete();
        }

        ///<summary>This function deletes all columns from the data.
        public void DeleteAllColumns()
        {
            foreach (Column column in columns.Values)
            {
                column.DeletePersistence();
            }
            columns.Clear();
        }

        ///<summary>This function deletes all joined users from the data.
        public void DeleteAllJoinedUsers()
        {
            foreach (User user in joinedUsers.Values)
            {
                UserJoinedBoardDTO userJoinedBoardDTO = new(CreatorEmail, BoardName, user.email);
                userJoinedBoardDTO.Delete();
            }
            joinedUsers.Clear();
        }

        ///<summary>This function adds a user to the joined users.
        /// </summary>
        /// <param name="user">the user that is to be added.</param>
        public void LoadUser(User user)
        {
            joinedUsers.Add(user.email, user);
        }


        /// <summary>
        /// Removes a specific column
        /// </summary>
        /// <param name="columnOrdinal">The column location. The first column location is identified by 0, the location increases by 1 for each column</param>

        public void RemoveColumn(int columnOrdinal)
        {
            if (columns.Count== minimumOfColumns)
            {
                String message = "The board "+ BoardName + "contains" + minimumOfColumns + " columns therefore it is not possible to remove the column.";
                log.Warn(message);
                throw new Exception(message);

            }
            int numOfTask;
            int nextColumn;
            if (columnOrdinal == 0)
            {
                numOfTask = columns[0].TaskIds.Count + columns[1].TaskIds.Count;
                nextColumn = 1;
            }
            else
            {
                numOfTask = columns[columnOrdinal].TaskIds.Count + columns[columnOrdinal-1].TaskIds.Count;
                nextColumn = columnOrdinal - 1;
            }
            
            if (columns[nextColumn].TaskLimit != -1 && columns[nextColumn].TaskLimit < numOfTask)
            {
                String message = "The board " + columns[nextColumn].Name + " cannot contain this amount of tasks";
                log.Warn(message);
                throw new Exception(message);
            }

            Column columnToBeRemoved = columns[columnOrdinal];

            // Move tasks from the column to be removed to it's neighbor
            columnToBeRemoved.TaskIds.ForEach(delegate (int taskId){
                MoveTask(columnOrdinal, nextColumn, taskId);
            });
            for (int i = columnOrdinal; i < columns.Count - 1; i++)
            {
                columns[i] = columns[i + 1];
                columns[i].ColumnOrdinal = i;
            }
            columns.Remove(columns.Count - 1);
            columnToBeRemoved.columnDTO.Delete();
        }

        /// <summary>
        /// Renames a specific column
        /// <param name="columnOrdinal">The column location. The first column location is identified by 0, the location increases by 1 for each column</param>
        /// <param name="newColumnName">The new column name</param>        
        public void RenameColumn(int columnOrdinal,String newColumnName)
        {
            columns[columnOrdinal].Name = newColumnName;
        }


        /// <summary>
        /// Moves a column shiftSize times to the right. If shiftSize is negative, the column moves to the left
        /// </summary>
        /// <param name="columnOrdinal">The column location. The first column location is identified by 0, the location increases by 1 for each column</param>
        /// <param name="shiftSize">The number of times to move the column, relativly to its current location. Negative values are allowed</param>  
       
        public void MoveColumn(int columnOrdinal, int shiftSize)
        {
            int index = columnOrdinal + shiftSize;
            if (index < 0 || columns.Count <= index)
            {
                String message = "Shift size out of bounds.";
                log.Warn(message);
                throw new Exception(message);
            }
            if (columns[columnOrdinal].TaskIds.Count !=0)
            {
                String message = "The column " + columns[columnOrdinal].Name + " is not empty.";
                log.Warn(message);
                throw new Exception(message);
            }

            Column temp;
            if (shiftSize > 0) 
            { 
                for (int i = columnOrdinal; i < index; i++)
                {
                    temp = columns[i];
                    columns[i] = columns[i + 1];
                    columns[i].ColumnOrdinal = i;
                    columns[i + 1] = temp;
                }
                columns[index].ColumnOrdinal = index;
            }
            else
            {
                for (int i = columnOrdinal; i > index; i--)
                {
                    temp = columns[i];
                    columns[i] = columns[i - 1];
                    columns[i].ColumnOrdinal = i;
                    columns[i - 1] = temp;
                }
                columns[index].ColumnOrdinal = index;
            }
        }




        // ----------------- Milestone 3 --------------



        public List<Tuple<int, Task>> GetInProgressTasksWithColumns(string userEmail)
        {
            List<Tuple<int, Task>> inProgressTasks = new List<Tuple<int, Task>>();
            int FirstColumnOrdinal = ColumnOrdinals.Min();
            int LastColumnOrdinal = ColumnOrdinals.Max();
            foreach (KeyValuePair<int,Column> pair in columns)
            {
                if (pair.Key != FirstColumnOrdinal && pair.Key != LastColumnOrdinal)
                {
                    List<Task> tasks = pair.Value.GetAllTasks();
                    foreach (Task task in tasks)
                    {
                        if (task.EmailAssignee == userEmail)
                            inProgressTasks.Add(new Tuple<int, Task>(pair.Key, task));
                    }
                }

            }
            return inProgressTasks;
        }
    }
}
