//using Moq;
//using NUnit.Framework;
//using Realms;
//using SmokeFree;
//using SmokeFree.Abstraction.Managers;
//using SmokeFree.Abstraction.Services.Data.Test;
//using SmokeFree.Abstraction.Services.General;
//using SmokeFree.Abstraction.Utility.DeviceUtilities;
//using SmokeFree.Abstraction.Utility.Logging;
//using SmokeFree.Abstraction.Utility.Wrappers;
//using SmokeFree.Data.Models;
//using SmokeFree.ViewModels.ErrorAndEmpty;
//using SmokeFree.ViewModels.Test;
//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace UnderTestViewModelTests.UnitTests
//{
//    /// <summary>
//    /// UnderTestViewModel - StartSmokingCommand Tests
//    /// </summary>
//    public class StartSmokingCommand_Should
//    {
//        /// <summary>
//        /// When User Starts to smoke, should add new smoke to db
//        /// </summary>
//        [Test]
//        public void Add_New_Smoke_To_Db()
//        {
//            //Arrange
//            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
//            var realm = Realm.GetInstance(config);

//            var dateTime = DateTime.Now;
//            var isSmoking = true;
//            var navigationServiceMock = new Mock<INavigationService>();

//            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
//            dateTimeWrapperMock.Setup(e => e.Now()).Returns(dateTime);

//            var appLoggerServiceMock = new Mock<IAppLogger>();

//            var dialogServiceMock = new Mock<IDialogService>();
//            dialogServiceMock.Setup(e => e.ConfirmAsync(
//                It.IsAny<string>(),
//                It.IsAny<string>(),
//                It.IsAny<string>(),
//                It.IsAny<string>()))
//                    .Returns(Task.FromResult(true));

//            
//            var testCalculationServiceMock = new Mock<ITestCalculationService>();
//            var deviceTimerMock = new Mock<IDeviceTimer>();

//            var underTestViewModel = new UnderTestViewModel(
//                realm,
//                navigationServiceMock.Object,
//                dateTimeWrapperMock.Object,
//                appLoggerServiceMock.Object,
//                dialogServiceMock.Object,
//                
//                testCalculationServiceMock.Object,
//                deviceTimerMock.Object
//                );

//            var userId = Globals.UserId;
//            var testId = "TEST_ID";

//            var user = new User()
//            {
//                Id = userId,
//                TestId = testId
//            };

//            var userTest = new Test()
//            {
//                Id = testId,
//                UserId = userId
//            };

//            realm.Write(() =>
//            {
//                realm.Add(user);

//                user.Tests.Add(userTest);
//            });

//            // Act
//            underTestViewModel.StartSmokingCommand.Execute(isSmoking);

//            //Assert
//            var userUpdatedTest = realm.Find<Test>(testId);

//            Assert.NotNull(userUpdatedTest);

//            var smokedCigaresUnderTest = userUpdatedTest.SmokedCigaresUnderTest;

//            Assert.True(smokedCigaresUnderTest.Count == 1);

//            var smokedCigare = smokedCigaresUnderTest.FirstOrDefault();

//            Assert.True(smokedCigare.TestId.Equals(testId));
//            Assert.True(smokedCigare.CreatedOn.LocalDateTime.Equals(dateTime));
//            Assert.True(smokedCigare.StartSmokeTime.LocalDateTime.Equals(dateTime));
//            Assert.True(smokedCigare.EndSmokeTime.Equals(new DateTimeOffset()));
//        }

//        /// <summary>
//        /// User Smoke Confirmation is false
//        /// </summary>
//        [Test]
//        public void Nothing_Happens_If_User_Is_Not_Confirming_That_Is_Smoking_Now()
//        {
//            //Arrange
//            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
//            var realm = Realm.GetInstance(config);

//            var dateTime = DateTime.Now;
//            var isSmoking = true;
//            var navigationServiceMock = new Mock<INavigationService>();

//            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
//            dateTimeWrapperMock.Setup(e => e.Now()).Returns(dateTime);

