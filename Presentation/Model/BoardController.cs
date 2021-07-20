using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Model
{
    public class BoardController : NotifiableModelObject
    {

        public UserModel LoggedInUser { get; private set; }
        public ObservableCollection<BoardModel> CreatedBoards { get; set; }
        public ObservableCollection<BoardModel> JoinedBoards { get; set; }

        public BoardController(BackendController controller, UserModel user) : base(controller)
        {
            this.LoggedInUser = user;
            CreatedBoards = controller.GetCreatedBoards(LoggedInUser);
            CreatedBoards.CollectionChanged += CreatedHandleChange;

            JoinedBoards = controller.GetJoinedBoards(LoggedInUser);
            CreatedBoards.CollectionChanged += JoinedHandleChange;
        }




        private void CreatedHandleChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (BoardModel y in e.NewItems)
                {
                   // Controller.CreateBoard(y.CreatorEmail,y.BoardName);
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (BoardModel y in e.OldItems)
                {
                    Controller.RemoveBoard(LoggedInUser.Email, y.CreatorEmail, y.BoardName);
                }
            }
        }



        private void JoinedHandleChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (BoardModel y in e.NewItems)
                {
                    //Controller.CreateBoard(y.CreatorEmail, y.BoardName);
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (BoardModel y in e.OldItems)
                {
                    Controller.RemoveBoard(LoggedInUser.Email, y.CreatorEmail, y.BoardName);
                }
            }
        }
    }
}
