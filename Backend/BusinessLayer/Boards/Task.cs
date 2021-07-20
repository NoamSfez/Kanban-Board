using System;
using log4net;
using log4net.Config;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using IntroSE.Kanban.Backend.DataAccessLayer.DalControllers;

namespace IntroSE.Kanban.Backend.BusinessLayer

{   /// <summary>
    /// The main Task class.
    /// Represents a Task in the Kanban system.
    /// </summary>

    class Task
    {
        static readonly int maximumDescription = 300;
        static readonly int maximumTitle = 50;
        public int Id { get { return _id; }}
        public DateTime CreationTime { get { return _creationTime; }}
        public string Title { get { return _title; } set { UpdateTaskTitle(value); taskDTO.Title = value; } }
        public string Description { get { return _description; } set { UpdateTaskDescription(value); taskDTO.Description = value; } }
        public DateTime DueDate { get { return _dueDate; } set { UpdateTaskDueDate(value); taskDTO.DueDate = value; } }

        public string EmailAssignee { get { return _emailAssignee; } set { _emailAssignee = value; taskDTO.EmailAssignee = value; } }

        private bool _done;
        private readonly int _id;
        private readonly DateTime _creationTime;
        private string _title;
        private string _description;
        private DateTime _dueDate;
        private string _emailAssignee;

        public TaskDTO taskDTO;
        private TaskDalController taskDalController;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Constructor for the Task
        /// </summary>
        public Task(int id, DateTime creationTime, string title, string description, string emailAssignee, DateTime dueDate)
        {
            taskDTO = new(id, creationTime, title, description, emailAssignee, dueDate);
            this._id = id;
            this._creationTime = creationTime;
            this.DueDate = dueDate;
            this.Description = description;
            this.Title = title;
            this._emailAssignee = emailAssignee;
            configLog();
            taskDalController = new();
            this._done = false;
        }

        
        internal Task(TaskDTO taskDTO)
        {
            configLog();
            taskDalController = new();
            this.taskDTO = taskDTO;
            this._id = taskDTO.Id;
            this._creationTime = taskDTO.CreationTime;
            this._dueDate = taskDTO.DueDate;
            this._description = taskDTO.Description;
            this._title = taskDTO.Title;
            this._emailAssignee = taskDTO.EmailAssignee;
        }

        /// <summary>
        /// This method configures the logger.
        /// </summary>
        private void configLog()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        /// <summary>
        /// Update the due date for the task
        /// </summary>
        /// <param name="dueDate">The new due date of the task</param>
        /// <returns> No return value
        public void UpdateTaskDueDate(DateTime dueDate)
        {
            verifyDueDate(dueDate);
            log.Info("The due date has changed from : "+ this.DueDate + " to : "+dueDate);
            this._dueDate = dueDate;
        }

        /// <summary>
        /// Verify the due date (not expired)
        /// </summary>
        /// <param name="dueDate">The new dueDate of the task</param>
        /// <returns> No return value
        private void verifyDueDate(DateTime dueDate)
        {
            if (dueDate < DateTime.Now)
            {
                log.Warn("The due date has expired.");
                throw new Exception("The due date has expired.");
            }
        }

        /// <summary>
        /// Update the title for the task
        /// </summary>
        /// <param name="title">The new title of the task</param>
        /// <returns> No return value
        public void UpdateTaskTitle(string title)
        {
            verifyTitle(title);
            log.Info("The title has changed from : " + this.Title + " to : " + title);
            this._title = title;
        }

        /// <summary>
        /// Verify the title (maximum 50 leters & not empty)
        /// </summary>
        /// <param name="title">The new title of the task</param>
        /// <returns> No return value
        private void verifyTitle(string title)
        {
            if (title.Length > maximumTitle || title.Length == 0)
            {
                log.Warn("The title must have a maximum of " + maximumTitle + " characters and not be empty.");
                throw new Exception("The title must have a maximum of "+ maximumTitle + " characters and not be empty.");
            }
        }

        /// <summary>
        /// Update the description for the task
        /// </summary>
        /// <param name="description">The new description of the task</param>
        /// <returns> No return value
        public void UpdateTaskDescription(string description)
        {
            verifiyDescription(description);
            log.Info("The description has changed from : " + this.Description + " to : " + description);
            this._description = description;
        }

        /// <summary>
        /// Verify the description (maximum 300 leters)
        /// </summary>
        /// <param name="description">The new description of the task</param>
        /// <returns> No return value
        private void verifiyDescription(string description)
        {
            if (description.Length > maximumDescription)
            {
                log.Warn("The description must have a maximum of " + maximumDescription + " characters.");
                throw new Exception("The description must have a maximum of " + maximumDescription + " characters.");
            }
        }

        ///<summary>This function deletes this task from the data.
        public void DeletePersistence()
        {
            taskDTO.Delete();
        }

        ///<summary>This function creates a new task.
        /// <returns> Task </returns>
        internal Task Clone()
        {
            return new Task(_id, _creationTime, _title, _description, _emailAssignee, _dueDate);
        }
    }
}