//            var appLoggerServiceMock = new Mock<IAppLogger>();

//            var dialogServiceMock = new Mock<IDialogService>();
//            dialogServiceMock.Setup(e => e.ConfirmAsync(
//                It.IsAny<string>(),
//                It.IsAny<string>(),
//                It.IsAny<string>(),
//                It.IsAny<string>()))
//                    .Returns(Task.FromResult(false));

//            
//            var testCalculationServiceMock = new Mock<ITestCalculationService>();
//            var deviceTimerMock = new Mock<IDeviceTimer>();

//            var underTestViewModel = new UnderTestViewModel(
//                realm,
//                navigationServiceMock.Object,
//                dateTimeWrapperMock.Object,
//                appLoggerServiceMock.Object,
//                dialogServiceMock.Object,
//                
//                testCalculationServiceMock.Object,
//                deviceTimerMock.Object
//                );

//            var userId = Globals.UserId;
//            var testId = "TEST_ID";

//            var user = new User()
//            {
//                Id = userId,
//                TestId = testId
//            };

//            var userTest = new Test()
//            {
//                Id = testId,
//                UserId = userId
//            };

//            realm.Write(() =>
//            {
//                realm.Add(user);

//                user.Tests.Add(userTest);
//            });

//            // Act
//            underTestViewModel.StartSmokingCommand.Execute(isSmoking);

//            //Assert
//            var userUpdatedTest = realm.Find<Test>(testId);

//            Assert.NotNull(userUpdatedTest);

//            var smokedCigaresUnderTest = userUpdatedTest.SmokedCigaresUnderTest;

//            Assert.True(smokedCigaresUnderTest.Count == 0);
//        }

//        /// <summary>
//        /// Update vm props when new smoke is added
//        /// </summary>
//        [Test]
//        public void Set_Up_ViewModel_Properties_When_Adding_New_Smoke_Is_Done()
//        {
//            //Arrange
//            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
//            var realm = Realm.GetInstance(config);

//            var dateTime = DateTime.Now;
//            var isSmoking = true;
//            var navigationServiceMock = new Mock<INavigationService>();

//            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
//            dateTimeWrapperMock.Setup(e => e.Now()).Returns(dateTime);

//            var appLoggerServiceMock = new Mock<IAppLogger>();

//            var dialogServiceMock = new Mock<IDialogService>();
//            dialogServiceMock.Setup(e => e.ConfirmAsync(
//                It.IsAny<string>(),
//                It.IsAny<string>(),
//                It.IsAny<string>(),
//                It.IsAny<string>()))
//                    .Returns(Task.FromResult(true));

//            
//            var testCalculationServiceMock = new Mock<ITestCalculationService>();
//            var deviceTimerMock = new Mock<IDeviceTimer>();

//            var underTestViewModel = new UnderTestViewModel(
//                realm,
//                navigationServiceMock.Object,
//                dateTimeWrapperMock.Object,
//                appLoggerServiceMock.Object,
//                dialogServiceMock.Object,
//                
//                testCalculationServiceMock.Object,
//                deviceTimerMock.Object
//                );

//            var userId = Globals.UserId;
//            var testId = "TEST_ID";

//            var user = new User()
//            {
//                Id = userId,
//                TestId = testId
//            };

//            var userTest = new Test()
//            {
//                Id = testId,
//                UserId = userId
//            };

//            realm.Write(() =>
//            {
//                realm.Add(user);

//                user.Tests.Add(userTest);
//            });

//            // Act
//            underTestViewModel.StartSmokingCommand.Execute(isSmoking);

//            //Assert
//            var userUpdatedTest = realm.Find<Test>(testId);

//            Assert.NotNull(userUpdatedTest);

//            var smokedCigaresUnderTest = userUpdatedTest.SmokedCigaresUnderTest;

//            Assert.True(smokedCigaresUnderTest.Count == 1);

//            var smokedCigare = smokedCigaresUnderTest.FirstOrDefault();

