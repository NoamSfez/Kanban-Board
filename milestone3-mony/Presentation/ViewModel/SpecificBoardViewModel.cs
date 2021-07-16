using MaterialDesignThemes.Wpf;
using Presentation.Model;
using Presentation.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Presentation.ViewModel
{
    public class SpecificBoardViewModel : NotifiableObject, IPageViewModel
    {
        public SpecificBoardViewModel(UserModel loggedInUser, BoardController boardController, BoardModel board)
        {
            BackendController = loggedInUser.Controller;
            BoardController = boardController;
            Board = board;
            LoggedInUser = loggedInUser;
            _message = "";
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


        private string _filter;
        public string Filter
        {
            get => _filter;
            set
            {
                this._filter = value;
                foreach (var viewModel in ColumnViewModels)
                {
                    viewModel.SetFilter(value);
                }
                RaisePropertyChanged(nameof(Filter));
            }
        }

        public BackendController BackendController { get; private set; }

        public BoardController BoardController { get; private set; }

        public BoardModel Board { get; private set; }

        private ObservableCollection<ColumnViewModel> _columnViewModels;

        public ObservableCollection<ColumnViewModel> ColumnViewModels
        {
            get
            {
                if (_columnViewModels == null)
                {
                    _columnViewModels = new();
                    foreach (ColumnModel column in Board.Columns)
                    {
                        _columnViewModels.Add(new ColumnViewModel(LoggedInUser, column));
                    }
                }
                return _columnViewModels;
            }
        }

        public UserModel LoggedInUser { get; private set; }

        public string BoardName => Board.BoardName;

        public string Name => BoardName;

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


        private ICommand _createTaskCommand;

        public ICommand CreateTaskCommand
        {
            get
            {
                if (_createTaskCommand == null)
                {
                    _createTaskCommand = new RelayCommand(
                        p => CreateTaskDialog());
                }

                return _createTaskCommand;
            }
        }


        private ICommand _moveSelectedTaskRightCommand;

        public ICommand MoveSelectedTaskRightCommand
        {
            get
            {
                if (_moveSelectedTaskRightCommand == null)
                {
                    _moveSelectedTaskRightCommand = new RelayCommand(
                        p => MoveSelectedTaskRight());
                }

                return _moveSelectedTaskRightCommand;
            }
        }



        public async void CreateTaskDialog()
        {
            CreateTaskView createTaskView = new CreateTaskView
            {
                DataContext = new CreateTaskViewModel(LoggedInUser, Board.FirstColumn)
            };

            var result = await DialogHost.Show(createTaskView);
        }


        public void MoveSelectedTaskRight()
        {
            Message = "";
            try
            { // TODO Fix emails (EVERYWHERE)
                BackendController.AdvanceTask(LoggedInUser, Board.CreatorEmail, BoardName, SelectedTask.ColumnOrdinal, SelectedTask.ID);
                ColumnModel oldColumn = Board.Columns.Where(column => column.ColumnOrdinal == SelectedTask.ColumnOrdinal).First();
                ColumnModel newColumn = Board.Columns.Where(column => column.ColumnOrdinal == (SelectedTask.ColumnOrdinal + 1)).First();
                newColumn.Tasks.Add(SelectedTask);
                SelectedTask.ColumnOrdinal = SelectedTask.ColumnOrdinal + 1;
                oldColumn.Tasks.Remove(SelectedTask);

            }
            catch (Exception e)
            {
                Message = e.Message;
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
                DataContext = new SpecificTaskViewModel(LoggedInUser, Board, SelectedTask)
            };

            var result = await DialogHost.Show(specificTaskView);
        }

        public void Reload()
        {
            Board = BackendController.GetBoard(LoggedInUser, Board.CreatorEmail, BoardName);
            _columnViewModels = null;
            RaisePropertyChanged(nameof(ColumnViewModels));
        }



        private ICommand _addColumnCommand;

        public ICommand AddColumnCommand
        {
            get
            {
                if (_addColumnCommand == null)
                {
                    _addColumnCommand = new RelayCommand(
                        p => AddColumnDialog());
                }

                return _addColumnCommand;
            }
        }

        public async void AddColumnDialog()
        {
            CreateColumnView createColumnView = new CreateColumnView
            {
                DataContext = new CreateColumnViewModel(LoggedInUser, this)
            };

            var result = await DialogHost.Show(createColumnView);
        }


        private ICommand _removeColumnCommand;

        public ICommand RemoveColumnCommand
        {
            get
            {
                if (_removeColumnCommand == null)
                {
                    _removeColumnCommand = new RelayCommand(
                        p => RemoveColumn((ColumnModel) p),
                        p => p is ColumnModel);
                }

                return _removeColumnCommand;
            }
        }

        public void RemoveColumn(ColumnModel columnModel)
        {
            try
            {
                Board.Columns.Remove(columnModel);
            }
            catch (Exception e)
            {
                Message = e.Message;
            }
            finally
            {
                Reload();
            }
        }



        private ICommand _moveColumnCommand;

        public ICommand MoveColumnCommand
        {
            get
            {
                if (_moveColumnCommand == null)
                {
                    _moveColumnCommand = new RelayCommand(
                        p => MoveColumn((ColumnModel)p),
                        p => p is ColumnModel);
                }

                return _moveColumnCommand;
            }
        }

        public async void MoveColumn(ColumnModel columnModel)
        {
            try
            {
                MoveColumnView moveColumnView = new MoveColumnView
                {
                    DataContext = new MoveColumnViewModel(LoggedInUser, Board, columnModel)
                };

                var result = await DialogHost.Show(moveColumnView);
            }
            catch (Exception e)
            {
                Message = e.Message;
            }
            finally
            {
                Reload();
            }
        }
    }
}
