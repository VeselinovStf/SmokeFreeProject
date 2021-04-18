using Moq;
using NUnit.Framework;
using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.ViewModels.AppSettings;
using System;

namespace AppSettingsViewModelTests.UnitTests
{
    /// <summary>
    /// AppSettingsViewModel - NavigateBackCommand Tests
    /// </summary>
    public class NavigateBackCommand_Should
    {
        /// <summary>
        /// Creates Instance Of Class
        /// </summary>
        [Test]
        public void Navigate_Back()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            navigationServiceMock.Setup(e => e.BackToPreviousAsync());

            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();

            var appSettingsViewModel = new AppSettingsViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object
                );

            // Act
            appSettingsViewModel.NavigateBackCommand.Execute(new object());

            //Assert
            navigationServiceMock.Verify(e => e.BackToPreviousAsync(), Times.Once);

        }

    }
}