//            Assert.IsTrue(underTestViewModel.CurrentSmokeId.Equals(smokedCigare.Id));
//            //Assert.IsTrue(underTestViewModel.IsSmoking);
//            //Assert.IsTrue(underTestViewModel.CurrentSmokeTime.Equals(new TimeSpan(0, 0, 0)));

//        }

//        /// <summary>
//        /// Starts StartSmokingTimer
//        /// </summary>
//        [Test]
//        public void Start_Smoke_Timer_When_Adding_New_Smoke_Is_Completed()
//        {
//            //Arrange
//            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
//            var realm = Realm.GetInstance(config);

//            var dateTime = DateTime.Now;
//            var isSmoking = true;
//            var navigationServiceMock = new Mock<INavigationService>();

//            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
//            dateTimeWrapperMock.Setup(e => e.Now()).Returns(dateTime);

//            var appLoggerServiceMock = new Mock<IAppLogger>();

//            var dialogServiceMock = new Mock<IDialogService>();
//            dialogServiceMock.Setup(e => e.ConfirmAsync(
//                It.IsAny<string>(),
//                It.IsAny<string>(),
//                It.IsAny<string>(),
//                It.IsAny<string>()))
//                    .Returns(Task.FromResult(true));

//            
//            var testCalculationServiceMock = new Mock<ITestCalculationService>();
//            var deviceTimerMock = new Mock<IDeviceTimer>();
//            deviceTimerMock.Setup(e =>
//                e.Start(It.IsAny<Func<Task<bool>>>(), It.IsAny<CancellationTokenSource>()));

//            var underTestViewModel = new UnderTestViewModel(
//                realm,
//                navigationServiceMock.Object,
//                dateTimeWrapperMock.Object,
//                appLoggerServiceMock.Object,
//                dialogServiceMock.Object,
//                
//                testCalculationServiceMock.Object,
//                deviceTimerMock.Object
//                );

//            var userId = Globals.UserId;
//            var testId = "TEST_ID";

//            var user = new User()
//            {
//                Id = userId,
//                TestId = testId
//            };

//            var userTest = new Test()
//            {
//                Id = testId,
//                UserId = userId
//            };

//            realm.Write(() =>
//            {
//                realm.Add(user);

//                user.Tests.Add(userTest);
//            });

//            // Act
//            underTestViewModel.StartSmokingCommand.Execute(isSmoking);

//            //Assert
//            deviceTimerMock.Verify(e => e.Start(It.IsAny<Func<Task<bool>>>(), It.IsAny<CancellationTokenSource>()), Times.Once);
//        }

//        /// <summary>
//        /// App State - SmokedCigaresUnderTest should contain only one started smoke
//        /// </summary>
//        [Test]
//        public void Log_Critical_When_SmokedCigaresUnderTest_Collection_Contains_More_Then_One_Started_Smoke()
//        {
//            //Arrange
//            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
//            var realm = Realm.GetInstance(config);

//            var dateTime = DateTime.Now;
//            var isSmoking = true;
//            var navigationServiceMock = new Mock<INavigationService>();

//            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
//            dateTimeWrapperMock.Setup(e => e.Now()).Returns(dateTime);

//            var appLoggerServiceMock = new Mock<IAppLogger>();
//            appLoggerServiceMock.Setup(e => e.LogCritical(It.IsAny<string>(), It.IsAny<string>()));

//            var dialogServiceMock = new Mock<IDialogService>();
//            dialogServiceMock.Setup(e => e.ConfirmAsync(
//                It.IsAny<string>(),
//                It.IsAny<string>(),
//                It.IsAny<string>(),
//                It.IsAny<string>()))
//                    .Returns(Task.FromResult(true));

//            
//            var testCalculationServiceMock = new Mock<ITestCalculationService>();
//            var deviceTimerMock = new Mock<IDeviceTimer>();


//            var underTestViewModel = new UnderTestViewModel(
//                realm,
//                navigationServiceMock.Object,
//                dateTimeWrapperMock.Object,
//                appLoggerServiceMock.Object,
//                dialogServiceMock.Object,
//                
//                testCalculationServiceMock.Object,
//                deviceTimerMock.Object
//                );

