using Moq;
using NUnit.Framework;
using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Resx;
using SmokeFree.ViewModels.Test;
using System;
using System.Globalization;
using Xamarin.CommunityToolkit.Helpers;

namespace TestResultViewModelTests.UnitTests
{
    /// <summary>
    /// TestResultViewModel - Ctor Tests
    /// </summary>
    public class Ctor_Should
    {
        /// <summary>
        /// Creates Instance Of Class
        /// </summary>
        [Test]
        public void Create_Instance_Of_UnderTestViewModel_Successfully()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();

            // Act
            var testResultViewModel = new TestResultViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object);

            //Assert
            Assert.NotNull(testResultViewModel);
        }

        /// <summary>
        /// Assign View Title On Creation
        /// </summary>
        [Test]
        public void Assign_ViewTitle_For_TestResultViewModel()
        {
            //Arrange
            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();

            LocalizationResourceManager.Current.CurrentCulture = new CultureInfo("uk");
            var resourceManager = ResourceManagerMock.GetResourceManager();
            var resourceViewTitle = resourceManager.GetString("TestResultViewModelTiitle");


            /// Act
            var testResultViewModel = new TestResultViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object);

            var underTestViewTitle = testResultViewModel.ViewTitle;

            //Assert
            Assert.NotNull(resourceViewTitle);
            Assert.AreEqual(resourceViewTitle, underTestViewTitle);
        }
    }
}