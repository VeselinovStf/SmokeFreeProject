using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.ViewModels.Base;

namespace SmokeFree.ViewModels.Challenge
{
    /// <summary>
    /// ChallengeVie Model
    /// </summary>
    public class ChallengeViewModel : ViewModelBase
    {
        public ChallengeViewModel(
            INavigationService navigationService,
            IDateTimeWrapper dateTimeWrapper,
            IAppLogger appLogger,
            IDialogService dialogService) : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
        }
    }
}