//            var userId = Globals.UserId;
//            var testId = "TEST_ID";
//            var smokeId = Guid.NewGuid().ToString();
//            var user = new User()
//            {
//                Id = userId,
//                TestId = testId
//            };

//            var userTest = new Test()
//            {
//                Id = testId,
//                UserId = userId
//            };

//            var newSmoke = new Smoke()
//            {
//                Id = smokeId,
//                CreatedOn = dateTime,
//                StartSmokeTime = dateTime,
//                TestId = testId
//            };

//            userTest.SmokedCigaresUnderTest.Add(newSmoke);

//            realm.Write(() =>
//            {
//                realm.Add(user);

//                user.Tests.Add(userTest);
//            });

//            // Act
//            underTestViewModel.StartSmokingCommand.Execute(isSmoking);

//            //Assert
//            appLoggerServiceMock.Verify(e => e.LogCritical(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
//        }

//        /// <summary>
//        /// App State - SmokedCigaresUnderTest should contain only one started smoke, set completed if are found some
//        /// </summary>
//        [Test]
//        public void Mark_Completed_Smokes_In_SmokedCigaresUnderTest_Collection_When_Contains_More_Then_One_Started_Smoke()
//        {
//            //Arrange
//            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
//            var realm = Realm.GetInstance(config);

//            var dateTime = DateTime.Now;
//            var isSmoking = true;
//            var navigationServiceMock = new Mock<INavigationService>();

//            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
//            dateTimeWrapperMock.Setup(e => e.Now()).Returns(dateTime);

//            var appLoggerServiceMock = new Mock<IAppLogger>();

//            var dialogServiceMock = new Mock<IDialogService>();
//            dialogServiceMock.Setup(e => e.ConfirmAsync(
//                It.IsAny<string>(),
//                It.IsAny<string>(),
//                It.IsAny<string>(),
//                It.IsAny<string>()))
//                    .Returns(Task.FromResult(true));

//            
//            var testCalculationServiceMock = new Mock<ITestCalculationService>();
//            var deviceTimerMock = new Mock<IDeviceTimer>();


//            var underTestViewModel = new UnderTestViewModel(
//                realm,
//                navigationServiceMock.Object,
//                dateTimeWrapperMock.Object,
//                appLoggerServiceMock.Object,
//                dialogServiceMock.Object,
//                
//                testCalculationServiceMock.Object,
//                deviceTimerMock.Object
//                );

//            var userId = Globals.UserId;
//            var testId = "TEST_ID";
//            var smokeId = Guid.NewGuid().ToString();
//            var user = new User()
//            {
//                Id = userId,
//                TestId = testId
//            };

//            var userTest = new Test()
//            {
//                Id = testId,
//                UserId = userId
//            };

//            var newSmoke = new Smoke()
//            {
//                Id = smokeId,
//                CreatedOn = dateTime,
//                StartSmokeTime = dateTime,
//                TestId = testId
//            };

//            userTest.SmokedCigaresUnderTest.Add(newSmoke);

//            realm.Write(() =>
//            {
//                realm.Add(user);

//                user.Tests.Add(userTest);
//            });

//            // Act
//            underTestViewModel.StartSmokingCommand.Execute(isSmoking);

//            //Assert
//            var userUpdatedTest = realm.Find<Test>(testId);

//            Assert.NotNull(userUpdatedTest);

//            var smokedCigaresUnderTest = userUpdatedTest.SmokedCigaresUnderTest;

//            Assert.True(smokedCigaresUnderTest.Count == 2);

//            var smokedCigare = smokedCigaresUnderTest.FirstOrDefault(e => e.Id == smokeId);

//            Assert.True(smokedCigare.TestId.Equals(testId));
//            Assert.True(smokedCigare.CreatedOn.LocalDateTime.Equals(dateTime));
//            Assert.True(smokedCigare.StartSmokeTime.LocalDateTime.Equals(dateTime));
//            Assert.True(smokedCigare.EndSmokeTime.LocalDateTime.Equals(dateTime));
//            Assert.True(smokedCigare.ModifiedOn.LocalDateTime.Equals(dateTime));
//        }

