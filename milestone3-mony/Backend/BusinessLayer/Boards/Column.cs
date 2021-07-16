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
    /// The main Column class.
    /// Represents a Column in a Board of the Kanban system.
    /// </summary>
    class Column
    {
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != null && value != "")
                {
                    _name = value;
                    columnDTO.Name = value;
                }
                else throw new Exception("Column name cannot be empty.");
            }
        }
        public int TaskLimit { get { return _taskLimit; } 
            set {
                if ((value > 0 && value >= tasks.Count) || value == -1)
                {
                    _taskLimit = value;
                    columnDTO.TaskLimit = value;
                }  
                else throw new Exception("Invalid task limit.");
            } }

        public int ColumnOrdinal { get { return _columnOrdinal; } set { _columnOrdinal = value; columnDTO.ColumnOrdinal = value; } }
        public int SizeOfColumn { get { return tasks.Count; } }

        public List<int> TaskIds { get { return new List<int>(tasks.Keys); } }
        private Dictionary<int, Task> tasks;
        private string _name;
        private int _taskLimit;
        private int _columnOrdinal;
        private readonly int _columnId;

        public ColumnDTO columnDTO;
        private TaskDalController taskDalController;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        ///<summary>Constructor for the Column.</summary>
        ///<param name="name">the column's name.</param>
        ///<param name="taskLimit">the limit of tasks on this column.</param>
        ///<param name="columnOrdinal">the column number (ID).</param>
        public Column(string name, int taskLimit, int columnId ,int columnOrdinal)
        {
            configLog();
            taskDalController = new();
            columnDTO = new(columnId, columnOrdinal, name, taskLimit);
            this.tasks = new Dictionary<int, Task>();
            this.Name = name;
            this.TaskLimit = taskLimit;
            this._columnOrdinal = columnOrdinal;
            _columnId = columnId;


        }


        public Column(ColumnDTO columnDTO)
        {
            configLog();
            taskDalController = new();
            this._columnId = columnDTO.ColumnId;
            this.tasks = new Dictionary<int, Task>();
            this.columnDTO = columnDTO;
            this.Name = columnDTO.Name;
            this.TaskLimit = columnDTO.TaskLimit;
            this._columnOrdinal = columnDTO.ColumnOrdinal;
            LoadAllTasks();
        }

        /// <summary>
        /// This method configures the logger.
        /// </summary>
        private void configLog()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        ///<summary>This function inserts a task into this column.
        /// </summary>
        ///<param name="id">the id of the task.</param>
        ///<param name="creationDate">the creation date of the task.</param>
        ///<param name="title">the title of the task.</param>
        ///<param name="description">the description of task.</param>
        ///<param name="emailAssignee">the email of the assigned user that gets the task .</param>
        ///<param name="dueDate">the due Date of the task.</param>
        /// <returns> Task </returns>
        public Task AddTask(int id, DateTime creationDate, string title, string description, string emailAssignee, DateTime dueDate)
        {
            if (tasks.Count == _taskLimit)
            {
                log.Warn($"Column {ColumnOrdinal} tried to add a task but reached the task limit.");
                throw new Exception($"Column {ColumnOrdinal} reached the task limit.");
            }

            if (tasks.ContainsKey(id))
            {
                log.Warn($"Column {ColumnOrdinal} tried to add a task with an ID that already exists - {id}.");
                throw new Exception($"Column {ColumnOrdinal} already has a task with the ID - {id}.");
            }
            Task task = new Task(id, creationDate, title, description, emailAssignee, dueDate);
            columnDTO.AddTask(task.taskDTO);

            tasks.Add(task.Id, task);

            return task;

        }

        ///<summary>This function inserts a task into this column.
        /// </summary>
        ///<param name="task">the task that is to be inserted into the column.</param>
        /// <returns> Task </returns>
        public Task AddTask(Task task)
        {
            if (tasks.Count == _taskLimit)
            {
                log.Warn($"Column {ColumnOrdinal} tried to add a task but reached the task limit.");
                throw new Exception($"Column {ColumnOrdinal} reached the task limit.");
            }

            if (tasks.ContainsKey(task.Id))
            {
                log.Warn($"Column {ColumnOrdinal} tried to add a task with an ID that already exists - {task.Id}.");
                throw new Exception($"Column {ColumnOrdinal} already has a task with the ID - {task.Id}.");
            }
            columnDTO.AddTask(task.taskDTO);

            tasks.Add(task.Id, task);

            return task;

        }

        ///<summary>This method verifies if a task (with the specific taskID) exists
        /// </summary>
        ///<param name="taskId">The task that needs to be verified.</param>
        /// <returns> Boolean </returns>
        private bool IsExisting(int taskId)
        {
            return tasks.ContainsKey(taskId);
        }

        ///<summary>This method return a task with this taskID
        /// </summary>
        ///<param name="taskId">The task that will be returned.</param>
        /// <returns> Task </returns>
        public Task GetTask(int taskId)
        {
            if (IsExisting(taskId))
            {
                return tasks[taskId];
            }
            else
            {
                log.Error("The task with task ID: " + taskId + " does not exist in the column : "+this.Name);
                throw new Exception("The task with task ID: " + taskId + " does not exist in the column : " + this.Name);
            }
        }

        ///<summary>This method removes a task with this task ID
        /// </summary>
        ///<param name="taskId">The task that will be removed.</param>
        /// <returns> Boolean </returns>
        public bool RemoveTask(int taskId)
        {
            Task task;
            if (tasks.TryGetValue(taskId, out task))
            {
                task.DeletePersistence();
                tasks.Remove(taskId);
                log.Info("Removed Task with task ID: " + taskId);
                return true;
            }
            else
            {
                log.Error("The task with task ID: " + taskId + " does not exist in the column : " + this.Name);
                throw new Exception("The task with task ID: " + taskId + " does not exist in the column : " + this.Name);
            }
        }

        ///<summary>This method returns all the tasks in the column.
        /// </summary>
        /// <returns> List<Task> </returns>
        public List<Task> GetAllTasks()
        {
            return new List<Task>(tasks.Values);
        }

        ///<summary>This function loads all tasks from the data.
        public void LoadAllTasks()
        {
            List<TaskDTO> tasksDTOs = taskDalController.SelectAllTasks(columnDTO);
            foreach (TaskDTO taskDTO in tasksDTOs)
            {
                Task task = new(taskDTO);
                tasks.Add(taskDTO.Id, task);
            }
        }

        public void DeletePersistence()
        {
            DeleteAllTasks();
            columnDTO.Delete();
        }

        ///<summary>This function deletes all tasks from the data.
        public void DeleteAllTasks()
        {
            foreach (Task task in tasks.Values)
            {
                task.DeletePersistence();
            }
            tasks.Clear();
        }

        ///<summary>This method returns the tasks of the user.
        /// </summary>
        /// <param name="email">The user email for whom all tasks are returned.</param>
        /// <returns> IEnumerable<Task> </returns>
        internal IEnumerable<Task> GetUserTasks(string email)
        {
            List<Task> userTasks = new();
            foreach (Task task in tasks.Values)
            {
                if (task.EmailAssignee == email)
                {
                    userTasks.Add(task);
                }
            }
            return userTasks;
        }

        public void statusOfTask()
        {

        }

    }
}
