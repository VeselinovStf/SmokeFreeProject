using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.ViewModels.Base;

namespace SmokeFree.ViewModels.Test
{
    /// <summary>
    /// UnderTestView Model
    /// </summary>
    public class UnderTestViewModel : ViewModelBase
    {
        #region FIELDS

        private readonly Realm _realm;

        #endregion

        #region CTOR

        public UnderTestViewModel(
            Realm realm,
            INavigationService navigationService,
            IDateTimeWrapper dateTimeWrapper,
            IAppLogger appLogger,
            IDialogService dialogService) : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
            // Database
            _realm = realm;
        }

        #endregion
    }
}