//        /// <summary>
//        /// Navigates to SomethingWentWrongViewModel when user is not found
//        /// </summary>
//        [Test]
//        public void Navigates_To_SomethingWentWrongViewModel_When_User_Is_Not_Found()
//        {
//            //Arrange
//            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
//            var realm = Realm.GetInstance(config);

//            var dateTime = DateTime.Now;
//            bool isSmoking = true;
//            var navigationServiceMock = new Mock<INavigationService>();
//            navigationServiceMock.Setup(e => e.NavigateToAsync<SomethingWentWrongViewModel>());


//            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
//            dateTimeWrapperMock.Setup(e => e.Now()).Returns(dateTime);

//            var appLoggerServiceMock = new Mock<IAppLogger>();
//            var dialogServiceMock = new Mock<IDialogService>();
//            dialogServiceMock.Setup(e => e.ConfirmAsync(
//                It.IsAny<string>(),
//                It.IsAny<string>(),
//                It.IsAny<string>(),
//                It.IsAny<string>()))
//                    .Returns(Task.FromResult(true));

//            
//            var testCalculationServiceMock = new Mock<ITestCalculationService>();
//            var deviceTimerMock = new Mock<IDeviceTimer>();

//            var underTestViewModel = new UnderTestViewModel(
//                realm,
//                navigationServiceMock.Object,
//                dateTimeWrapperMock.Object,
//                appLoggerServiceMock.Object,
//                dialogServiceMock.Object,
//                
//                testCalculationServiceMock.Object,
//                deviceTimerMock.Object
//                );

//            // Act
//            underTestViewModel.StartSmokingCommand.Execute(isSmoking);

//            //Assert
//            navigationServiceMock.Verify(e => e.NavigateToAsync<SomethingWentWrongViewModel>(), Times.Once);
//        }

//        /// <summary>
//        /// Navigates to SomethingWentWrongViewModel when user test is not found
//        /// </summary>
//        [Test]
//        public void Navigates_To_SomethingWentWrongViewModel_When_User_Test_Is_Not_Found()
//        {
//            //Arrange
//            var config = new InMemoryConfiguration(Guid.NewGuid().ToString());
//            var realm = Realm.GetInstance(config);

//            var dateTime = DateTime.Now;
//            var isSmoking = true;
//            var navigationServiceMock = new Mock<INavigationService>();
//            navigationServiceMock.Setup(e => e.NavigateToAsync<SomethingWentWrongViewModel>());


//            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
//            dateTimeWrapperMock.Setup(e => e.Now()).Returns(dateTime);

//            var appLoggerServiceMock = new Mock<IAppLogger>();

//            var dialogServiceMock = new Mock<IDialogService>();
//            dialogServiceMock.Setup(e => e.ConfirmAsync(
//                It.IsAny<string>(),
//                It.IsAny<string>(),
//                It.IsAny<string>(),
//                It.IsAny<string>()))
//                    .Returns(Task.FromResult(true));

//            
//            var testCalculationServiceMock = new Mock<ITestCalculationService>();
//            var deviceTimerMock = new Mock<IDeviceTimer>();

//            var underTestViewModel = new UnderTestViewModel(
//                realm,
//                navigationServiceMock.Object,
//                dateTimeWrapperMock.Object,
//                appLoggerServiceMock.Object,
//                dialogServiceMock.Object,
//                
//                testCalculationServiceMock.Object,
//                deviceTimerMock.Object
//                );

//            var userId = Globals.UserId;
//            var testId = "TEST_ID";

//            var user = new User()
//            {
//                Id = userId,
//                TestId = testId
//            };

//            realm.Write(() =>
//            {
//                realm.Add(user);
//            });

//            // Act
//            underTestViewModel.StartSmokingCommand.Execute(isSmoking);

//            //Assert
//            navigationServiceMock.Verify(e => e.NavigateToAsync<SomethingWentWrongViewModel>(), Times.Once);
//        }
//    }
//}
