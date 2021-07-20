using MaterialDesignThemes.Wpf;
using Presentation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Presentation.ViewModel
{
    public class CreateTaskViewModel : NotifiableObject
    {

        public CreateTaskViewModel(UserModel user, ColumnModel column)
        {
            Controller = user.Controller;
            LoggedInUser = user;
            Message = "";
            _description = "";
            _column = column;
            _dueDate = DateTime.Now + TimeSpan.FromHours(1);

        }

        public BackendController Controller { get; private set; }

        public UserModel LoggedInUser { get; private set; }

        private ColumnModel _column;

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                this._title = value;
                RaisePropertyChanged(nameof(Title));
            }
        }


        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                this._description = value;
                RaisePropertyChanged(nameof(Description));
            }
        }


        private DateTime _dueDate;

        public DateTime DueDate
        {
            get => _dueDate;
            set
            {
                this._dueDate = value;
                RaisePropertyChanged(nameof(DueDate));
            }
        }
        public DateTime DueTime
        {
            get => DateTime.Now.Date + _dueDate.TimeOfDay;
            set
            {
                this._dueDate = _dueDate.Date + value.TimeOfDay;
                RaisePropertyChanged(nameof(DueDate));
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

        private ICommand _createTaskCommand;

        public ICommand CreateTaskCommand
        {
            get
            {
                if (_createTaskCommand == null)
                {
                    _createTaskCommand = new RelayCommand(
                        p => CreateTask());
                }
                return _createTaskCommand;
            }
        }

        public void CreateTask()
        {

            Message = "";
            try
            {
                TaskModel task = Controller.AddTask(LoggedInUser, _column.CreatorEmail, _column.BoardName, Title, Description, DueDate);
                _column.Tasks.Add(task);

                DialogHost.CloseDialogCommand.Execute(null, null);
            }
            catch (Exception e)
            {
                Message = e.Message;
            }


        }
    }
}
