using Presentation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Presentation
{
    class LoginViewModel : NotifiableObject
    {
        public LoginViewModel()
        {
            this.Controller = new BackendController();
        }

        public BackendController Controller { get; private set; }

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                this._username = value;
                RaisePropertyChanged("Username");
            }
        }
        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                this._password = value;
                RaisePropertyChanged("Password");
            }
        }
        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                this._message = value;
                RaisePropertyChanged("Message");
            }
        }

        private string _regUsername;
        public string RegUsername
        {
            get => _regUsername;
            set
            {
                this._regUsername = value;
                RaisePropertyChanged("Username");
            }
        }
        private string _regPassword;
        public string RegPassword
        {
            get => _regPassword;
            set
            {
                this._regPassword = value;
                RaisePropertyChanged("Password");
            }
        }

        private string _regMessage;
        public string RegMessage
        {
            get => _regMessage;
            set
            {
                this._regMessage = value;
                RaisePropertyChanged("RegMessage");
            }
        }

        private ICommand _loginCommand;
        private ICommand _registerCommand;

        public ICommand LoginCommand
        {
            get
            {
                if (_loginCommand == null)
                {
                    _loginCommand = new RelayCommand(
                        p => Login((ICloseable)p),
                        p => p is ICloseable);
                }

                return _loginCommand;
            }
        }

        public ICommand RegisterCommand
        {
            get
            {
                if (_registerCommand == null)
                {
                    _registerCommand = new RelayCommand(
                        p => Register());
                }

                return _registerCommand;
            }
        }

        public void Login(ICloseable window)
        {
            Message = "";
            try
            {
                UserModel user = Controller.Login(Username, Password);

                try
                {
                    ApplicationView applicationView = new ApplicationView(user);
                    applicationView.Show();

                    if (window != null)
                    {
                        window.Close();
                    }
                }
                catch (Exception e)
                {
                    Controller.Logout(Username);
                    Message = e.Message;
                }
            }
            catch (Exception e)
            {
                Message = e.Message;
            }

        }

        public void Register()
        {
            RegMessage = "";
            try
            {
                Controller.Register(RegUsername, RegPassword);
                RegMessage = "Registered successfully!";
            }
            catch (Exception e)
            {
                RegMessage = e.Message;
            }
        }
    }
}
