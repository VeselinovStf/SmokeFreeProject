using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Bootstrap;
using SmokeFree.Data.Models;
using SmokeFree.Resx;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;


namespace SmokeFree
{
    public partial class App : Application
    {
        public App()
        {
            InitializeAppContainer();

            // TODO: AA: Remove in Release
            if (Globals.MockRun)
            {
                DevelopmentDatabaseClearing();
                TestPackToChallenge();
            }

            InitializeComponent();

            InitializeLocalization();

            InitializeNavigation();
        }

        private void TestPackToChallenge()
        {
            // start = 4 * 60 + 4 * 5
            var dateTime = DateTime.Now.AddMinutes(-275);
            var testId = "Test_ID";
            var testStartTime = dateTime;

            var challengeId = "Challenge_ID";
            var challengeDays = 5;
            var challengeGoalTime = dateTime.AddDays(challengeDays);
            var testDays = 1;
            var smokesDistanceInMinutes = 60;
            var oneSmokeTimeInMinutes = 7;

            var testSmokes = 5;
            var totalSmokeTime = oneSmokeTimeInMinutes * testSmokes;

            var testTimeElapsed = dateTime;

            var smoke1start = dateTime;
            var smoke1end = smoke1start.AddMinutes(oneSmokeTimeInMinutes);

            var smoke2start = smoke1end.AddMinutes(smokesDistanceInMinutes);
            var smoke2end = smoke2start.AddMinutes(oneSmokeTimeInMinutes);

            var smoke3start = smoke2end.AddMinutes(smokesDistanceInMinutes);
            var smoke3end = smoke3start.AddMinutes(oneSmokeTimeInMinutes);

            var smoke4start = smoke3end.AddMinutes(smokesDistanceInMinutes);
            var smoke4end = smoke4start.AddMinutes(oneSmokeTimeInMinutes);

            var smoke5start = smoke4end.AddMinutes(smokesDistanceInMinutes);
            var smoke5end = smoke5start.AddMinutes(oneSmokeTimeInMinutes);


            var testEndTime = ((testSmokes - 1) * smokesDistanceInMinutes) + totalSmokeTime;
            var endTestTime = dateTime.AddMinutes(testEndTime);

            var smoke1 = new Smoke()
            {
                StartSmokeTime = smoke1start,
                EndSmokeTime = smoke1end
            };
            var smoke2 = new Smoke()
            {
                StartSmokeTime = smoke2start,
                EndSmokeTime = smoke2end
            };
            var smoke3 = new Smoke()
            {
                StartSmokeTime = smoke3start,
                EndSmokeTime = smoke3end
            };
            var smoke4 = new Smoke()
            {
                StartSmokeTime = smoke4start,
                EndSmokeTime = smoke4end
            };
            var smoke5 = new Smoke()
            {
                StartSmokeTime = smoke5start,
                EndSmokeTime = smoke5end
            };

            var test = new Test()
            {
                Id = testId,
                UserId = Globals.UserId,
                TestStartDate = testStartTime,
                TestEndDate = endTestTime
            };

            var userChallenge = new Challenge()
            {
                Id = challengeId,
                UserId = Globals.UserId,
                GoalCompletitionTime = challengeGoalTime
            };

            var user = new User()
            {
                Id = Globals.UserId,
                UserSmokeStatuses = UserSmokeStatuses.Bad.ToString(),
                UserState = UserStates.UserUnderTesting.ToString(),
                Localozation = "en",
                TestId = testId
            };

            test.SmokedCigaresUnderTest.Add(smoke1);
            test.SmokedCigaresUnderTest.Add(smoke2);
            test.SmokedCigaresUnderTest.Add(smoke3);
            test.SmokedCigaresUnderTest.Add(smoke4);
            test.SmokedCigaresUnderTest.Add(smoke5);

            user.Challenges.Add(userChallenge);
            user.Tests.Add(test);

            var realm = AppContainer.Resolve<Realm>();

            realm.Write(() =>
            {
                realm.Add(user);
            });

        }

        private void InitializeLocalization()
        {
            LocalizationResourceManager.Current.PropertyChanged += (sender, e)
                => AppResources.Culture = LocalizationResourceManager.Current.CurrentCulture;


            LocalizationResourceManager.Current.Init(AppResources.ResourceManager);

            var appPreferences = AppContainer.Resolve<IAppPreferencesService>();
            var appLanguage = appPreferences.LanguageValue;

            // TODO: C: Change from settings in the future and initiate current culture
            LocalizationResourceManager.Current.CurrentCulture = appLanguage == string.Empty ? CultureInfo.CurrentCulture : new CultureInfo(appLanguage);
        }

        private void DevelopmentDatabaseClearing()
        {
            var realm = AppContainer.Resolve<Realm>();

            realm.Write(() =>
            {
                realm.RemoveAll<User>();
                realm.RemoveAll<Test>();
                realm.RemoveAll<Challenge>();
                realm.RemoveAll<TestResult>();
                realm.RemoveAll<Smoke>();
                realm.RemoveAll<DayChallengeSmoke>();
            });
        }

        private Task InitializeNavigation()
        {
            var navigationService = AppContainer.Resolve<INavigationService>();
            return navigationService.InitializeAsync();
        }

        private void InitializeAppContainer()
        {
            AppContainer.RegisterDependencies();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
