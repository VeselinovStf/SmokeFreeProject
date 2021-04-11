using Moq;
using NUnit.Framework;
using Realms;
using SmokeFree;
using SmokeFree.Abstraction.Managers;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.ViewModels.Test;
using System;
using System.Threading.Tasks;

namespace UnderTestViewModelTests.UnitTests
{
    /// <summary>
    /// UnderTestViewModel InitializeAsync Tests
    /// </summary>
    public class InitializeAsync_Should
    {
        /// <summary>
        /// Initilize Async Should Set All Properties
        /// </summary>
        [Test]
        public async Task Initialize_Initial_State_Of_UnderTestViewModel_With_Values()
        {
            //Arrange
            var config = new InMemoryConfiguration("Initialize_Initial_State_Of_UnderTestViewModel_With_Values");
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
            var appLoggerServiceMock = new Mock<IAppLogger>();
            var dialogServiceMock = new Mock<IDialogService>();
            var notificationManagerMock = new Mock<INotificationManager>();

            var underTestViewModel = new UnderTestViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object,
                appLoggerServiceMock.Object,
                dialogServiceMock.Object,
                notificationManagerMock.Object
                );

            object parameter = new object();
            var currentlySmokedCount = 2;
            var timeSenceLastSmoke = new TimeSpan(1,1,1);
            var testLeftTime = new TimeSpan(1, 1, 1);
            var currentSmokeId = 3;
            var currentSmokeTime = new TimeSpan(1, 1, 1);

            var userTest = new Test()
            {
                UserId = Globals.UserId
            };

            realm.Write(() =>
            {
                realm.Add(userTest);
            });

            // Act
            await underTestViewModel.InitializeAsync(parameter);

            //Assert
            Assert.AreEqual(currentlySmokedCount,underTestViewModel.CurrentlySmokedCount,"CurrentlySmokedCount not equal");
            Assert.AreEqual(currentSmokeId, underTestViewModel.CurrentSmokeId, "CurrentSmokeId not equal");
            Assert.IsTrue(underTestViewModel.TimeSenceLastSmoke.Equals(timeSenceLastSmoke), "TimeSenceLastSmoke not equal");
            Assert.IsTrue(underTestViewModel.TestLeftTime.Equals(testLeftTime), "TestLeftTime not equal");
            Assert.IsTrue(underTestViewModel.CurrentSmokeTime.Equals(currentSmokeTime), "CurrentSmokeTime not equal");

        }
    }
}
