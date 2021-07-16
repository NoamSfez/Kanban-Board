using Presentation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Presentation
{
    public class ApplicationViewModel : NotifiableObject
    {
        #region Fields
        private UserModel _user;

        private ICommand _changePageCommand;
        private ICommand _logoutCommand;
        private ICommand _changePageToBoards;

        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;

        #endregion

        public ApplicationViewModel(UserModel user)
        {
            _user = user;
            // Add available pages
            PageViewModels.Add(new BoardsViewModel(this, user));
            PageViewModels.Add(new TasksViewModel(user));

            // Set starting page
            CurrentPageViewModel = PageViewModels[0];
        }

        #region Properties / Commands

        public IPageViewModel? SelectedItem
        {
            get => _currentPageViewModel;
            set => ChangeViewModel(value);
        }

        public ICommand ChangePageCommand
        {
            get
            {
                if (_changePageCommand == null)
                {
                    _changePageCommand = new RelayCommand(
                        p => ChangeViewModel((IPageViewModel)p),
                        p => p is IPageViewModel);
                }

                return _changePageCommand;
            }
        }

        public ICommand ChangePageToBoards
        {
            get
            {
                if (_changePageToBoards == null)
                {
                    _changePageToBoards = new RelayCommand(
                        p => ChangeViewModel(PageViewModels[0]));
                }

                return _changePageToBoards;
            }
        }

        public ICommand LogoutCommand
        {
            get
            {
                if (_logoutCommand == null)
                {
                    _logoutCommand = new RelayCommand(
                        p => Logout((ICloseable)p),
                        p => p is ICloseable);
                }

                return _logoutCommand;
            }
        }

        public List<IPageViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IPageViewModel>();

                return _pageViewModels;
            }
        }

        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged("CurrentPageViewModel");
                }
            }
        }

        public String LoggedInUserEmail
        {
            get
            {
                return _user.Email;
            }
        }

        #endregion

        #region Methods

        private void ChangeViewModel(IPageViewModel viewModel)
        {
            viewModel.Reload();
            CurrentPageViewModel = viewModel;

            OnPropertyChanged("CurrentPageViewModel");
        }

        private void Logout(ICloseable window)
        {
            _user.Controller.Logout(LoggedInUserEmail);
            LoginView loginView = new();
            loginView.Show();

            if (window != null)
            {
                window.Close();
            }

        }

        #endregion
    }
}
