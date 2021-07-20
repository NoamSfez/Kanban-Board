using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using IntroSE.Kanban.Backend.ServiceLayer;

namespace Presentation.Model
{
    public class TaskModel : NotifiableModelObject
    {
        public UserModel LoggedInUser { get; private set; }

        private int _id;
        public int ID
        {
            get => _id;
        }


        private string _creatorEmail;
        public string CreatorEmail
        {
            get => _creatorEmail;
        }

        private string _boardName;
        public string BoardName
        {
            get => _boardName;
        }

        private int _columnOrdinal;

        public int ColumnOrdinal
        {
            get => _columnOrdinal;
            set
            {
                _columnOrdinal = value;
                RaisePropertyChanged(nameof(ColumnOrdinal));
            }
        }

        private string _taskTitle;

        public string TaskTitle
        {
            get => _taskTitle;
            set
            {
                Message = "";
                try
                {
                    Controller.UpdateTaskTitle(LoggedInUser, _creatorEmail, BoardName, ColumnOrdinal, ID, value);
                    _taskTitle = value;
                    RaisePropertyChanged(nameof(TaskTitle));
                }
                catch (Exception e)
                {
                    Message = e.Message;
                }
            }
        }

        private string _taskDescription;

        public string TaskDescription
        {
            get => _taskDescription;
            set
            {
                Message = "";
                try
                {
                    Controller.UpdateTaskDescription(LoggedInUser, _creatorEmail, BoardName, ColumnOrdinal, ID, value);
                    _taskDescription = value;
                    RaisePropertyChanged(nameof(TaskDescription));
                    RaisePropertyChanged(nameof(DescriptionIconVisibility));
                }
                catch (Exception e)
                {
                    Message = e.Message;
                }
            }
        }

        public bool HasDescription
        {
            get => _taskDescription != null && _taskDescription != "";
        }

        public Visibility DescriptionIconVisibility
        {
            get
            {
                if (HasDescription)
                    return System.Windows.Visibility.Visible;
                else
                    return System.Windows.Visibility.Collapsed;
            }
        }

        private DateTime _taskDueDate;

        public DateTime TaskDueDate
        {
            get => _taskDueDate;
            set
            {
                Message = "";
                try
                {

                    Controller.UpdateTaskDueDate(LoggedInUser, _creatorEmail, BoardName, ColumnOrdinal, ID, value);
                    _taskDueDate = value;
                    RaisePropertyChanged(nameof(TaskDueDate));
                }
                catch (Exception e)
                {
                    Message = e.Message;
                }
            }
        }

        private DateTime _taskCreationDate;
        public DateTime TaskCreationDate
        {
            get => _taskCreationDate;
        }

        private string _taskAssignee;
        public string TaskAssignee
        {
            get => _taskAssignee;
            set
            {
                Message = "";
                try
                {
                    Controller.AssignTask(LoggedInUser, _creatorEmail, BoardName, ColumnOrdinal, ID, value);
                    _taskAssignee = value;
                    RaisePropertyChanged(nameof(TaskAssignee));
                    RaisePropertyChanged(nameof(Border));
                }
                catch (Exception e)
                {
                    Message = e.Message;
                }
            }
        }


        public Brush Background
        {
            get
            {
                if (DateTime.Now > TaskDueDate)
                    return new SolidColorBrush(Color.FromRgb(255, 112, 112));
                long fullDuration = TaskDueDate.Ticks - TaskCreationDate.Ticks;
                long nowDuration = DateTime.Now.Ticks - TaskCreationDate.Ticks;

                // If 75% of time has passed
                if ((fullDuration/4)*3 < nowDuration)
                    return new SolidColorBrush(Color.FromRgb(255, 199, 125));
                

                return new SolidColorBrush(Colors.White);
            }
        }

        public Brush Border
        {
            get
            {
                return TaskAssignee.Equals(LoggedInUser.Email, StringComparison.Ordinal)
                    ? new SolidColorBrush(Color.FromRgb(68, 138, 255))
                    : new SolidColorBrush(Colors.White);
            }
        }


        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                this._message = value;
                RaisePropertyChanged(nameof(Message));
            }
        }



        public TaskModel(UserModel loggedInUser, string creatorEmail, string boardName, int columnOrdinal, Task task) : base(loggedInUser.Controller)
        {
            LoggedInUser = loggedInUser;
            _creatorEmail = creatorEmail;
            _boardName = boardName;
            _columnOrdinal = columnOrdinal;
            _id = task.Id;
            _taskTitle = task.Title;
            _taskDescription = task.Description;
            _taskDueDate = task.DueDate;
            _taskCreationDate = task.CreationTime;
            _taskAssignee = task.emailAssignee;
        }

        public TaskModel(UserModel loggedInUser, string creatorEmail, string boardName, string title, string description, DateTime dueDate) : base(loggedInUser.Controller)
        {
            LoggedInUser = loggedInUser;
            _creatorEmail = creatorEmail;
            _boardName = boardName;
            _columnOrdinal = 0;
            _taskTitle = title;
            _taskDescription = description;
            _taskDueDate = dueDate;
            _taskCreationDate = DateTime.Now;
            _taskAssignee = creatorEmail;
            _message = "";
        }

    }
}
