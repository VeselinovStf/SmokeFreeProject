using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.ViewModels.Base;

namespace SmokeFree.ViewModels.AppSettings
{
    /// <summary>
    /// AppSetting View Model
    /// </summary>
    public class AppSettingsViewModel : ViewModelBase
    {
        public AppSettingsViewModel(
            INavigationService navigationService,
            IDateTimeWrapper dateTimeWrapper,
            IAppLogger appLogger,
            IDialogService dialogService) : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
        }
    }
}
