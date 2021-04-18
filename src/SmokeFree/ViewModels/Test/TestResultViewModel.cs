using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Resx;
using SmokeFree.ViewModels.Base;

namespace SmokeFree.ViewModels.Test
{
    /// <summary>
    /// TestResultView Model
    /// </summary>
    public class TestResultViewModel : ViewModelBase
    {
        #region FIELDS


        #endregion

        #region CTOR

        public TestResultViewModel(
            INavigationService navigationService,
            IDateTimeWrapper dateTimeWrapper,
            IAppLogger appLogger,
            IDialogService dialogService) : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
            // Set View Title
            ViewTitle = AppResources.TestResultViewModelTiitle;
        }

        #endregion
    }
}
