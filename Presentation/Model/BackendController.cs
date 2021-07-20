using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using IntroSE.Kanban.Backend.ServiceLayer;
using Presentation.Model;

namespace Presentation
{
    public class BackendController
    {
        private Service Service { get; set; }
        public BackendController(Service service)
        {
            this.Service = service;
        }

        public BackendController()
        {
            this.Service = new Service();
            Service.LoadData();
        }

        public UserModel Login(string username, string password)
        {
            Response<User> user = Service.Login(username, password);
            if (user.ErrorOccured)
            {
                throw new Exception(user.ErrorMessage);
            }
            return new UserModel(this, username);
        }

        internal void Register(string username, string password)
        {
            Response res = Service.Register(username, password);
            if (res.ErrorOccured)
            {
                throw new Exception(res.ErrorMessage);
            }
        }

        internal void Logout(string username)
        {
            Response res = Service.Logout(username);
            if (res.ErrorOccured)
            {
                throw new Exception(res.ErrorMessage);
            }
        }

        internal BoardModel CreateBoard(UserModel loggedInUser, string creatorEmail, string boardName)
        {
            Response res = Service.AddBoard(creatorEmail, boardName);
            if (res.ErrorOccured)
            {
                throw new Exception(res.ErrorMessage);
            }
            return GetBoard(loggedInUser, creatorEmail, boardName);
        }


        internal BoardModel JoinBoard(UserModel loggedInUser, string creatorEmail, string boardName)
        {
            Response res = Service.JoinBoard(loggedInUser.Email, creatorEmail, boardName);
            if (res.ErrorOccured)
            {
                throw new Exception(res.ErrorMessage);
            }
            return GetBoard(loggedInUser, creatorEmail, boardName);
        }

        internal void RemoveBoard(string userEmail, string creatorEmail, string boardName)
        {
            Response res = Service.RemoveBoard(userEmail, creatorEmail, boardName);
            if (res.ErrorOccured)
            {
                throw new Exception(res.ErrorMessage);
            }
        }

        internal List<string> GetJoinedBoardNames(string userEmail)
        {
            IList<string> joinedBoardNames = Service.GetBoardNames(userEmail).Value;
            return new List<string>(joinedBoardNames);
        }

        internal List<string> GetCreatedBoardNames(string userEmail)
        {
            // TODO Change to getCreatedBoardNames?
            IList<string> joinedBoardNames = Service.GetBoardNames(userEmail).Value;
            return new List<string>(joinedBoardNames);
        }

        internal BoardModel GetBoard(UserModel loggedInUser, string creatorEmail ,string boardName)
        {
            Response<Board> board = Service.GetBoard(loggedInUser.Email, creatorEmail, boardName);
            if (board.ErrorOccured)
            {
                throw new Exception(board.ErrorMessage);
            }
            return new BoardModel(loggedInUser, board.Value);
        }

        internal ObservableCollection<BoardModel> GetCreatedBoards(UserModel loggedInUser)
        {
            IList<Board> createdBoards = Service.GetCreatedBoards(loggedInUser.Email).Value;

            if (createdBoards == null)
                return new ObservableCollection<BoardModel>();
            ObservableCollection<BoardModel> createdBoardModels = new();
            new List<Board>(createdBoards).ForEach(delegate (Board board)
            {
                createdBoardModels.Add(new BoardModel(loggedInUser, board));
            });


            return createdBoardModels;

        }

        internal ObservableCollection<BoardModel> GetJoinedBoards(UserModel loggedInUser)
        {
            IList<Board> joinedBoards = Service.GetJoinedButNotCreatedBoards(loggedInUser.Email).Value;
            if (joinedBoards == null)
                return new ObservableCollection<BoardModel>();
            ObservableCollection<BoardModel> joinedBoardModels = new();
            new List<Board>(joinedBoards).ForEach(delegate (Board board)
            {
                joinedBoardModels.Add(new BoardModel(loggedInUser, board));
            });


            return joinedBoardModels;

        }


        internal ColumnModel GetColumn(UserModel loggedInUser, string creatorEmail, string boardName, int columnOrdinal)
        {
            Response<Column> column = Service.GetServiceColumn(loggedInUser.Email, creatorEmail, boardName, columnOrdinal);
            if (column.ErrorOccured)
            {
                throw new Exception(column.ErrorMessage);
            }
            return new ColumnModel(loggedInUser, creatorEmail, boardName, column.Value);
        }

        internal TaskModel GetTask(UserModel loggedInUser, string creatorEmail, string boardName, int columnOrdinal, int taskId)
        {
            Response<Task> task = Service.GetTask(loggedInUser.Email, creatorEmail, boardName, columnOrdinal, taskId);
            if (task.ErrorOccured)
            {
                throw new Exception(task.ErrorMessage);
            }
            return new TaskModel(loggedInUser, creatorEmail, boardName, columnOrdinal, task.Value);
        }


