using Autofac;
using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Services.General;
using SmokeFree.Utilities.Logging;
using SmokeFree.Utilities.Wrappers;
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
                SchemaVersion = 4,
            };
            
            _builder.Register(c => Realm.GetInstance(realmConfiguration)).InstancePerDependency();

            //VIEW MODELS
            _builder.RegisterType<OnBoardingViewModel>();
            _builder.RegisterType<CreateTestViewModel>();

            // SERVICES
            // SERVICES - GENERAL
            _builder.RegisterType<NavigationService>().As<INavigationService>().SingleInstance();
            _builder.RegisterType<DialogService>().As<IDialogService>();

            // UTILITIES
            // LOGGER
            _builder.RegisterType<DebugLogger>().As<IAppLogger>().SingleInstance();
            // DateTime Wrapper
            _builder.RegisterType<DateTimeWrapper>().As<IDateTimeWrapper>().SingleInstance();


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
