using Moq;
using NUnit.Framework;
using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Resx;
using SmokeFree.ViewModels.OnBoarding;
using System.Globalization;
using Xamarin.CommunityToolkit.Helpers;

namespace OnBoardingViewModelTests.UnitTests
{
    /// <summary>
    ///  OnBoardingViewModel ctor Tests
    /// </summary>
    public class Ctor_Should
    {
        /// <summary>
        /// Creates Instance Of Class
        /// </summary>
        [Test]
        public void Create_Instance_Successfully()
        {
            //Arrange
            var config = new InMemoryConfiguration("Create_Instance_Successfully");
            var realm = Realm.GetInstance(config);

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

            //Assert
            Assert.NotNull(onBoardingViewModel);
        }

        /// <summary>
        /// Assign View Title On Creation
        /// </summary>
        [Test]
        public void Assign_ViewTitle()
        {
            //Arrange
            var config = new InMemoryConfiguration("Assign_ViewTitle");
            var realm = Realm.GetInstance(config);

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

            //LocalizationResourceManager.Current.CurrentCulture = new CultureInfo("uk");
            var resourceManager = ResourceManagerMock.GetResourceManager();
            var resourceViewTitle = resourceManager.GetString("OnBoardingViewTitle");
            var onBoardingViewTitle = onBoardingViewModel.ViewTitle;

            //Assert
            Assert.NotNull(resourceViewTitle);
            Assert.AreEqual(resourceViewTitle, onBoardingViewTitle);
        }
    }
}
