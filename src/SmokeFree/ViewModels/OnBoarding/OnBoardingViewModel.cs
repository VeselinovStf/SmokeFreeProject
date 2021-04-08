using Realms;
using SmokeFree.Abstraction.Services.General;
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
            IDateTimeWrapper dateTimeWrapper)
            : base(navigationService,dateTimeWrapper)
        {
            _realm = realm;
        }

        #endregion

        #region INIT

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

                // Return ...
                return base.InitializeAsync(parameter);
            }
            catch (Exception ex)
            {

                throw;
            }          
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