        internal TaskModel AddTask(UserModel loggedInUser, string creatorEmail, string boardName, string title, string description, DateTime dueDate)
        {
            Response<Task> task = Service.AddTask(loggedInUser.Email, creatorEmail, boardName, title, description, dueDate);
            if (task.ErrorOccured)
            {
                throw new Exception(task.ErrorMessage);
            }
            return new TaskModel(loggedInUser, creatorEmail, boardName, 0, task.Value);
        }


        internal void AdvanceTask(UserModel loggedInUser, string creatorEmail, string boardName, int columnOrdinal, int taskId)
        {
            Response res = Service.AdvanceTask(loggedInUser.Email, creatorEmail, boardName, columnOrdinal, taskId);
            if (res.ErrorOccured)
            {
                throw new Exception(res.ErrorMessage);
            }
        }


        internal void UpdateTaskTitle(UserModel loggedInUser, string creatorEmail, string boardName, int columnOrdinal, int taskId, string title)
        {
            Response res = Service.UpdateTaskTitle(loggedInUser.Email, creatorEmail, boardName, columnOrdinal, taskId, title);
            if (res.ErrorOccured)
            {
                throw new Exception(res.ErrorMessage);
            }
        }

        internal void UpdateTaskDescription(UserModel loggedInUser, string creatorEmail, string boardName, int columnOrdinal, int taskId, string description)
        {
            Response res = Service.UpdateTaskDescription(loggedInUser.Email, creatorEmail, boardName, columnOrdinal, taskId, description);
            if (res.ErrorOccured)
            {
                throw new Exception(res.ErrorMessage);
            }
        }

        internal void UpdateTaskDueDate(UserModel loggedInUser, string creatorEmail, string boardName, int columnOrdinal, int taskId, DateTime dueDate)
        {
            Response res = Service.UpdateTaskDueDate(loggedInUser.Email, creatorEmail, boardName, columnOrdinal, taskId, dueDate);
            if (res.ErrorOccured)
            {
                throw new Exception(res.ErrorMessage);
            }
        }

        internal void AssignTask(UserModel loggedInUser, string creatorEmail, string boardName, int columnOrdinal, int taskId, string emailAssignee)
        {
            Response res = Service.AssignTask(loggedInUser.Email, creatorEmail, boardName, columnOrdinal, taskId, emailAssignee);
            if (res.ErrorOccured)
            {
                throw new Exception(res.ErrorMessage);
            }
        }



        internal ObservableCollection<TaskModel> GetInProgressTasks(UserModel loggedInUser)
        {
            Response<List<Tuple<string, string, int, Task>>> res = Service.GetAllInProgressTasksExtended(loggedInUser.Email);
            if (res.ErrorOccured)
            {
                throw new Exception(res.ErrorMessage);
            }
            if (res == null)
                return new ObservableCollection<TaskModel>();
            ObservableCollection<TaskModel> inProgressTasks = new();
            res.Value.ForEach(delegate (Tuple<string, string, int, Task> tuple)
            {
                inProgressTasks.Add(new TaskModel(loggedInUser, tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4));
            });
            return inProgressTasks;
        }


        // --------------------- COLUMNS --------------------


        internal void AddColumn(UserModel loggedInUser, string creatorEmail, string boardName, int columnOrdinal, string columnName)
        {
            Response res = Service.AddColumn(loggedInUser.Email, creatorEmail, boardName, columnOrdinal, columnName);
            if (res.ErrorOccured)
            {
                throw new Exception(res.ErrorMessage);
            }
        }

        internal void RemoveColumn(UserModel loggedInUser, string creatorEmail, string boardName, int columnOrdinal)
        {
            Response res = Service.RemoveColumn(loggedInUser.Email, creatorEmail, boardName, columnOrdinal);
            if (res.ErrorOccured)
            {
                throw new Exception(res.ErrorMessage);
            }
        }


        internal void MoveColumn(UserModel loggedInUser, string creatorEmail, string boardName, int columnOrdinal, int shiftSize)
        {
            Response res = Service.MoveColumn(loggedInUser.Email, creatorEmail, boardName, columnOrdinal, shiftSize);
            if (res.ErrorOccured)
            {
                throw new Exception(res.ErrorMessage);
            }
        }


        internal void RenameColumn(UserModel loggedInUser, string creatorEmail, string boardName, int columnOrdinal, string newColumnName)
        {
            Response res = Service.RenameColumn(loggedInUser.Email, creatorEmail, boardName, columnOrdinal, newColumnName);
            if (res.ErrorOccured)
            {
                throw new Exception(res.ErrorMessage);
            }
        }


        internal void LimitColumn(UserModel loggedInUser, string creatorEmail, string boardName, int columnOrdinal, int limit)
        {
            Response res = Service.LimitColumn(loggedInUser.Email, creatorEmail, boardName, columnOrdinal, limit);
            if (res.ErrorOccured)
            {
                throw new Exception(res.ErrorMessage);
            }
        }
    }
}
