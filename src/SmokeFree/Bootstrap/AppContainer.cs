using Autofac;
using Realms;
using SmokeFree.Abstraction.Managers;
using SmokeFree.Abstraction.Services.Data.Test;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.DeviceUtilities;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Services.Data.Test;
using SmokeFree.Services.General;
using SmokeFree.Utilities.DeviceUtilities;
using SmokeFree.Utilities.Logging;
using SmokeFree.Utilities.Wrappers;
using SmokeFree.ViewModels.AppSettings;
using SmokeFree.ViewModels.Challenge;
using SmokeFree.ViewModels.ErrorAndEmpty;
using SmokeFree.ViewModels.OnBoarding;
using SmokeFree.ViewModels.Test;
using System;

namespace SmokeFree.Bootstrap
{
    /// <summary>
    /// Application Container
    /// </summary>
    public class AppContainer
    {
        private static IContainer _container;

        /// <summary>
        /// Dependency Registrations
        /// </summary>
        public static void RegisterDependencies()
        {
            var _builder = new ContainerBuilder();

            // Database
            var realmConfiguration = new RealmConfiguration
            {
                SchemaVersion = 6
            };

            _builder.Register(c => Realm.GetInstance(realmConfiguration)).InstancePerDependency();

            //VIEW MODELS
            _builder.RegisterType<OnBoardingViewModel>();
            _builder.RegisterType<CreateTestViewModel>();
            _builder.RegisterType<AppSettingsViewModel>();
            _builder.RegisterType<UnderTestViewModel>();
            _builder.RegisterType<SomethingWentWrongViewModel>();
            _builder.RegisterType<TestResultViewModel>();
            _builder.RegisterType<CreateChallengeViewModel>();
            _builder.RegisterType<ChallengeViewModel>();

            // SERVICES
            // SERVICES - GENERAL
            _builder.RegisterType<NavigationService>().As<INavigationService>().SingleInstance();
            _builder.RegisterType<DialogService>().As<IDialogService>();
            _builder.RegisterType<NetworkConnectionService>().As<INetworkConnectionService>();
            _builder.RegisterType<AppPreferencesService>().As<IAppPreferencesService>();

            // SERVICES - DATA
            _builder.RegisterType<TestCalculationService>().As<ITestCalculationService>();


            // UTILITIES
            // LOGGER
            _builder.RegisterType<AppLogger>().As<IAppLogger>().SingleInstance();
            _builder.RegisterType<LocalLogUtility>().As<ILocalLogUtility>();

            // DateTime Wrapper
            _builder.RegisterType<DateTimeWrapper>().As<IDateTimeWrapper>().SingleInstance();

            // Device Utilities
            _builder.RegisterType<DeviceTimer>().As<IDeviceTimer>().InstancePerDependency();
            _builder.RegisterType<DeviceEmailSender>().As<IDeviceEmailSender>().InstancePerDependency();

            // Platform Specific Implementations
            _builder.Register(ctx => Xamarin.Forms.DependencyService.Get<INotificationManager>())
                .As<INotificationManager>();

            _container = _builder.Build();
        }

        /// <summary>
        /// Resolve from Container
        /// </summary>
        /// <param name="typeName">Type of entity to resolve</param>
        /// <returns></returns>
        public static object Resolve(Type typeName)
        {
            return _container.Resolve(typeName);
        }

        /// <summary>
        /// Generic Resolve from Container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }
    }
}
