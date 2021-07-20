using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Model
{
    public class ColumnModel : NotifiableModelObject
    {

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

        public UserModel LoggedInUser { get; private set; }

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

        private int _taskLimit;
        public int TaskLimit
        {
            get => _taskLimit;
            set
            {
                Message = "";
                try
                {
                    Controller.LimitColumn(LoggedInUser, _creatorEmail, BoardName, ColumnOrdinal, value);
                    _taskLimit = value;
                    RaisePropertyChanged(nameof(ColumnName));
                }
                catch (Exception e)
                {
                    Message = e.Message;
                }
            }
        }


        private string _columnName;
        public string ColumnName
        {
            get => _columnName;
            set
            {
                Message = "";
                try
                {
                    Controller.RenameColumn(LoggedInUser, _creatorEmail, BoardName, ColumnOrdinal, value);
                    _columnName = value;
                    RaisePropertyChanged(nameof(ColumnName));
                }
                catch (Exception e)
                {
                    Message = e.Message;
                }
            }
        }

        public ObservableCollection<TaskModel> Tasks { get; set; }



        public ColumnModel(UserModel loggedInUser, string creatorEmail, string boardName, int columnOrdinal, string columnName, int taskLimit) : base(loggedInUser.Controller)
        {
            LoggedInUser = loggedInUser;
            _creatorEmail = creatorEmail;
            _boardName = boardName;
            _columnOrdinal = columnOrdinal;
            _columnName = columnName;
            _taskLimit = taskLimit;
        }

        public ColumnModel(UserModel loggedInUser, string creatorEmail, string boardName, Column column) : base(loggedInUser.Controller) 
        {
            LoggedInUser = loggedInUser;
            _creatorEmail = creatorEmail;
            _boardName = boardName;
            _columnOrdinal = column.ColumnOrdinal;
            _columnName = column.Name;
            _taskLimit = column.TaskLimit;

            Tasks = new ObservableCollection<TaskModel>(column.Tasks.
                    Select((c, i) => Controller.GetTask(loggedInUser, _creatorEmail, _boardName, _columnOrdinal, c)).ToList());
            Tasks.CollectionChanged += HandleChange;
        }

        private void HandleChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (TaskModel newTask in e.NewItems)
                {
                    //Controller.AddTask(_creatorEmail, CreatorEmail, BoardName, newTask.TaskTitle, newTask.TaskDescription, newTask.TaskDueDate);
                    RaisePropertyChanged(nameof(Tasks));
                }

            }
        }

    }
}
