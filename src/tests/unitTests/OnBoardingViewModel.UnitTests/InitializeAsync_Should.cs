using Moq;
using NUnit.Framework;
using Realms;
using SmokeFree;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.ViewModels.OnBoarding;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnBoardingViewModelTests.UnitTests
{
    /// <summary>
    /// OnBoardingViewModel InitializeAsync Tests
    /// </summary>
    public class InitializeAsync_Should
    {       
        /// <summary>
        /// Create A Default User, If is not existing. When override of InitializeAsync Is Called
        /// </summary>
        [Test]
        public async Task Create_Default_User_If_Not_Exist_Successfully()
        {
            //Arrange
            var config = new InMemoryConfiguration("Create_Default_User_Successfully");
            var realm = Realm.GetInstance(config);

            var dateTimeOfCreation = DateTime.Now;

            var navigationServiceMock = new Mock<INavigationService>();
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();
                dateTimeWrapperMock.Setup(e => e.Now()).Returns(dateTimeOfCreation);

            var onBoardingViewModel = new OnBoardingViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object);

            object initializeParameter = new object();

            var globalUserId = Globals.UserId;

            //Act
            await onBoardingViewModel.InitializeAsync(initializeParameter);
            var realmUser = realm.Find<User>(globalUserId);

            //Assert
            Assert.NotNull(realmUser);
            Assert.Equals(globalUserId, realmUser.Id);
            Assert.AreEqual(realmUser.CreatedOn, dateTimeOfCreation);
            Assert.AreNotEqual(realmUser.UserState, UserStates.UserUnderTesting);
        }

        /// <summary>
        /// Initialize Async is not creating new user if is previosly created. When override of InitializeAsync Is Called
        /// </summary>
        [Test]
        public async Task Not_Creatte_New_User_If_Is_Existing()
        {
            //Arrange
            var config = new InMemoryConfiguration("Not_Creatte_New_User_If_Is_Existing");
            var realm = Realm.GetInstance(config);

            var dateTimeOfCreation = DateTime.Now;
            var globalUserId = Globals.UserId;

            var navigationServiceMock = new Mock<INavigationService>();
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();

            var previouslyCreatedUser = new User()
            {
                CreatedOn = dateTimeOfCreation,
                Id = globalUserId
            };

            var onBoardingViewModel = new OnBoardingViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object);

            object initializeParameter = new object();

            realm.Write(() =>
            {
                realm.Add(previouslyCreatedUser);
            });

            //Act
            await onBoardingViewModel.InitializeAsync(initializeParameter);
            var realmUser = realm.All<User>()
                .ToList();

            //Assert
            Assert.AreEqual(realmUser.Count, 1);

            var currentPresentUser = realmUser.FirstOrDefault
                (u => u.Id == globalUserId);

            Assert.NotNull(currentPresentUser);
            Assert.AreEqual(currentPresentUser.CreatedOn, dateTimeOfCreation);
        }
    }
}