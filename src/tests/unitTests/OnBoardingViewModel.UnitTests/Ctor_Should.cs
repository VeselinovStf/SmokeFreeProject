using Moq;
using NUnit.Framework;
using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.ViewModels.OnBoarding;

namespace OnBoardingViewModelTests.UnitTests
{
    /// <summary>
    ///  OnBoardingViewModel ctor Tests
    /// </summary>
    public class Ctor_Should
    {
        [Test]
        public void Create_Instance_Successfully()
        {
            //Arrange
            var config = new InMemoryConfiguration("Create_Instance_Successfully");
            var realm = Realm.GetInstance(config);

            var navigationServiceMock = new Mock<INavigationService>();
            var dateTimeWrapperMock = new Mock<IDateTimeWrapper>();

            var onBoardingViewModel = new OnBoardingViewModel(
                realm,
                navigationServiceMock.Object,
                dateTimeWrapperMock.Object
                );

            //Assert
            Assert.NotNull(onBoardingViewModel);
        }
    }
}
