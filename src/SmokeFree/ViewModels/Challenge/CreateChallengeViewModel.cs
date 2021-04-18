using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.ViewModels.Base;

namespace SmokeFree.ViewModels.Challenge
{
    /// <summary>
    /// CreateChallengeVie Model
    /// </summary>
    public class CreateChallengeViewModel : ViewModelBase
    {
        public CreateChallengeViewModel(
            INavigationService navigationService,
            IDateTimeWrapper dateTimeWrapper, 
            IAppLogger appLogger, 
            IDialogService dialogService) : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
        }
    }
}
