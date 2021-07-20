using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer.DalControllers;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using log4net;
using log4net.Config;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Tests")]

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    /// <summary>
    /// This controller class keeps track of boards that are registered in the system.
    /// </summary>
    class BoardController
    {
        public Dictionary<Tuple<string,string>, Board> boards;
        private UserController userController;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public BoardDalController boardDalController;
        private UserJoinedBoardDalController userJoinedBoardDalController;

        ///<summary>Constructor for the Board Controller.</summary>
        ///<param name="userController">the controller of the users.</param>
        public BoardController(UserController userController)
        {
            configLog();
            this.userController = userController;
            boardDalController = new();
            userJoinedBoardDalController = new();
            boards = new Dictionary<Tuple<string, string>, Board>();
        }

        /// <summary>
        /// This method configures the logger.
        /// </summary>
        private void configLog()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        /// <summary>
        /// This method checks the user is logged in.
        /// </summary>
        /// <param name="email">the email of the user.</param>
        /// <exception cref="System.Exception">Thrown when the user is not logged in.</exception>
        private void VerifyLoggedIn(string email)
        {
            if (userController.LoggedInUser.email != email)
                throw new Exception("User is not logged in.");
        }

        /// <summary>
        /// Add a board
        /// </summary>
        /// <param name="email">the email of the user.</param>
        /// <param name="boardName">the name of the board to be logged into.</param>
        /// <exception cref="System.Exception">Thrown when the user has already created a board with this name.</exception>
        public void AddBoard(string email, string boardName)
        {
            VerifyLoggedIn(email);
            Tuple<string, string> identifier = Tuple.Create(email, boardName);
            if (boards.ContainsKey(identifier))
            {
                log.Warn($"{email} tried to create a board with the same name as an existing one: {boardName}.");
                throw new Exception($"{email} already has a board named {boardName}.");
            }
            boards.Add(identifier, new Board(userController.GetUser(email), boardName));
            log.Debug("Added Board : " + boardName);
        }

        /// <summary>
        /// Adds a board created by another user to the logged-in user. 
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board's creator</param>
        /// <param name="boardName">The name of the new board</param>
        /// <returns>A response object. The response should contain an error message in case of an error</returns>
        public void JoinBoard(string userEmail, string creatorEmail, string boardName)
        {
            VerifyLoggedIn(userEmail);

            if (boards.TryGetValue(Tuple.Create(creatorEmail, boardName), out Board board))
            {
                if (board.CheckJoined(userEmail))
                {
                    String message = userEmail + " tried to join board they already joined: " + creatorEmail + "-" + boardName;
                    log.Error(message);
                    throw new Exception(message);
                }        
                else
                {
                    board.AddUser(userController.GetUser(userEmail));
                }
            }
            else
            {
                log.Error("The board does not exist");
                throw new Exception("The board does not exist.");
            }
        }

        /// <summary>
        /// Remove a board
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board's creator. Must be logged in</param>
        /// <param name="boardName">the name of the board to be removed.</param>
        /// <exception cref="System.Exception">Thrown when the board does not exist.</exception>
        public void RemoveBoard(string userEmail, string creatorEmail, string boardName)
        {
            VerifyLoggedIn(userEmail);

            if (userEmail != creatorEmail)
            {
                String message = userEmail + " tried to remove a board they didn't create: " + creatorEmail + "-" + boardName;
                log.Error(message);
                throw new Exception(message);
            }

            Tuple<string, string> identifier = Tuple.Create(creatorEmail, boardName);
            Board board;
            if (boards.TryGetValue(identifier, out board))
            {
                board.DeletePersistence();
                boards.Remove(identifier);
                log.Info("Removed Board : " + creatorEmail + "-" + boardName);
            }
            else
            {
                String message = userEmail + " tried to remove a non-existent board: " + creatorEmail + "-" + boardName;
                log.Warn(message);
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Get a board
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in and joined the board</param>
        /// <param name="creatorEmail">Email of the board's creator</param>
        /// <param name="boardName">the name of the board to be retrieved.</param>
        /// <exception cref="System.Exception">Thrown when the board does not exist.</exception>
        public Board GetBoard(string userEmail, string creatorEmail, string boardName)
        {
            VerifyLoggedIn(userEmail);

            if (boards.TryGetValue(Tuple.Create(creatorEmail, boardName), out Board board))
            {
                if (board.CheckJoined(userEmail))
                    return board;
                else
                {
                    String message = userEmail + " tried to get an unjoined board: " + creatorEmail + "-" + boardName;
                    log.Error(message);
                    throw new Exception(message);
                }
            }
            else
            {
                log.Error("The board does not exists");
                throw new Exception("The board does not exist.");
            }
        }

        ///<summary>This method returns all boards by a specific user's email. 
        /// </summary>
        ///<param name="email">the email in the board.</param>
        /// <returns> List<Board> </returns>
        /// <exception cref="System.Exception">Thrown when the email does not have any boards.</exception>
        public List<Board> GetAllBoards(string email)
        {
            VerifyLoggedIn(email);
          
            List<Board> boardsJoined = new();
            foreach (Board board in boards.Values)
            {
                if (board.CheckJoined(email))
                    boardsJoined.Add(board);
            }
            //If boardsBelongToEmail is empty
            if (!boardsJoined.Any())
            {
                String message = email + "tried to get all joined boards but does not have any.";
                log.Warn(message);
                //throw new Exception(message);
            }
            return boardsJoined;
        }

        ///<summary>This method returns all tasks in progress from all boards for a specific user. 
        /// </summary>
        ///<param name="email">the email in the board.</param>
        /// <returns> List<Board> </returns>
        /// <exception cref="System.Exception">Thrown when the email has no boards.</exception>
        public List<Task> GetAllInProgressTasks(string email)
        {
            List<Board> joinedBoards = GetAllBoards(email);
            List<Task> tasks = new();
            joinedBoards.ForEach(delegate (Board board)
            {
                tasks.AddRange(board.GetAllInProgressTasks(email));
            });
            return tasks;
        }

        ///<summary>This function loads all the boards from the data. 
        public void LoadAllBoards()
        {
            List<BoardDTO> boardDTOs= boardDalController.SelectAllBoards();
            foreach (BoardDTO boardDTO in boardDTOs)
            {
                Board board = new Board(boardDTO);
                LoadJoinedUsers(board);
                boards.Add(Tuple.Create(board.CreatorEmail, board.BoardName), board);
            }
        }

        ///<summary>This function loads all users who have access to this board.
        /// </summary>
        ///<param name="board">the board that users have access to.</param>
        public void LoadJoinedUsers(Board board)
        {
            List<UserJoinedBoardDTO> userJoinedBoardDTOs = userJoinedBoardDalController.SelectAllFromBoard(board.boardDTO);
            foreach (UserJoinedBoardDTO userJoinedBoardDTO in userJoinedBoardDTOs)
            {
                User user = userController.GetUser(userJoinedBoardDTO.JoinedUserEmail);
                board.LoadUser(user);
            }
        }

        ///<summary>This function deletes all boards from persistent storage.
        public void DeleteAllBoards()
        {
            foreach (Board board in boards.Values)
            {
                board.DeletePersistence();
            }
            boards.Clear();
        }

        ///<summary>This method returns all boards a specific users/email joined but didnt create. 
        /// </summary>
        ///<param name="email">the email in the board.</param>
        /// <returns> List<Board> </returns>
        /// <exception cref="System.Exception">Thrown when the user does not have any boards.</exception>
        public List<Board> GetAllJoinedButNotCreatedBoards(string email)
        {
            VerifyLoggedIn(email);

            List<Board> boardsJoined = new();
            foreach (Board board in boards.Values)
            {
                if (board.CheckJoined(email))
                    if (!(board.CreatorEmail == email))
                        boardsJoined.Add(board);
            }
            //If boardsBelongToEmail is empty
            if (!boardsJoined.Any())
            {
                String message = email + "tried to get all joined boards but does not have any.";
                log.Warn(message);
                //throw new Exception(message);
            }
            return boardsJoined;
        }


        ///<summary>This method returns all boards a specific user created. 
        /// </summary>
        ///<param name="email">the email in the board.</param>
        /// <returns> List<Board> </returns>
        /// <exception cref="System.Exception">Thrown when the users does not have any boards.</exception>
        public List<Board> GetCreatedBoards(string email)
        {
            VerifyLoggedIn(email);

            List<Board> boardsJoined = new();
            foreach (Board board in boards.Values)
            {
                if (board.CreatorEmail == email)
                    boardsJoined.Add(board);
            }
            //If boardsBelongToEmail is empty
            if (!boardsJoined.Any())
            {
                String message = email + "tried to get all created boards but does not have any.";
                log.Warn(message);
                //throw new Exception(message);
            }
            return boardsJoined;
        }


        public List<Tuple<string,string,int, Task>> GetAllInProgressTasksExtended(string userEmail)
        {
            List<Tuple<string, string, int, Task>> allInProgressTasks = new List<Tuple<string, string, int, Task>>();
            List<Board> joinedBoards = GetAllBoards(userEmail);
            List<Task> tasks = new();
            joinedBoards.ForEach(delegate (Board board)
            {
                List<Tuple<int, Task>> inProgressTasks = board.GetInProgressTasksWithColumns(userEmail);
                inProgressTasks.ForEach(delegate (Tuple<int, Task> pair)
                {
                    allInProgressTasks.Add(new Tuple<string, string, int, Task>(board.CreatorEmail, board.BoardName, pair.Item1, pair.Item2));
                });
            });
            return allInProgressTasks;
        }
    }
}
