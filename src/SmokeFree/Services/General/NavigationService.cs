using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Data.Models;
using SmokeFree.Resx;
using SmokeFree.Utilities.UserStateHelpers;
using SmokeFree.ViewModels.Base;
using SmokeFree.ViewModels.Challenge;
using SmokeFree.ViewModels.ErrorAndEmpty;
using SmokeFree.ViewModels.OnBoarding;
using SmokeFree.ViewModels.Test;
using SmokeFree.Views;
using SmokeFree.Views.OnBoarding;
using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;

namespace SmokeFree.Services.General
{
    /// <summary>
    /// App Navigation
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly Realm _realm;
        private readonly IAppLogger _appLogger;
        private readonly IDialogService _dialogService;

        public NavigationService(
            Realm realm,
            IAppLogger appLogger,
            IDialogService dialogService)
        {
            _realm = realm;
            _appLogger = appLogger;
            _dialogService = dialogService;
        }

        public Task InitializeAsync()
        {
            try
            {
                var user = this._realm
                    .Find<User>(Globals.UserId);

                if (user == null)
                {
                    return NavigateToAsync<OnBoardingViewModel>();
                }

                LocalizationResourceManager.Current.CurrentCulture = new CultureInfo(user.Localozation);

                var userState = StringToEnum.ToUserState<UserStates>(user.UserState);

                // TODO: C: Clear navigation back stack
                switch (userState)
                {
                    case UserStates.Initial:
                        return NavigateToAsync<OnBoardingViewModel>();
                    case UserStates.CompletedOnBoarding:
                        return NavigateToAsync<CreateTestViewModel>();
                    case UserStates.UserUnderTesting:
                        return NavigateToAsync<UnderTestViewModel>();
                    case UserStates.IsTestComplete:
                        return NavigateToAsync<TestResultViewModel>();
                    case UserStates.InCreateChallenge:
                        return NavigateToAsync<CreateChallengeViewModel>();
                    case UserStates.InChallenge:
                        return NavigateToAsync<ChallengeViewModel>();
                    case UserStates.Complete:
                        //TODO: B: Add Complete
                        break;
                    default:
                        break;
                }

                throw new Exception($"Nowhere to navigate! User Id: {user.Id}, User State: {user.UserState}");
            }
            catch (Exception ex)
            {
                this._appLogger.LogCritical(ex.Message);

                return NavigateToAsync<SomethingWentWrongViewModel>();
            }

        }

        /// <summary>
        /// Turn Back To Previous Page
        /// </summary>
        /// <returns></returns>
        public Task BackToPreviousAsync()
        {
            try
            {
                var mainPage = Application.Current.MainPage as SmokeFreeNavigationView;

                if (mainPage != null)
                {
                    if (mainPage.Navigation.NavigationStack.Count > 1)
                    {
                        mainPage.Navigation.PopAsync();
                    }
                    else
                    {
                        return NavigateToAsync<OnBoardingViewModel>();
                    }
                }

            }
            catch (Exception ex)
            {
                this._appLogger.LogCritical(ex.Message);

                return NavigateToAsync<SomethingWentWrongViewModel>();
            }

            return Task.FromResult(true);
        }

        /// <summary>
        /// Remove Last Inserted (-2) View From Navigation
        /// </summary>
        /// <returns></returns>
        public Task RemoveLastFromBackStackAsync()
        {
            try
            {
                var mainPage = Application.Current.MainPage as SmokeFreeNavigationView;

                if (mainPage != null)
                {
                    mainPage.Navigation.RemovePage(
                        mainPage.Navigation.NavigationStack[mainPage.Navigation.NavigationStack.Count - 2]);
                }


            }
            catch (Exception ex)
            {
                this._appLogger.LogCritical(ex.Message);

                return NavigateToAsync<SomethingWentWrongViewModel>();
            }

            return Task.FromResult(true);

        }

        /// <summary>
        /// Remove Last Inserted View From Navigation
        /// </summary>
        /// <returns></returns>
        public Task RemoveBackStackAsync()
        {
            try
            {
                var mainPage = Application.Current.MainPage as SmokeFreeNavigationView;

                if (mainPage != null)
                {
                    for (int i = 0; i < mainPage.Navigation.NavigationStack.Count - 1; i++)
                    {
                        var page = mainPage.Navigation.NavigationStack[i];
                        mainPage.Navigation.RemovePage(page);
                    }
                }


            }
            catch (Exception ex)
            {
                this._appLogger.LogCritical(ex.Message);

                return NavigateToAsync<SomethingWentWrongViewModel>();
            }

            return Task.FromResult(true);
        }

        public Task NavigateToAsync<TViewModel>() where TViewModel : ViewModelBase
        {
            return InternalNavigateToAsync(typeof(TViewModel), null);
        }

        public Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : ViewModelBase
        {
            return InternalNavigateToAsync(typeof(TViewModel), parameter);
        }

        private async Task InternalNavigateToAsync(Type viewModelType, object parameter)
        {
            try
            {
                Page page = CreatePage(viewModelType, parameter);

                if (page is OnBoardingView)
                {
                    Application.Current.MainPage = new SmokeFreeNavigationView(page);
                }
                else
                {
                    var navigationPage = Application.Current.MainPage as SmokeFreeNavigationView;
                    if (navigationPage != null)
                    {
                        await navigationPage.PushAsync(page);
                    }
                    else
                    {
                        Application.Current.MainPage = new SmokeFreeNavigationView(page);
                    }
                }

                await (page.BindingContext as ViewModelBase).InitializeAsync(parameter);
            }
            catch (Exception ex)
            {
                this._appLogger.LogCritical(ex);

                // Validate Type param
                if (viewModelType != null)
                {
                    // Validate if SomethingWentWrongViewModel is throwing
                    if (viewModelType.Name.Equals(nameof(SomethingWentWrongViewModel)))
                    {
                        this._appLogger.LogCritical($"Application is trowing second navigation exception on - Navigate to {nameof(SomethingWentWrongViewModel)}");

                        // Notify for critical exception
                        await this._dialogService
                            .ShowDialog(
                                AppResources.CriticalApplicationFail,
                                AppResources.SomethingWentWrongViewModelHeader,
                                AppResources.ButtonOkText);

                        // Force close the app
                        throw new Exception(AppResources.CriticalApplicationFail);
                    }
                }

                await NavigateToAsync<SomethingWentWrongViewModel>();
            }

        }

        private Type GetPageTypeForViewModel(Type viewModelType)
        {
            var viewName = viewModelType.FullName.Replace("Model", string.Empty);
            var viewModelAssemblyName = viewModelType.GetTypeInfo().Assembly.FullName;
            var viewAssemblyName = string.Format(
                        CultureInfo.InvariantCulture, "{0}, {1}", viewName, viewModelAssemblyName);
            var viewType = Type.GetType(viewAssemblyName);
            return viewType;
        }

        private Page CreatePage(Type viewModelType, object parameter)
        {
            Type pageType = GetPageTypeForViewModel(viewModelType);
            if (pageType == null)
            {
                throw new Exception($"Cannot locate page type for {viewModelType}");
            }

            Page page = Activator.CreateInstance(pageType) as Page;
            return page;
        }


    }
}
