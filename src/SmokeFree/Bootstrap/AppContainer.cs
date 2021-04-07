using Autofac;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Services.General;
using SmokeFree.Utilities.Logging;
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

            //VIEW MODELS
            _builder.RegisterType<OnBoardingViewModel>();
            _builder.RegisterType<CreateTestViewModel>();

            // SERVICES
            // SERVICES - GENERAL
            _builder.RegisterType<NavigationService>().As<INavigationService>().SingleInstance();

            // UTILITIES
            // UTILITIES - LOGGER
            _builder.RegisterType<DebugLogger>().As<IAppLogger>().SingleInstance();


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
