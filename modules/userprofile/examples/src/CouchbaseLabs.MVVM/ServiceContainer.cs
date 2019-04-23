using System;
using SimpleInjector;

namespace CouchbaseLabs.MVVM
{
    public static class ServiceContainer
    {
        static readonly Container _container = new Container();

        public static void Register<TService, TImplementation>(bool transient = false) where TService : class where TImplementation : class, TService
        {
            Lifestyle style = transient ? Lifestyle.Transient : Lifestyle.Singleton;
            _container.Register<TService, TImplementation>(style);
        }

        public static void Register<TService>(Func<TService> generator, bool transient = false) where TService : class
        {
            Lifestyle style = transient ? Lifestyle.Transient : Lifestyle.Singleton;
            _container.Register(generator, style);
        }

        public static void Register(Type serviceType, Type implementationType, bool isTransient = false)
        {
            if (isTransient)
            {
                _container.Register(serviceType, implementationType, Lifestyle.Transient);
            }
            else
            {
                _container.Register(serviceType, implementationType, Lifestyle.Singleton);
            }
        }

        public static void Register<TService>(TService instance) where TService : class
        {
            _container.RegisterInstance(instance);
        }

        public static T GetInstance<T>() where T : class
        {
            try
            {
                return _container.GetInstance<T>();
            }
            catch (ActivationException)
            {
                return null;
            }
        }

        internal static T GetRequiredInstance<T>() where T : class
        {
            return GetInstance<T>() ?? throw new InvalidOperationException(
                       $@"A required dependency injection class is missing ({typeof(T).FullName}).");
        }
    }
}
