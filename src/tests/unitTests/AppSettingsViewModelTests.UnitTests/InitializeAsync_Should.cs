using Moq;
using NUnit.Framework;
using Realms;
using SmokeFree;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.DeviceUtilities;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.ViewModels.AppSettings;
using SmokeFree.ViewModels.ErrorAndEmpty;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AppSettingsViewModelTests.UnitTests
{
    /// <summary>
    /// AppSettingsViewModel InitializeAsync Tests
    /// </summary>
    public class InitializeAsync_Should
    {
        /// <summary>
       /// Sets AppUser Property 
        /// </summary>
        [Test]
        public async Task Sets_User_Property_Successfully_When_User_Exists()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var dateTimeOfCreation = DateTime.Now;
            var globalUserId = Globals.UserId;

            var navigationServiceMock = new Mock<INavigationService>();
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();
            var preferencesServiceMock = new Mock<IAppPreferencesService>();
            var networkConnectivityServiceMock = new Mock<INetworkConnectionService>();
            var localLogUtilityServiceMock = new Mock<ILocalLogUtility>();
            var deviceEmailSenderServiceMock = new Mock<IDeviceEmailSender>();

            var user = new User()
            {
                CreatedOn = dateTimeOfCreation,
                Id = globalUserId
            };

            var onBoardingViewModel = new AppSettingsViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object,
                preferencesServiceMock.Object,
                 networkConnectivityServiceMock.Object,
                localLogUtilityServiceMock.Object,
                deviceEmailSenderServiceMock.Object
                );

            object initializeParameter = new object();

            realm.Write(() =>
            {
                realm.Add(user);
            });

            //Act
            await onBoardingViewModel.InitializeAsync(initializeParameter);
            var realmUser = realm.All<User>()
                .ToList();

            //Assert
            Assert.AreEqual(realmUser.Count, 1);

            var currentPresentUser = realmUser.FirstOrDefault
                (u => u.Id == globalUserId);

            Assert.NotNull(currentPresentUser);
            Assert.True(currentPresentUser.CreatedOn.Equals(dateTimeOfCreation));
        }



        /// <summary>
        /// Sets ViewModel NotificationSwitch property equal to User NotificationState   
        /// </summary>
        [Test]
        public async Task Sets_NotificationSwitch_Property_If_User_Is_Allready_Created_In_DB()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var dateTimeOfCreation = DateTime.Now;
            var globalUserId = Globals.UserId;

            var navigationServiceMock = new Mock<INavigationService>();
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();
            var preferencesServiceMock = new Mock<IAppPreferencesService>();
            var networkConnectivityServiceMock = new Mock<INetworkConnectionService>();
            var localLogUtilityServiceMock = new Mock<ILocalLogUtility>();
            var deviceEmailSenderServiceMock = new Mock<IDeviceEmailSender>();

            var user = new User()
            {
                CreatedOn = dateTimeOfCreation,
                Id = globalUserId,
                NotificationState = true
            };

            var onBoardingViewModel = new AppSettingsViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object,
                preferencesServiceMock.Object,
                 networkConnectivityServiceMock.Object,
                localLogUtilityServiceMock.Object,
                deviceEmailSenderServiceMock.Object
                );

            object initializeParameter = new object();

            realm.Write(() =>
            {
                realm.Add(user);
            });

            //Act
            await onBoardingViewModel.InitializeAsync(initializeParameter);
            var viewModelAppUser = onBoardingViewModel.AppUser;

            //Assert
            Assert.NotNull(viewModelAppUser);
            Assert.True(viewModelAppUser.NotificationState == user.NotificationState);
        }

        /// <summary>
        /// Notifies If User Is Not Found In Db
        /// </summary>
        [Test]
        public async Task Navigate_To_SomethingWentWrongViewModel_If_User_Is_Not_Found()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            navigationServiceMock.Setup(e => e.NavigateToAsync<SomethingWentWrongViewModel>());

            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();
            var preferencesServiceMock = new Mock<IAppPreferencesService>();
            var networkConnectivityServiceMock = new Mock<INetworkConnectionService>();
            var localLogUtilityServiceMock = new Mock<ILocalLogUtility>();
            var deviceEmailSenderServiceMock = new Mock<IDeviceEmailSender>();

            var onBoardingViewModel = new AppSettingsViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object,
                preferencesServiceMock.Object,
                 networkConnectivityServiceMock.Object,
                localLogUtilityServiceMock.Object,
                deviceEmailSenderServiceMock.Object
                );

            object initializeParameter = new object();

            //Act
            await onBoardingViewModel.InitializeAsync(initializeParameter);

            //Assert
            navigationServiceMock.Verify(e => e.NavigateToAsync<SomethingWentWrongViewModel>(), Times.Once);

        }
    }
}
