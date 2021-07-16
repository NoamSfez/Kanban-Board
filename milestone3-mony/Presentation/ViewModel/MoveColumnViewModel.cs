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
    class MoveColumnViewModel : NotifiableObject
    {
        public MoveColumnViewModel(UserModel loggedInUser, BoardModel board, ColumnModel column)
        {
            Controller = loggedInUser.Controller;
            LoggedInUser = loggedInUser;
            Column = column;
            Board = board;
            Message = "";
            _newOrdinal = column.ColumnOrdinal;
        }

        public BackendController Controller { get; private set; }

        public UserModel LoggedInUser { get; private set; }

        public BoardModel Board { get; private set; }
        public ColumnModel Column { get; private set; }

        private int _newOrdinal;
        public int NewOrdinal
        {
            get => _newOrdinal;
            set
            {
                this._newOrdinal = value;
                RaisePropertyChanged(nameof(NewOrdinal));
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

        private ICommand _moveColumnCommand;

        public ICommand MoveColumnCommand
        {
            get
            {
                if (_moveColumnCommand == null)
                {
                    _moveColumnCommand = new RelayCommand(
                        p => MoveColumn());
                }

                return _moveColumnCommand;
            }
        }

        public IEnumerable<int> ColumnOrdinalOptions
        {
            get
            {
                return Enumerable.Range(0, Board.Columns.Count());
            }
        }

        public void MoveColumn()
        {

            Message = "";
            try
            {
                if (Column.ColumnOrdinal != NewOrdinal)
                    Controller.MoveColumn(LoggedInUser, Column.CreatorEmail, Column.BoardName, Column.ColumnOrdinal, NewOrdinal - Column.ColumnOrdinal);

                DialogHost.CloseDialogCommand.Execute(null, null);
            }
            catch (Exception e)
            {
                Message = e.Message;
            }


        }
    }
}
