using MaterialDesignThemes.Wpf;
using Presentation.Model;
using Presentation.View;
using Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Presentation
{
    class BoardsViewModel : NotifiableObject, IPageViewModel
    {
        private ApplicationViewModel _appVM;

        public BoardsViewModel(ApplicationViewModel appVM, UserModel user)
        {
            _appVM = appVM;
            BackendController = user.Controller;
            _loggedInUser = user;
            BoardController = new BoardController(BackendController, user);
        }

        public BackendController BackendController { get; private set; }

        public BoardController BoardController { get; private set; }

        private SpecificBoardView _currentSpecificBoardView;


        public SpecificBoardView CurrentSpecificBoardView
        {
            get
            {
                return _currentSpecificBoardView;
            }
            set
            {
                _currentSpecificBoardView = value;
                EnableDeleteBoard = value != null;
                RaisePropertyChanged(nameof(CurrentSpecificBoardView));
            }
        }

        private UserModel _loggedInUser;

        private BoardModel _selectedCreatedBoard;
        public BoardModel SelectedCreatedBoard
        {
            get
            {
                return _selectedCreatedBoard;
            }
            set
            {
                _selectedCreatedBoard = value;
                EnableDeleteBoard = value != null;
                RaisePropertyChanged(nameof(SelectedCreatedBoard));
            }
        }
        private bool _enableDeleteBoard = false;
        public bool EnableDeleteBoard
        {
            get => _enableDeleteBoard;
            private set
            {
                _enableDeleteBoard = value;
                RaisePropertyChanged(nameof(EnableDeleteBoard));
            }
        }

        public string Name
        {
            get { return "Boards"; }
        }

        private ICommand _createBoardCommand;

        public ICommand CreateBoardCommand
        {
            get
            {
                if (_createBoardCommand == null)
                {
                    _createBoardCommand = new RelayCommand(
                        p => CreateBoardDialog());
                }

                return _createBoardCommand;
            }
        }


        private ICommand _deleteBoardCommand;

        public ICommand DeleteBoardCommand
        {
            get
            {
                if (_deleteBoardCommand == null)
                {
                    _deleteBoardCommand = new RelayCommand(
                        p => DeleteBoard());
                }

                return _deleteBoardCommand;
            }
        }

        private ICommand _openBoardCommand;

        public ICommand OpenBoardCommand
        {
            get
            {
                if (_openBoardCommand == null)
                {
                    _openBoardCommand = new RelayCommand(
                        p => OpenSpecificBoardView((BoardModel) p),
                        p => p is BoardModel);
                }

                return _openBoardCommand;
            }
        }


        private ICommand _joinBoardCommand;

        public ICommand JoinBoardCommand
        {
            get
            {
                if (_joinBoardCommand == null)
                {
                    _joinBoardCommand = new RelayCommand(
                        p => JoinBoardDialog());
                }

                return _joinBoardCommand;
            }
        }

        public async void CreateBoardDialog()
        {
            CreateBoardView createBoardView = new CreateBoardView
            {
                DataContext = new CreateBoardViewModel(_loggedInUser, BoardController)
            };

            var result = await DialogHost.Show(createBoardView);
        }

        public async void JoinBoardDialog()
        {
            JoinBoardView joinBoardView = new JoinBoardView
            {
                DataContext = new JoinBoardViewModel(_loggedInUser, BoardController)
            };

            var result = await DialogHost.Show(joinBoardView);
        }

        public void DeleteBoard()
        {
            //Message = "";
            try
            {
                BoardController.CreatedBoards.Remove(SelectedCreatedBoard);
            }
            catch (Exception e)
            {
                //Message = e.Message;
            }
        }

        public void OpenSpecificBoardView(BoardModel board)
        {
            // Reload before opening incase there were changes in the task view
            board = BackendController.GetBoard(_loggedInUser, board.CreatorEmail, board.BoardName);

            _appVM.CurrentPageViewModel = new SpecificBoardViewModel(_loggedInUser, BoardController, board);
        }

        public void Reload()
        {
            BoardController = new BoardController(BackendController, _loggedInUser);
        }
    }
}
