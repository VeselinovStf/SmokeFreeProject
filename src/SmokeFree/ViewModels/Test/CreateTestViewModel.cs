using SmokeFree.Abstraction.Services.General;
using SmokeFree.ViewModels.Base;
using SmokeFree.ViewModels.OnBoarding;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;

namespace SmokeFree.ViewModels.Test
{
    public class CreateTestViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public CreateTestViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public IAsyncValueCommand GOGO => new AsyncValueCommand(GOGOGO);

        private async ValueTask GOGOGO()
        {
            await this._navigationService.NavigateToAsync<OnBoardingViewModel>();
            await this._navigationService.RemoveBackStackAsync();
        }
    }
}
