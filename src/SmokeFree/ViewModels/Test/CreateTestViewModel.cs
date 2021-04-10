using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.ViewModels.Base;

namespace SmokeFree.ViewModels.Test
{
    public class CreateTestViewModel : ViewModelBase
    {
        #region FIELDS

        /// <summary>
        /// Database
        /// </summary>
        private readonly Realm _realm;

        #endregion

        #region CTOR

        public CreateTestViewModel(
            Realm realm,
           INavigationService navigationService,
           IDateTimeWrapper dateTimeWrapper,
           IAppLogger appLogger,
           IDialogService dialogService)
           : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
            // Set Database
            _realm = realm;
        }

        #endregion

        #region INIT


        #endregion

        #region COMMANDS


        #endregion

        #region PROPS


        #endregion

    }
}
