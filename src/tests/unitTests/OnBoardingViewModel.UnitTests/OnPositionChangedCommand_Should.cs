using Moq;
using NUnit.Framework;
using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.ViewModels.OnBoarding;
using System;
using System.Threading.Tasks;

namespace OnBoardingViewModelTests.UnitTests
{
    /// <summary>
    /// OnPositionChangedCommand Tests
    /// </summary>
    public class OnPositionChangedCommand_Should
    {
        /// <summary>
        /// Navigate to next view when user is move throug all of on boarding items
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task Change_SelectedOnBoardingItemsIndex_On_Call()
        {
            //Arrange
            var config = new InMemoryConfiguration("Change_SelectedOnBoardingItemsIndex_On_Call");
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
            var initialSelectedOnBoardingItemsIndex = onBoardingViewModel.SelectedOnBoardingItemsIndex;
            var newPosition = 2;

            //Act
            await onBoardingViewModel.InitializeAsync(initializeParameter);

            //Assert
            Assert.AreEqual(initialSelectedOnBoardingItemsIndex, 0);

            onBoardingViewModel.OnPositionChangedCommand
                .Execute(newPosition);

            Assert.AreEqual(newPosition, onBoardingViewModel.SelectedOnBoardingItemsIndex);
        }
    }
}
