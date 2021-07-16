using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    class BoardService
    {
        private BoardController boardController;

        public BoardService(BoardController boardController)
        {
            this.boardController = boardController;
        }

        /// <summary>
        /// Limit the number of tasks in a specific column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response LimitColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int limit)
        {
            try
            {
                boardController.GetBoard(userEmail, creatorEmail, boardName).GetColumn(columnOrdinal).TaskLimit = limit;
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }


        /// <summary>
        /// Get the limit of a specific column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>The limit of the column.</returns>
        public Response<int> GetColumnLimit(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            try
            {
                return Response<int>.FromValue(boardController.GetBoard(userEmail, creatorEmail, boardName).GetColumn(columnOrdinal).TaskLimit);
            }
            catch (Exception e)
            {
                return Response<int>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Get the name of a specific column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>The name of the column.</returns>
        public Response<string> GetColumnName(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            try
            {
                return Response<string>.FromValue(boardController.GetBoard(userEmail, creatorEmail, boardName).GetColumn(columnOrdinal).Name);
            }
            catch (Exception e)
            {
                return Response<string>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Creates a new board for the logged-in user.
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="boardName">The name of the new board</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AddBoard(string userEmail, string boardName)
        {
            try
            {
                boardController.AddBoard(userEmail, boardName);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Removes a board.
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response RemoveBoard(string userEmail, string creatorEmail, string boardName)
        {
            try
            {
                boardController.RemoveBoard(userEmail, creatorEmail, boardName);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Returns the list of board of a user. The user must be logged-in. The function returns all the board names the user created or joined.
        /// </summary>
        /// <param name="userEmail">The email of the user. Must be logged-in.</param>
        /// <returns>A response object with a value set to the board, instead the response should contain a error message in case of an error</returns>
        public Response<IList<String>> GetBoardNames(string userEmail)
        {
            try
            {
                List<BusinessLayer.Board> boards = boardController.GetAllBoards(userEmail);
                List<String> boardNames = new();
                boards.ForEach(delegate (BusinessLayer.Board board)
                {
                    boardNames.Add(board.BoardName);
                });
                return Response<IList<String>>.FromValue(boardNames);
            }
            catch (Exception e)
            {
                return Response<IList<String>>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Adds a board created by another user to the logged-in user. 
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the new board</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response JoinBoard(string userEmail, string creatorEmail, string boardName)
        {
            try
            {
                boardController.JoinBoard(userEmail, creatorEmail, boardName);
                return new Response();
            }
            catch(Exception e)
            {
                return new Response(e.Message);
            }
        }


        ///<summary>This method loads all boards from persistent storage to the runtime.</summary>
        ///<returns cref="Response">The response of the action</returns>
        public Response LoadAllBoards()
        {
            try
            {
                boardController.LoadAllBoards();
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }


        ///<summary>This method deletes all boards from persistent storage and runtime.</summary>
        ///<returns cref="Response">The response of the action</returns>
        public Response DeleteAllBoards()
        {
            try
            {
                boardController.DeleteAllBoards();
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Add a new task.
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        /// <returns>A response object with a value set to the Task, instead the response should contain a error message in case of an error</returns>
        public Response<Task> AddTask(string userEmail, string creatorEmail, string boardName, string title, string description, DateTime dueDate)
        {
            try
            {

                return Response<Task>.FromValue(new Task(boardController.GetBoard(userEmail, creatorEmail, boardName).AddTask(userEmail, title, description, dueDate)));
            }
            catch (Exception e)
            {
                return Response<Task>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Update the due date of a task
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="dueDate">The new due date of the column</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response UpdateTaskDueDate(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, DateTime dueDate)
        {
            //Unless we missed something, why do we need columnOrdinal for this? TaskId is the identifier for the Task and is independant of the column.
            try
            {
                boardController.GetBoard(userEmail, creatorEmail, boardName).UpdateTaskDueDate(userEmail, columnOrdinal, taskId, dueDate);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Update task title
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="title">New title for the task</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response UpdateTaskTitle(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, string title)
        {
            {
                try
                {
                    boardController.GetBoard(userEmail, creatorEmail, boardName).UpdateTaskTitle(userEmail, columnOrdinal, taskId, title);
                    return new Response();
                }
                catch (Exception e)
                {
                    return new Response(e.Message);
                }
            }
        }

        /// <summary>
        /// Update the description of a task
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="description">New description for the task</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response UpdateTaskDescription(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, string description)
        {
            {
                try
                {
                    boardController.GetBoard(userEmail, creatorEmail, boardName).UpdateTaskDescription(userEmail, columnOrdinal, taskId, description);
                    return new Response();
                }
                catch (Exception e)
                {
                    return new Response(e.Message);
                }
            }
        }

        /// <summary>
        /// Assigns a task to a user
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>        
        /// <param name="emailAssignee">Email of the user to assign to task to</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AssignTask(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, string emailAssignee)
        {
            {
                try
                {
                    boardController.GetBoard(userEmail, creatorEmail, boardName).AssignTask(userEmail, columnOrdinal, taskId, emailAssignee);
                    return new Response();
                }
                catch (Exception e)
                {
                    return new Response(e.Message);
                }
            }
        }

        /// <summary>
        /// Advance a task to the next column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AdvanceTask(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId)
        {
            {
                try
                {
                    boardController.GetBoard(userEmail, creatorEmail, boardName).AdvanceTask(userEmail, columnOrdinal, taskId);
                    return new Response();
                }
                catch (Exception e)
                {
                    return new Response(e.Message);
                }
            }
        }
        /// <summary>
        /// Returns a column given it's name
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>A response object with a value set to the Column, The response should contain a error message in case of an error</returns>
        public Response<IList<Task>> GetColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            {
                try
                {
                    List<BusinessLayer.Task> tasks = boardController.GetBoard(userEmail, creatorEmail, boardName).GetColumn(columnOrdinal).GetAllTasks();
                    IList<Task> sTasks = new List<Task>();
                    tasks.ForEach(delegate (BusinessLayer.Task task)
                    {
                        sTasks.Add(new Task(task));
                    });
                    return Response<IList<Task>>.FromValue(sTasks);
                }
                catch (Exception e)
                {
                    return Response<IList<Task>>.FromError(e.Message);
                }
            }
        }

        /// <summary>
        /// Returns all the In progress tasks of the user.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <returns>A response object with a value set to the list of tasks, The response should contain a error message in case of an error</returns>
        public Response<IList<Task>> InProgressTasks(string email)
        {
            try
            {
                List<BusinessLayer.Task> tasks = boardController.GetAllInProgressTasks(email);
                List<Task> sTasks = new List<Task>();
                tasks.ForEach(delegate (BusinessLayer.Task task)
                {
                    sTasks.Add(new Task(task));
                });
                return Response<IList<Task>>.FromValue(sTasks);
            }
            catch (Exception e)
            {
                return Response<IList<Task>>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Adds a new column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The location of the new column. Location for old columns with index>=columnOrdinal is increased by 1 (moved right). The first column is identified by 0, the location increases by 1 for each column.</param>
        /// <param name="columnName">The name for the new columns</param>        
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AddColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal, string columnName)
        {
            try
            {
                boardController.GetBoard(userEmail, creatorEmail, boardName).AddColumn(columnName,-1, columnOrdinal);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }


        /// <summary>
        /// Removes a specific column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column location. The first column location is identified by 0, the location increases by 1 for each column</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response RemoveColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            try
            {
                boardController.GetBoard(userEmail, creatorEmail, boardName).RemoveColumn(columnOrdinal);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            };
        }

        /// <summary>
        /// Renames a specific column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column location. The first column location is identified by 0, the location increases by 1 for each column</param>
        /// <param name="newColumnName">The new column name</param>        
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response RenameColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal, string newColumnName)
        {
            try
            {
                boardController.GetBoard(userEmail, creatorEmail, boardName).RenameColumn(columnOrdinal, newColumnName);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            };
        }

        /// <summary>
        /// Moves a column shiftSize times to the right. If shiftSize is negative, the column moves to the left
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column location. The first column location is identified by 0, the location increases by 1 for each column</param>
        /// <param name="shiftSize">The number of times to move the column, relativly to its current location. Negative values are allowed</param>  
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response MoveColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int shiftSize)
        {
            try
            {
                boardController.GetBoard(userEmail, creatorEmail, boardName).MoveColumn(columnOrdinal,shiftSize);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            };
        }






        // -------------------- OUR ADDITIONAL FUNCTIONALITY ---------------------


        /// <summary>
        /// Returns a board service object
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <returns>A response object with a value set to a board service object</returns>
        public Response<Board> GetBoard(string userEmail, string creatorEmail, string boardName)
        {
            {
                try
                {
                    Board board = new Board(boardController.GetBoard(userEmail, creatorEmail, boardName));
                    return Response<Board>.FromValue(board);
                }
                catch (Exception e)
                {
                    return Response<Board>.FromError(e.Message);
                }
            }
        }

        /// <summary>
        /// Returns a column service object
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The ordinal number for the column</param>
        /// <returns>A response object with a value set to a column service object</returns>
        public Response<Column> GetServiceColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            try
            {
                Column column = new Column(boardController.GetBoard(userEmail, creatorEmail, boardName).GetColumn(columnOrdinal));
                return Response<Column>.FromValue(column);
            }
            catch (Exception e)
            {
                return Response<Column>.FromError(e.Message);
            }
        }


        /// <summary>
        /// Returns a list of created boards
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <returns>A response object with a value set to a list of board service objects</returns>
        public Response<IList<Board>> GetCreatedBoards(string userEmail)
        {
            try
            {// TODO change to get all created boards
                List<BusinessLayer.Board> boards = boardController.GetCreatedBoards(userEmail);
                List<Board> sBoards = new List<Board>();
                boards.ForEach(delegate (BusinessLayer.Board board)
                {
                    sBoards.Add(new Board(board));
                });
                return Response<IList<Board>>.FromValue(sBoards);
            }
            catch (Exception e)
            {
                return Response<IList<Board>>.FromError(e.Message);
            }
        }


        /// <summary>
        /// Returns a list of created boards
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <returns>A response object with a value set to a list of board service objects</returns>
        public Response<IList<Board>> GetJoinedButNotCreatedBoards(string userEmail)
        {
            try
            {// TODO change to get all created boards
                List<BusinessLayer.Board> boards = boardController.GetAllJoinedButNotCreatedBoards(userEmail);
                List<Board> sBoards = new List<Board>();
                boards.ForEach(delegate (BusinessLayer.Board board)
                {
                    sBoards.Add(new Board(board));
                });
                return Response<IList<Board>>.FromValue(sBoards);
            }
            catch (Exception e)
            {
                return Response<IList<Board>>.FromError(e.Message);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="creatorEmail"></param>
        /// <param name="boardName"></param>
        /// <param name="columnOrdinal"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public Response<Task> GetTask(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId)
        {
            try
            {
                Task task = new Task(boardController.GetBoard(userEmail, creatorEmail, boardName).GetColumn(columnOrdinal).GetTask(taskId));
                return Response<Task>.FromValue(task);
            }
            catch (Exception e)
            {
                return Response<Task>.FromError(e.Message);
            }
        }


        /// <summary>
        /// Returns a tuple of a task and it's column ordinal
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public Response<List<Tuple<string, string, int, Task>>> GetAllInProgressTasksExtended(string userEmail)
        {
            try
            {
                List<Tuple<string, string, int, BusinessLayer.Task>> tasks = boardController.GetAllInProgressTasksExtended(userEmail);
                List<Tuple<string, string, int, Task>> sTasks = new List<Tuple<string, string, int, Task>>();
                tasks.ForEach(delegate (Tuple<string, string, int, BusinessLayer.Task> tuple)
                {
                    sTasks.Add(new Tuple<string, string, int, Task>(tuple.Item1, tuple.Item2, tuple.Item3, new Task(tuple.Item4)));
                });
                return Response<List<Tuple<string, string, int, Task>>>.FromValue(sTasks);
            }
            catch (Exception e)
            {
                return Response<List<Tuple<string, string, int, Task>>>.FromError(e.Message);
            }
        }
    }
}
