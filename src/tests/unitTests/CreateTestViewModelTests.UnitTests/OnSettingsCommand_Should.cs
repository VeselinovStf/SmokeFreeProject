using Moq;
using NUnit.Framework;
using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.ViewModels.AppSettings;
using SmokeFree.ViewModels.Test;

namespace CreateTestViewModelTests.UnitTests
{
    /// <summary>
    /// OnSettingsCommand Tests
    /// </summary>
    public class OnSettingsCommand_Should
    {
        /// <summary>
        /// Navigate to app settings when command is executed
        /// </summary>
        [Test]
        public void Navigate_To_AppSettings_View_From_CreateTestViewModel_When_Executed()
        {
            //Arrange
            var config = new InMemoryConfiguration("Navigate_To_AppSettings_View_From_CreateTestViewModel_When_Executed");
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            navigationServiceMock.Setup(e => e.NavigateToAsync<AppSettingsViewModel>());

            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();

            var createTestViewModel = new CreateTestViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object
                );

            var call = createTestViewModel.OnSettingsCommand.ExecuteAsync();

            //Assert
            navigationServiceMock.Verify(e => e.NavigateToAsync<AppSettingsViewModel>(), Times.Once);
        }
    }
}
