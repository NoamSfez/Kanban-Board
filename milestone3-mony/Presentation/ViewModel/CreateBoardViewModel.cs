using MaterialDesignThemes.Wpf;
using Presentation.Model;
using System;
using System.Windows.Input;

namespace Presentation
{
    internal class CreateBoardViewModel : NotifiableObject
    {
        public CreateBoardViewModel(UserModel loggedInUser, BoardController boardController)
        {
            Controller = loggedInUser.Controller;
            LoggedInUser = loggedInUser;
            Message = "";
            _boardController = boardController;


        }

        public BackendController Controller { get; private set; }

        public UserModel LoggedInUser { get; private set; }

        private BoardController _boardController;

        private string _boardName;
        public string BoardName
        {
            get => _boardName;
            set
            {
                this._boardName = value;
                RaisePropertyChanged(nameof(BoardName));
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

        private ICommand _createBoardCommand;

        public ICommand CreateBoardCommand
        {
            get
            {
                if (_createBoardCommand == null)
                {
                    _createBoardCommand = new RelayCommand(
                        p => CreateBoard());
                }

                return _createBoardCommand;
            }
        }

        public void CreateBoard()
        {

            Message = "";
            try
            {
                BoardModel boardModel = Controller.CreateBoard(LoggedInUser, LoggedInUser.Email, BoardName);
                _boardController.CreatedBoards.Add(boardModel);

                DialogHost.CloseDialogCommand.Execute(null, null);
            }
            catch (Exception e)
            {
                Message = e.Message;
            }


        }
    }
}