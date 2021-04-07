using Autofac;
using System;

namespace SmokeFree.Bootstrap
{
    public class AppContainer
    {
        //private static TinyIoCContainer _builder;
        private static IContainer _container;

        public static void RegisterDependencies()
        {
            var _builder = new ContainerBuilder();
        

            _container = _builder.Build();
        }

        public static object Resolve(Type typeName)
        {
            return _container.Resolve(typeName);
        }

        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }
    }
}
