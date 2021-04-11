using Moq;
using NUnit.Framework;
using Realms;
using SmokeFree;
using SmokeFree.Abstraction.Managers;
using SmokeFree.Abstraction.Services.Data.Test;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.Resx;
using SmokeFree.ViewModels.Test;
using System;
using System.Globalization;
using Xamarin.CommunityToolkit.Helpers;

namespace UnderTestViewModelTests.UnitTests
{
    /// <summary>
    /// UnderTestViewModel Ctor Tests
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
            var config = new InMemoryConfiguration("Create_Instance_Of_UnderTestViewModel_Successfully");
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();
            var notificationManagerMock = new Mock<INotificationManager>();
            var testCalculationServiceMock = new Mock<ITestCalculationService>();

            // Act
            var underTestViewModel = new UnderTestViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object,
                notificationManagerMock.Object,
                testCalculationServiceMock.Object
                );

            //Assert
            Assert.NotNull(underTestViewModel);
        }

        /// <summary>
        /// Assign View Title On Creation
        /// </summary>
        [Test]
        public void Assign_ViewTitle_For_UnderTestViewModel()
        {
            //Arrange
            var config = new InMemoryConfiguration("Assign_ViewTitle_For_UnderTestViewModel");
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();
            var notificationManagerMock = new Mock<INotificationManager>();
            var testCalculationServiceMock = new Mock<ITestCalculationService>();


            LocalizationResourceManager.Current.CurrentCulture = new CultureInfo("uk");
            var resourceManager = ResourceManagerMock.GetResourceManager();
            var resourceViewTitle = resourceManager.GetString("UnderTestViewTiitle");
           

            // Act
            var underTestViewModel = new UnderTestViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object,
                notificationManagerMock.Object,
                testCalculationServiceMock.Object
                );

            var underTestViewTitle = underTestViewModel.ViewTitle;

            //Assert
            Assert.NotNull(resourceViewTitle);
            Assert.AreEqual(resourceViewTitle, underTestViewTitle);
        }

        /// <summary>
        /// Initializes NotificationReceived event when ctor is called and user is allowing notifications
        /// </summary>
        [Test]
        public void Initializes_Test_Completition_Notification_When_User_Allowes_Notifications()
        {
            //Arrange
            var config = new InMemoryConfiguration("Initializes_Test_Completition_Notification_When_User_Allowes_Notifications");
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();
            var testCalculationServiceMock = new Mock<ITestCalculationService>();

            var notificationManagerMock = new Mock<INotificationManager>();
            notificationManagerMock.Raise(e => e.NotificationReceived += (sender, args) => { });
           
            var user = new User()
            {
                Id = Globals.UserId,
                NotificationState = true
            };

            realm.Write(() =>
            {
                realm.Add(user);
            });

            // Act
            var underTestViewModel = new UnderTestViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object,
                notificationManagerMock.Object,
                testCalculationServiceMock.Object
                );

            //Assert
            notificationManagerMock.VerifyAdd(m => m.NotificationReceived += It.IsAny<EventHandler>(), Times.Exactly(1));
        }

        /// <summary>
        /// Not Initializes NotificationReceived event when ctor is called and user is not allowing notifications
        /// </summary>
        [Test]
        public void Not_Initializes_Test_Completition_Notification_When_User_Not_Allowed_Notifications()
        {
            //Arrange
            var config = new InMemoryConfiguration("Not_Initializes_Test_Completition_Notification_When_User_Not_Allowed_Notifications");
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();
            var testCalculationServiceMock = new Mock<ITestCalculationService>();

            var notificationManagerMock = new Mock<INotificationManager>();
            notificationManagerMock.Raise(e => e.NotificationReceived += (sender, args) => { });

            var user = new User()
            {
                Id = Globals.UserId,
                NotificationState = false
            };

            realm.Write(() =>
            {
                realm.Add(user);
            });

            // Act
            var underTestViewModel = new UnderTestViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object,
                notificationManagerMock.Object,
                testCalculationServiceMock.Object
                );

            //Assert
            notificationManagerMock.VerifyAdd(m => m.NotificationReceived += It.IsAny<EventHandler>(), Times.Exactly(0));
        }

    }
}