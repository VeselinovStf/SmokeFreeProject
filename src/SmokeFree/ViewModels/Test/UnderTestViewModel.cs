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
        public UnderTestViewModel(
            INavigationService navigationService, 
            IDateTimeWrapper dateTimeWrapper, 
            IAppLogger appLogger, 
            IDialogService dialogService) : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
        }
    }
}
