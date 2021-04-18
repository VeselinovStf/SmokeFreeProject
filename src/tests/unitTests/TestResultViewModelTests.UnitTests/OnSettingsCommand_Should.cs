using Moq;
using NUnit.Framework;
using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.ViewModels.AppSettings;
using SmokeFree.ViewModels.Test;
using System;

namespace TestResultViewModelTests.UnitTests
{
    /// <summary>
    /// TestResultViewModel - OnSettingsCommand Tests
    /// </summary>
    public class OnSettingsCommand_Should
    {
        /// <summary>
        /// Navigate to AppSettingsViewModel
        /// </summary>
        [Test]
        public void Navigate_To_AppSettingsViewModel_Successfully()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            navigationServiceMock.Setup(e => e.NavigateToAsync<AppSettingsViewModel>());

            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();

            var testResultViewModel = new TestResultViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object);

            // Act
            testResultViewModel.OnSettingsCommand.Execute(new object());

            //Assert
            navigationServiceMock.Verify(e => e.NavigateToAsync<AppSettingsViewModel>(), Times.Once);

        }
    }
}
