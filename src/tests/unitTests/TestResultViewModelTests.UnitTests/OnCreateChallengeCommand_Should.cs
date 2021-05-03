using Moq;
using NUnit.Framework;
using Realms;
using SmokeFree.Abstraction.Services.Data.Test;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.ViewModels.Challenge;
using SmokeFree.ViewModels.Test;
using System;

namespace TestResultViewModelTests.UnitTests
{
    /// <summary>
    /// TestResultViewModel - OnCreateChallengeCommand Tests
    /// </summary>
    public class OnCreateChallengeCommand_Should
    {
        /// <summary>
        /// Navigate to CreateChallengeViewModel
        /// </summary>
        [Test]
        public void Navigate_To_CreateChallengeViewModel_Successfully()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            navigationServiceMock.Setup(e => e.NavigateToAsync<CreateChallengeViewModel>());

            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();
            var testCalculationServiceMock = new Mock<ITestCalculationService>();

            var testResultViewModel = new TestResultViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object,
                testCalculationServiceMock.Object);

            // Act
            testResultViewModel.OnCreateChallengeCommand.Execute(new object());

            //Assert
            navigationServiceMock.Verify(e => e.NavigateToAsync<CreateChallengeViewModel>(), Times.Once);

        }
    }
}
