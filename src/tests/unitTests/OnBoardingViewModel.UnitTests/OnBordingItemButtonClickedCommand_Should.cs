using Moq;
using NUnit.Framework;
using Realms;
using SmokeFree;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.ViewModels.Base;
using SmokeFree.ViewModels.OnBoarding;
using System;
using System.Threading.Tasks;

namespace OnBoardingViewModelTests.UnitTests
{
    /// <summary>
    /// OnBordingItemButtonClickedCommand Tests
    /// </summary>
    public class OnBordingItemButtonClickedCommand_Should
    {
        /// <summary>
        /// Initial UserStatus - Navigate to next view when user is move throug all of on boarding items
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task Initial_Navigate_To_Next_View_If_Is_Selected_Last_Carousel_Index()
        {
            //Arrange
            var config = new InMemoryConfiguration("Initial_Navigate_To_Next_View_If_Is_Selected_Last_Carousel_Index");
            var realm = Realm.GetInstance(config);

            var dateTimeOfCreation = DateTime.Now;

            var navigationServiceMock = new Mock<INavigationService>();
            navigationServiceMock.Setup(e => e.NavigateToAsync<ViewModelBase>());

            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();

            var onBoardingViewModel = new OnBoardingViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object
                );

            object initializeParameter = new object();

            var globalUserId = Globals.UserId;
            var newUser = new User()
            {
                Id = globalUserId,
                UserState = UserStates.Initial.ToString()
            };

            realm.Write(() =>
            {
                realm.Add(newUser);
            });

            //Act
            await onBoardingViewModel.InitializeAsync(initializeParameter);

            onBoardingViewModel.SelectedOnBoardingItemsIndex = onBoardingViewModel.OnBoardingItems.Count - 1;

            //Assert
            var commandResult = onBoardingViewModel.OnBordingItemButtonClickedCommand.ExecuteAsync().ConfigureAwait(false);
            navigationServiceMock.Verify(e => e.NavigateToAsync<ViewModelBase>(), Times.Once);
        }

        /// <summary>
        /// Saves New User State before navigating to next view
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task Changes_Initial_User_State_To_CompletedOnBoarding_Before_Navigation_To_Next()
        {
            //Arrange
            var config = new InMemoryConfiguration("Changes_User_State_To_CompletedOnBoarding_Before_Navigation_To_Next");
            var realm = Realm.GetInstance(config);

            var dateTimeOfCreation = DateTime.Now;

            var navigationServiceMock = new Mock<INavigationService>();
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();

            var onBoardingViewModel = new OnBoardingViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object
                );

            object initializeParameter = new object();

            var globalUserId = Globals.UserId;
            var newUser = new User()
            {
                Id = globalUserId,
                UserState = UserStates.Initial.ToString()
            };

            var newState = UserStates.CompletedOnBoarding.ToString();

            realm.Write(() =>
            {
                realm.Add(newUser);
            });

            //Act
            await onBoardingViewModel.InitializeAsync(initializeParameter);

            onBoardingViewModel.SelectedOnBoardingItemsIndex = onBoardingViewModel.OnBoardingItems.Count - 1;

            //Assert
            var commandResult = onBoardingViewModel.OnBordingItemButtonClickedCommand.ExecuteAsync().ConfigureAwait(false);

            var viewModelAppUser = onBoardingViewModel.AppUser;

            Assert.AreEqual(newState, viewModelAppUser.UserState);

            var dbSavedUser = realm.Find<User>(globalUserId);

            Assert.AreEqual(dbSavedUser.Id, viewModelAppUser.Id);
            Assert.AreEqual(dbSavedUser.UserState, viewModelAppUser.UserState);
        }

        /// <summary>
        /// UserStatus CreateTestFirstRun - Navigate to next view when user is move throug all of on boarding items
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task Navigate_To_Next_View_If_Is_Selected_Last_Carousel_Index()
        {
            //Arrange
            var config = new InMemoryConfiguration("Navigate_To_Next_View_If_Is_Selected_Last_Carousel_Index");
            var realm = Realm.GetInstance(config);

            var dateTimeOfCreation = DateTime.Now;

            var navigationServiceMock = new Mock<INavigationService>();
            navigationServiceMock.Setup(e => e.InitializeAsync());

            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();

            var onBoardingViewModel = new OnBoardingViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object
                );

            object initializeParameter = new object();

            var globalUserId = Globals.UserId;
            var newUser = new User()
            {
                Id = globalUserId,
                UserState = UserStates.CreateTestFirstRun.ToString()
            };

            realm.Write(() =>
            {
                realm.Add(newUser);
            });

            //Act
            await onBoardingViewModel.InitializeAsync(initializeParameter);

            onBoardingViewModel.SelectedOnBoardingItemsIndex = onBoardingViewModel.OnBoardingItems.Count - 1;

            //Assert
            var commandResult = onBoardingViewModel.OnBordingItemButtonClickedCommand.ExecuteAsync().ConfigureAwait(false);
            navigationServiceMock.Verify(e => e.InitializeAsync(), Times.Once);
        }

    }
}
