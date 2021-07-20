using NUnit.Framework;
using Moq;
using IntroSE.Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;

namespace Tests
{
    public class UserControllerTests
    {
        private Mock<UserController> userControllerMock;
        private Mock<User> userMock;

        private string userEmail;
        private string userPassword;

        [SetUp]
        public void Setup()
        {
            userControllerMock = new Mock<UserController>();
            userControllerMock.CallBase = true;

            userEmail = "achiya@bgu.ac.il";
            userPassword= "!Abcd1234";
            userMock = new Mock<User>(userEmail, userPassword);
        }

        // --------------------- LOGIN -----------------------
        [Test]
        public void Login_Success()
        {
            User user = userMock.Object;

            userControllerMock.Setup(u => u.GetUser(userEmail)).Returns(user);
            userControllerMock.Setup(u => u.LoggedInUser).Returns(null as User);

            UserController userController = userControllerMock.Object;

            User result = userController.Login(userEmail, userPassword);

            Assert.AreEqual(user, result);
        }

        [Test]
        public void Login_Fail_SomeoneAlreadyLoggedIn()
        {
            User user = userMock.Object;

            string loggedInEmail = "evilachiya@bgu.ac.il";
            string loggedInPassword= "!Abcd1234";
            User loggedInUser = new Mock<User>(loggedInEmail, loggedInPassword).Object;

            userControllerMock.Setup(u => u.GetUser(userEmail)).Returns(user);
            userControllerMock.Setup(u => u.LoggedInUser).Returns(loggedInUser);

            UserController userController = userControllerMock.Object;

            Assert.Throws<Exception>(() => userController.Login(userEmail, userPassword));
        }










        // --------------------- LOGOUT -----------------------

        [Test]
        public void Logout_Success()
        {
            User user = userMock.Object;

            userControllerMock.Setup(u => u.LoggedInUser).Returns(user);

            UserController userController = userControllerMock.Object;

            Assert.DoesNotThrow(() => userController.Logout(userEmail));
        }

        [Test]
        public void Logout_Fail_SomeoneElseLoggedIn()
        {
            User user = userMock.Object;

            string loggedInEmail = "evilachiya@bgu.ac.il";
            string loggedInPassword = "!Abcd1234";
            User loggedInUser = new Mock<User>(loggedInEmail, loggedInPassword).Object;

            userControllerMock.Setup(u => u.LoggedInUser).Returns(loggedInUser);

            UserController userController = userControllerMock.Object;

            Assert.Throws<Exception>(() => userController.Logout(userEmail));
        }

        [Test]
        public void Logout_Fail_NooneLoggedIn()
        {
            User user = userMock.Object;

            userControllerMock.Setup(u => u.LoggedInUser).Returns(null as User);

            UserController userController = userControllerMock.Object;

            Assert.Throws<Exception>(() => userController.Logout(userEmail));
        }








        // --------------------- GETUSER -----------------------


        [Test]
        public void GetUser_Success()
        {
            User user = userMock.Object;
            Dictionary<string, User> users = new();
            users.Add(user.email, user);

            UserController userController = userControllerMock.Object;

            userController.Users = users;

            User result = userController.GetUser(user.email);

            Assert.AreEqual(user, result);
        }

        [Test]
        public void GetUser_Fail_NotRegistered()
        {
            User user = userMock.Object;
            Dictionary<string, User> users = new();

            UserController userController = userControllerMock.Object;

            userController.Users = users;

            Assert.Throws<Exception>(() => userController.GetUser(user.email));

        }
    }
}