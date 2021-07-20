using Presentation.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Presentation.ViewModel
{
    public class ColumnViewModel : NotifiableObject
    {
        public ColumnViewModel(UserModel loggedInUser, ColumnModel column)
        {
            BackendController = loggedInUser.Controller;

            Column = column;
            LoggedInUser = loggedInUser;
        }


        public BackendController BackendController { get; private set; }


        public ColumnModel Column { get; private set; }

        public UserModel LoggedInUser { get; private set; }




        private ICollectionView _tasksView;

        public ICollectionView TasksView
        {
            get
            {
                if (_tasksView == null)
                {
                    _tasksView = CollectionViewSource.GetDefaultView(Column.Tasks);
                }
                return _tasksView;
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


        private ICommand _sortByDueDateCommand;

        public ICommand SortByDueDateCommand
        {
            get
            {
                if (_sortByDueDateCommand == null)
                {
                    _sortByDueDateCommand = new RelayCommand(
                        p => SortByDueDate());
                }

                return _sortByDueDateCommand;
            }
        }

        public void SortByDueDate()
        {
            TasksView.SortDescriptions.Clear();
            TasksView.SortDescriptions.Add(new SortDescription(nameof(TaskModel.TaskDueDate), ListSortDirection.Ascending));
        }

        private ICommand _sortByTitleCommand;

        public ICommand SortByTitleCommand
        {
            get
            {
                if (_sortByTitleCommand == null)
                {
                    _sortByTitleCommand = new RelayCommand(
                        p => SortByTitle());
                }

                return _sortByTitleCommand;
            }
        }

        public void SortByTitle()
        {
            TasksView.SortDescriptions.Clear();
            TasksView.SortDescriptions.Add(new SortDescription(nameof(TaskModel.TaskTitle), ListSortDirection.Ascending));
        }


        public Visibility AddTaskButtonVisibility
        {
            get
            {
                if (Column.ColumnOrdinal == 0)
                    return System.Windows.Visibility.Visible;
                else
                    return System.Windows.Visibility.Collapsed;
            }
        }
    }
}
