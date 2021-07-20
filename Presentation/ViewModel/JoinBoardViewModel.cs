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
    public class JoinBoardViewModel : NotifiableObject
    {
        public JoinBoardViewModel(UserModel loggedInUser, BoardController boardController)
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


        private string _creatorEmail;
        public string CreatorEmail
        {
            get => _creatorEmail;
            set
            {
                this._creatorEmail = value;
                RaisePropertyChanged(nameof(CreatorEmail));
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

        private ICommand _joinBoardCommand;

        public ICommand JoinBoardCommand
        {
            get
            {
                if (_joinBoardCommand == null)
                {
                    _joinBoardCommand = new RelayCommand(
                        p => JoinBoard());
                }

                return _joinBoardCommand;
            }
        }

        public void JoinBoard()
        {

            Message = "";
            try
            {
                BoardModel boardModel = Controller.JoinBoard(LoggedInUser, CreatorEmail, BoardName);
                _boardController.JoinedBoards.Add(boardModel);

                DialogHost.CloseDialogCommand.Execute(null, null);
            }
            catch (Exception e)
            {
                Message = e.Message;
            }


        }
    }
}