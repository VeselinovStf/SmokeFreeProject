using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.ViewModels.AppSettings;
using SmokeFree.ViewModels.Base;
using System;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;

namespace SmokeFree.ViewModels.Challenge
{
    public class CompletedChallengeViewModel : ViewModelBase
    {
        public CompletedChallengeViewModel(INavigationService navigationService, IDateTimeWrapper dateTimeWrapper, IAppLogger appLogger, IDialogService dialogService) : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
        }

        /// <summary>
        /// Navigate to settings View
        /// </summary>
        public IAsyncCommand OnSettingsCommand => new AsyncCommand(
            ExecuteNavigateToSetting,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async Task ExecuteNavigateToSetting()
        {
            try
            {
                await base._navigationService.NavigateToAsync<AppSettingsViewModel>();
            }
            catch (Exception ex)
            {
                base._appLogger.LogCritical(ex);

                await base.InternalErrorMessageToUser();
            }

        }
    }
}
