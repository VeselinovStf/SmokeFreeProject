using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.ViewModels.Base;
using SmokeFree.ViewModels.Test;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;

namespace SmokeFree.ViewModels.OnBoarding
{
    public class OnBoardingViewModel : ViewModelBase
    {
        private readonly Realm _realm;

        public OnBoardingViewModel(
            Realm realm,
            INavigationService navigationService,
            IDateTimeWrapper dateTimeWrapper)
            : base(navigationService,dateTimeWrapper)
        {
            _realm = realm;
        }

        public IAsyncValueCommand GOGO => new AsyncValueCommand(GOGOGO);

        private async ValueTask GOGOGO()
        {
            await base._navigationService.NavigateToAsync<CreateTestViewModel>();

            //var config = new RealmConfiguration
            //{
            //    SchemaVersion = 3,
            //};

            //var _realm = Realm.GetInstance(config);
            //var tests = _realm.All<Test>().ToList();

            ////_realm.Write(() =>
            ////{
            ////    var entry = new Test { CreatedOn = DateTimeOffset.Now };
            ////    _realm.Add(entry);
            ////});

            //Debug.WriteLine("HERE");
        }
    }
}
