using MaterialDesignThemes.Wpf;
using Presentation.Model;
using Presentation.View;
using Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Presentation
{
    class TasksViewModel : NotifiableObject, IPageViewModel
    {
        public string Name
        {
            get { return "Tasks"; }
        }

        public TasksViewModel( UserModel user)
        {
            BackendController = user.Controller;
            _loggedInUser = user;
            InProgressTasks = BackendController.GetInProgressTasks(_loggedInUser);
        }

        public BackendController BackendController { get; private set; }

        private UserModel _loggedInUser;

        private ObservableCollection<TaskModel> _inProgressTasks;

        public ObservableCollection<TaskModel> InProgressTasks { get; private set; }

        public void Reload()
        {
            InProgressTasks = BackendController.GetInProgressTasks(_loggedInUser);
            _tasksView = null;
            RaisePropertyChanged(nameof(TasksView));
        }






        private string _filter;
        public string Filter
        {
            get => _filter;
            set
            {
                this._filter = value;
                SetFilter(value);
                RaisePropertyChanged(nameof(Filter));
            }
        }

        public void SetFilter(string filter)
        {
            TasksView.Filter = (ob) =>
            {
                TaskModel task = ob as TaskModel;
                return task.TaskTitle.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0 || task.TaskDescription.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
            };
            RaisePropertyChanged(nameof(TasksView));
        }



        private ICollectionView _tasksView;

        public ICollectionView TasksView
        {
            get
            {
                if (_tasksView == null)
                {
                    _tasksView = CollectionViewSource.GetDefaultView(InProgressTasks);
                    _tasksView.GroupDescriptions.Clear();
                    _tasksView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(TaskModel.BoardName)));
                }
                return _tasksView;
            }
        }




        private TaskModel _selectedTask;
        public TaskModel SelectedTask
        {
            get
            {
                return _selectedTask;
            }
            set
            {
                _selectedTask = value;
                TaskIsSelected = value != null;
                RaisePropertyChanged(nameof(SelectedTask));
            }
        }

        private bool _taskIsSelected = false;
        public bool TaskIsSelected
        {
            get => _taskIsSelected;
            private set
            {
                _taskIsSelected = value;
                RaisePropertyChanged(nameof(TaskIsSelected));
            }
        }


        private ICommand _OpenTaskCommand;

        public ICommand OpenTaskCommand
        {
            get
            {
                if (_OpenTaskCommand == null)
                {
                    _OpenTaskCommand = new RelayCommand(
                        p => OpenTaskDialog());
                }

                return _OpenTaskCommand;
            }
        }

        public async void OpenTaskDialog()
        {
            SpecificTaskView specificTaskView = new SpecificTaskView
            {
                DataContext = new SpecificTaskViewModel(_loggedInUser, BackendController.GetBoard(_loggedInUser, SelectedTask.CreatorEmail, SelectedTask.BoardName), SelectedTask)
            };

            var result = await DialogHost.Show(specificTaskView);
        }


    }
}
