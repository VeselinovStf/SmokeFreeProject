using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.ViewModels.Base;
using SmokeFree.ViewModels.OnBoarding;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;

namespace SmokeFree.ViewModels.Test
{
    public class CreateTestViewModel : ViewModelBase
    {


        public CreateTestViewModel(
           INavigationService navigationService,
           IDateTimeWrapper dateTimeWrapper)
           : base(navigationService, dateTimeWrapper)
        {

        }

        public IAsyncValueCommand GOGO => new AsyncValueCommand(GOGOGO);

        private async ValueTask GOGOGO()
        {
            await this._navigationService.NavigateToAsync<OnBoardingViewModel>();
            await this._navigationService.RemoveBackStackAsync();
        }
    }
}
