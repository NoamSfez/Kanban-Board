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
    class CreateColumnViewModel : NotifiableObject
    {
        public CreateColumnViewModel(UserModel loggedInUser, SpecificBoardViewModel boardViewModel)
        {
            Controller = loggedInUser.Controller;
            LoggedInUser = loggedInUser;
            Message = "";
            _boardViewModel = boardViewModel;
        }

        public BackendController Controller { get; private set; }

        public UserModel LoggedInUser { get; private set; }

        private SpecificBoardViewModel _boardViewModel;


        private int _columnOrdinal;
        public int ColumnOrdinal
        {
            get => _columnOrdinal;
            set
            {
                this._columnOrdinal = value;
                RaisePropertyChanged(nameof(ColumnOrdinal));
            }
        }



        private string _columnName;
        public string ColumnName
        {
            get => _columnName;
            set
            {
                this._columnName = value;
                RaisePropertyChanged(nameof(ColumnName));
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

        private ICommand _createColumnCommand;

        public ICommand CreateColumnCommand
        {
            get
            {
                if (_createColumnCommand == null)
                {
                    _createColumnCommand = new RelayCommand(
                        p => CreateColumn());
                }

                return _createColumnCommand;
            }
        }


        public IEnumerable<int> ColumnOrdinalOptions
        {
            get
            {
                return Enumerable.Range(0,_boardViewModel.Board.Columns.Count()+1);
            }
        }

        public void CreateColumn()
        {

            Message = "";
            try
            {
                Controller.AddColumn(LoggedInUser, _boardViewModel.Board.CreatorEmail, _boardViewModel.Board.BoardName, ColumnOrdinal, ColumnName);
                _boardViewModel.Reload();

                DialogHost.CloseDialogCommand.Execute(null, null);
            }
            catch (Exception e)
            {
                Message = e.Message;
            }


        }
    }
}
