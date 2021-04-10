using Moq;
using NUnit.Framework;
using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Models.Views.OnBoarding;
using SmokeFree.Resx;
using SmokeFree.ViewModels.OnBoarding;
using System.Collections.Generic;
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

            LocalizationResourceManager.Current.CurrentCulture = new CultureInfo("uk");
            var resourceManager = ResourceManagerMock.GetResourceManager();
            var resourceViewTitle = resourceManager.GetString("OnBoardingViewTitle");
            var onBoardingViewTitle = onBoardingViewModel.ViewTitle;

            //Assert
            Assert.NotNull(resourceViewTitle);
            Assert.AreEqual(resourceViewTitle, onBoardingViewTitle);
        }

        /// <summary>
        /// Assign View Title On Creation
        /// </summary>
        [Test]
        public void OnBoardingItems_Are_Initialized_With_Correct_Items()
        {
            //Arrange
            var config = new InMemoryConfiguration("OnBoardingItems_Are_Initialized_With_Correct_Items");
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

            LocalizationResourceManager.Current.CurrentCulture = new CultureInfo("uk");
            var resourceManager = ResourceManagerMock.GetResourceManager();
            var onBoardingItemTitle1 = resourceManager.GetString("OnBoardingItemTitle1");
            var onBoardingItemText1 = resourceManager.GetString("OnBoardingItemText1");
            var onBoardingItemButton1 = resourceManager.GetString("OnBoardingItemButton1");
            var onBoardingItemImage1 = resourceManager.GetString("OnBoardingItemImage1");
            var onBoardingItemTitle2 = resourceManager.GetString("OnBoardingItemTitle2");
            var onBoardingItemText2 = resourceManager.GetString("OnBoardingItemText2");
            var onBoardingItemButton2 = resourceManager.GetString("OnBoardingItemButton2");
            var onBoardingItemImage2 = resourceManager.GetString("OnBoardingItemImage2");
            var onBoardingItemTitle3 = resourceManager.GetString("OnBoardingItemTitle3");
            var onBoardingItemText3 = resourceManager.GetString("OnBoardingItemText3");
            var onBoardingItemButton3 = resourceManager.GetString("OnBoardingItemButton3");
            var onBoardingItemImage3 = resourceManager.GetString("OnBoardingItemImage3");


            var staticOnBoardingItems = new List<OnBordingItem>()
            {
                new OnBordingItem()
                {
                    Image = onBoardingItemImage1,
                    Title = onBoardingItemTitle1,
                    Text = onBoardingItemText1,
                    ItemButtonText = onBoardingItemButton1
                },
                new OnBordingItem()
                {
                    Image = onBoardingItemImage2,
                    Title = onBoardingItemTitle2,
                    Text = onBoardingItemText2,
                    ItemButtonText = onBoardingItemButton2
                },
                new OnBordingItem()
                {
                    Image = onBoardingItemImage3,
                    Title = onBoardingItemTitle3,
                    Text = onBoardingItemText3,
                    ItemButtonText = onBoardingItemButton3
                }

            };

            Assert.NotNull(onBoardingViewModel.OnBoardingItems);

            var onboardingItems = onBoardingViewModel.OnBoardingItems;

            Assert.AreEqual(staticOnBoardingItems.Count, onboardingItems.Count);
            //Assert
            for (int i = 0; i < onboardingItems.Count; i++)
            {
                var itemUnderTest = onboardingItems[i];
                var item = staticOnBoardingItems[i];

                Assert.AreEqual(itemUnderTest.Title, item.Title);
                Assert.AreEqual(itemUnderTest.Text, item.Text);
                Assert.AreEqual(itemUnderTest.ItemButtonText, item.ItemButtonText);
                Assert.AreEqual(itemUnderTest.Image, item.Image);
            }

        }
    }
}
