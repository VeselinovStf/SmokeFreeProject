using Moq;
using NUnit.Framework;
using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Models.Views.Test;
using SmokeFree.Resx;
using SmokeFree.ViewModels.Test;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.CommunityToolkit.Helpers;

namespace CreateTestViewModelTests.UnitTests
{
    /// <summary>
    /// CreateTestViewModel Constructor Tests
    /// </summary>
    public class Ctor_Should
    {
        /// <summary>
        /// Creates Instance Of Class
        /// </summary>
        [Test]
        public void Create_Instance_Of_CreateTestViewModel_Successfully()
        {
            //Arrange
            var config = new InMemoryConfiguration("Create_Instance_Of_CreateTestViewModel_Successfully");
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
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

            //Assert
            Assert.NotNull(createTestViewModel);
        }

        /// <summary>
        /// Assign View Title On Creation
        /// </summary>
        [Test]
        public void Assign_ViewTitle_For_CreateTestViewModel()
        {
            //Arrange
            var config = new InMemoryConfiguration("Assign_ViewTitle_For_CreateTestViewModel");
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
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

            LocalizationResourceManager.Current.CurrentCulture = new CultureInfo("uk");
            var resourceManager = ResourceManagerMock.GetResourceManager();
            var resourceViewTitle = resourceManager.GetString("CreateTestViewTiitle");
            var onBoardingViewTitle = createTestViewModel.ViewTitle;

            //Assert
            Assert.NotNull(resourceViewTitle);
            Assert.AreEqual(resourceViewTitle, onBoardingViewTitle);
        }

        /// <summary>
        /// Initialize TestDurations Collection
        /// </summary>
        [Test]
        public void Initializes_TestTimeDurations_Not_Equal_Null()
        {
            //Arrange
            var config = new InMemoryConfiguration("Initializes_TestTimeDurations_Not_Equal_Null");
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
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

            //Assert
            Assert.NotNull(createTestViewModel.TestTimeDurations);
        }

        /// <summary>
        /// Assign Goal Date Time To Current Date Now
        /// </summary>
        [Test]
        public void Assign_GoalDateTime_To_Current_DateTime_Now()
        {
            //Arrange
            var config = new InMemoryConfiguration("Assign_GoalDateTime_To_Current_DateTime_Now");
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();

            var dateTimeNow = DateTime.Now;
            dateTimeWrapperMock.Setup(t => t.Now()).Returns(dateTimeNow);

            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();

            var createTestViewModel = new CreateTestViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object
                );

            //Assert
            Assert.True(createTestViewModel.GoalDateTime.Equals(dateTimeNow));
        }

        /// <summary>
        /// Assign Goal Date Time To Current Date Now
        /// </summary>
        [Test]
        public void InitiateTestTimeDurations_When_Is_Called()
        {
            //Arrange
            var config = new InMemoryConfiguration("InitiateTestTimeDurations_When_Is_Called");
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
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

            LocalizationResourceManager.Current.CurrentCulture = new CultureInfo("uk");
            var resourceManager = ResourceManagerMock.GetResourceManager();
            var singleDay = resourceManager.GetString("SingleDay");
            var pluralDay = resourceManager.GetString("PluralDay");

            var durations = new List<TestDurationItem>()
            {
                new TestDurationItem()
                {
                     DayValue = 1,
                     DisplayText =  string.Format("1 {0}", singleDay)
                },
                new TestDurationItem()
                {
                     DayValue = 2,
                     DisplayText =  string.Format("2 {0}", pluralDay)
                },
                new TestDurationItem()
                {
                     DayValue = 3,
                     DisplayText =  string.Format("3 {0}", pluralDay)
                },
                new TestDurationItem()
                {
                     DayValue = 4,
                     DisplayText =  string.Format("4 {0}", pluralDay)
                },
                new TestDurationItem()
                {
                     DayValue = 5,
                     DisplayText =  string.Format("5 {0}", pluralDay)
                }
            };

            //Assert
            Assert.NotNull(createTestViewModel.TestTimeDurations);

            var testTimeDurations = createTestViewModel.TestTimeDurations;

            Assert.AreEqual(durations.Count, testTimeDurations.Count);
            //Assert
            for (int i = 0; i < testTimeDurations.Count; i++)
            {
                var itemUnderTest = testTimeDurations[i];
                var item = durations[i];

                Assert.AreEqual(itemUnderTest.DayValue, item.DayValue);
                Assert.AreEqual(itemUnderTest.DisplayText, item.DisplayText);
            }
        }
    }
}