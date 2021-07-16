using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Model
{
    public class BoardModel: NotifiableModelObject
    {

        public UserModel LoggedInUser { get; private set; }
        private string _boardName;

        private string _creatorEmail;
        public string CreatorEmail
        {
            get => _creatorEmail;
            set
            {
                _creatorEmail = value;
                RaisePropertyChanged("CreatorEmail");
            }
        }

        public string BoardName
        {
            get => _boardName;
            set
            {
                _boardName = value;
                RaisePropertyChanged("BoardName");
            }
        }

        public ColumnModel FirstColumn
        {
            get
            {
                foreach (ColumnModel column in Columns)
                {
                    if (column.ColumnOrdinal == 0)
                        return column;
                }
                throw new Exception("There was no first column!");
            }
        }

        public ObservableCollection<ColumnModel> Columns { get; set; }

        public ObservableCollection<string> JoinedUsers { get; set; }

        public BoardModel(UserModel loggedInUser, string boardName, string creatorEmail) : base(loggedInUser.Controller)
        {
            this.LoggedInUser = loggedInUser;
            this._boardName = boardName;
            this._creatorEmail = creatorEmail;
            JoinedUsers = new();
            JoinedUsers.Add(LoggedInUser.Email);

        }

        public BoardModel(UserModel loggedInUser, Board board) : base(loggedInUser.Controller)
        {
            LoggedInUser = loggedInUser;
            this._creatorEmail = board.CreatorEmail;
            this._boardName = board.BoardName;

            JoinedUsers = new(board.JoinedUsers);

            Columns = new ObservableCollection<ColumnModel>(board.Columns.
                    Select((c, i) => Controller.GetColumn(LoggedInUser, _creatorEmail, _boardName, i)).ToList());
            Columns.CollectionChanged += HandleChange;
        }


        private void HandleChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ColumnModel columnModel in e.OldItems)
                {
                    Controller.RemoveColumn(LoggedInUser, CreatorEmail, BoardName, columnModel.ColumnOrdinal);
                }
            }
        }
    }
}
