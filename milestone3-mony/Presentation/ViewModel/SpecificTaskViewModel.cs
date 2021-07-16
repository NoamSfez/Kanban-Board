using Presentation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.ViewModel
{
    public class SpecificTaskViewModel : NotifiableObject
    {
        public SpecificTaskViewModel(UserModel loggedInUser, BoardModel board, TaskModel task)
        {
            BackendController = loggedInUser.Controller;
            Task = task;
            LoggedInUser = loggedInUser;
            Board = board;
        }

        public BoardModel Board { get; private set; }

        public DateTime DueDate
        {
            get => Task.TaskDueDate;
            set
            {
                this.Task.TaskDueDate = value;
                RaisePropertyChanged(nameof(DueDate));
            }
        }
        public DateTime DueTime
        {
            get => DateTime.Now.Date + DueDate.TimeOfDay;
            set
            {
                DueDate = DueDate.Date + value.TimeOfDay;
                RaisePropertyChanged(nameof(DueDate));
            }
        }

        public BackendController BackendController { get; private set; }


        public TaskModel Task { get; private set; }

        public UserModel LoggedInUser { get; private set; }


    }
}
