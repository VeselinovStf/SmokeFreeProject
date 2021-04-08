using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.ViewModels.Base;
using System;
using System.Threading.Tasks;

namespace SmokeFree.ViewModels.OnBoarding
{
    /// <summary>
    /// View Model For OnBoardingView
    /// </summary>
    public class OnBoardingViewModel : ViewModelBase
    {
        #region FIELDS

        /// <summary>
        /// Database
        /// </summary>
        private readonly Realm _realm;

        /// <summary>
        /// Application User
        /// </summary>
        private User _appUser;

        #endregion

        #region CTOR

        public OnBoardingViewModel(
            Realm realm,
            INavigationService navigationService,
            IDateTimeWrapper dateTimeWrapper,
            IAppLogger appLogger,
            IDialogService dialogService)
            : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
            _realm = realm;
        }

        #endregion

        #region INIT

        /// <summary>
        /// Initialize ViewModel - Any user who comes here is going trough on boarding
        /// </summary>
        /// <param name="parameter">Not Required</param>
        /// <returns>Base InitializeAsync</returns>
        public override Task InitializeAsync(object parameter)
        {
            try
            {
                // SET Global User Id
                var globalUserId = Globals.UserId;

                // Get User By Global Id
                var user = _realm.Find<User>(globalUserId);

                // For First Run Of App 
                if (user == null)
                {
                    // Create User If Not Exist
                    var newUser = new User()
                    {
                        Id = globalUserId,
                        CreatedOn = base._dateTime.Now()
                    };

                    // Execute Write Transaction
                    _realm.Write(() =>
                    {
                        _realm.Add(newUser);
                    });

                    this.AppUser = newUser;
                }
                else
                {
                    this.AppUser = user;
                }
            }
            catch (Exception ex)
            {
                base._appLogger.LogError(ex.Message);

                // TODO: A: Navigate to Error View Model
                // Set Option for 'go back'
                base.InternalErrorMessageToUser();
            }

            // TODO: A: Remove After - Navigate to Error View Model after Initialization Exception
            // Return
            return base.InitializeAsync(parameter);
        }

        #endregion

        #region PROPS

        /// <summary>
        /// Application User
        /// </summary>
        public User AppUser
        {
            get { return _appUser; }
            set
            {
                _appUser = value;
            }
        }


        #endregion
    }
}
