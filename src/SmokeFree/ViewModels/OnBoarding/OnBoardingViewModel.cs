using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Data.Models;
using SmokeFree.Models.Views.OnBoarding;
using SmokeFree.Resx;
using SmokeFree.ViewModels.Base;
using SmokeFree.ViewModels.Test;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace SmokeFree.ViewModels.OnBoarding
{
    /// <summary>
    /// View Model For OnBoardingView
    /// </summary>
    public class OnBoardingViewModel : ViewModelBase
    {
        #region FIELDS

        /// <summary>
        /// On Boarding Items For Carousel View
        /// </summary>
        private ObservableCollection<OnBordingItem> _onBoardingItems;

        /// <summary>
        /// Curent selected carousel index value
        /// </summary>
        private int _selectedOnBoardingItemsIndex;

        /// <summary>
        /// Database
        /// </summary>
        private readonly Realm _realm;

        /// <summary>
        /// Application User
        /// </summary>
        private User _appUser;

        #endregion

        #region CTOR

        public OnBoardingViewModel(
            Realm realm,
            INavigationService navigationService,
            IDateTimeWrapper dateTimeWrapper,
            IAppLogger appLogger,
            IDialogService dialogService)
            : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
            // Set View Title
            ViewTitle = AppResources.OnBoardingViewTitle;

            // Set Database
            _realm = realm;

            // Create collections
            this.OnBoardingItems = new ObservableCollection<OnBordingItem>();

            // Init OC OnBordingItems
            InitiateOnBoardingItems();
        }

        #endregion

        #region INIT

        /// <summary>
        /// Initialize ViewModel - Any user who comes here is going trough on boarding
        /// </summary>
        /// <param name="parameter">Not Required</param>
        /// <returns>Base InitializeAsync</returns>
        public override async Task InitializeAsync(object parameter)
        {
            try
            {
                // SET Global User Id
                var globalUserId = Globals.UserId;

                // Get User By Global Id
                var user = _realm.Find<User>(globalUserId);

                // For First Run Of App 
                if (user == null)
                {
                    // Create User If Not Exist
                    var newUser = new User()
                    {
                        Id = globalUserId,
                        CreatedOn = base._dateTime.Now(),
                        UserState = UserStates.Initial.ToString(),
                        Localozation = CultureInfo.CurrentCulture.Name
                    };

                    // Execute Write Transaction
                    _realm.Write(() =>
                    {
                        _realm.Add(newUser);
                    });

                    this.AppUser = newUser;
                }
                else
                {
                    this.AppUser = user;
                }
            }
            catch (Exception ex)
            {
                base._appLogger.LogError(ex.Message);

                await base.InternalErrorMessageToUser();
            }

            // TODO: A: Remove After - Navigate to Error View Model after Initialization Exception
        }

        /// <summary>
        /// Initiating OnBoardingItems Observable Collection with OnBording Item At CTOR level
        /// </summary>
        private void InitiateOnBoardingItems()
        {
            // Clear Collection
            this.OnBoardingItems.Clear();

            // Create List of OnBoarding Items
            // TODO: B: OnBoarding Items are hard coded in InitiateOnBoardingItems, check if this can be refactored
            var staticOnBoardingItems = new List<OnBordingItem>()
            {
                new OnBordingItem()
                {
                    Image = AppResources.OnBoardingItemImage1,
                    Title = AppResources.OnBoardingItemTitle1,
                    Text = AppResources.OnBoardingItemText1,
                    ItemButtonText = AppResources.OnBoardingItemButton1
                },
                new OnBordingItem()
                {
                    Image = AppResources.OnBoardingItemImage2,
                    Title = AppResources.OnBoardingItemTitle2,
                    Text = AppResources.OnBoardingItemText2,
                    ItemButtonText = AppResources.OnBoardingItemButton2
                },
                new OnBordingItem()
                {
                    Image = AppResources.OnBoardingItemImage3,
                    Title = AppResources.OnBoardingItemTitle3,
                    Text = AppResources.OnBoardingItemText3,
                    ItemButtonText = AppResources.OnBoardingItemButton3
                },
                new OnBordingItem()
                {
                    Image = AppResources.OnBoardingItemImage4,
                    Title = AppResources.OnBoardingItemTitle4,
                    Text = AppResources.OnBoardingItemText4,
                    ItemButtonText = AppResources.OnBoardingItemButton4
                }

            };

            // Add each on bording item to ObservableCollection
            foreach (var onBoardingItem in staticOnBoardingItems)
            {
                this.OnBoardingItems.Add(onBoardingItem);
            }
        }

        #endregion

        #region COMMANDS

        /// <summary>
        /// On Changed Carousel position, set carousel index
        /// </summary>
        public ICommand OnPositionChangedCommand => new Command<int>((position) =>
        {
            SelectedOnBoardingItemsIndex = position;
        });

        /// <summary>
        /// Find if final OnBoarding Item is selected and navigate to next page
        /// </summary>
        public IAsyncValueCommand OnBordingItemButtonClickedCommand => new AsyncValueCommand(
            ExecuteOnBordingItemButtonClicked,
            onException: base.GenericCommandExeptionHandler,
            allowsMultipleExecutions: false);

        private async ValueTask ExecuteOnBordingItemButtonClicked()
        {
            try
            {
                // Check if on boarding carousel is on last element
                if (this.SelectedOnBoardingItemsIndex + 1 == this.OnBoardingItems.Count)
                {
                    // If UserState Is initial 
                    // This is User First Run
                    // Else
                    // User is send here by AppSettings
                    // Navigate it back to AppSettings
                    if (AppUser.UserState.Equals(UserStates.Initial.ToString()))
                    {
                        // Persist User State in DB
                        this._realm.Write(() =>
                        {
                            AppUser.UserState = UserStates.CompletedOnBoarding.ToString();
                        });

                        // Navigate to user initial state of actions
                        await base._navigationService.NavigateToAsync<CreateTestViewModel>();
                    }
                    else
                    {
                        // TODO: A: Navigate back - and check for path validaty
                        //await base._navigationService.InitializeAsync();
                        await base._navigationService.InitializeAsync();
                        // TODO: B: Check if navigation stack is correct
                    }

                }
            }
            catch (Exception ex)
            {
                base._appLogger.LogCritical(ex);

                await base.InternalErrorMessageToUser();
            }
        }

        #endregion

        #region PROPS

        /// <summary>
        /// Application User
        /// </summary>
        public User AppUser
        {
            get { return _appUser; }
            set
            {
                _appUser = value;
            }
        }

        /// <summary>
        /// On Boarding Items For Carousel View
        /// </summary>
        public ObservableCollection<OnBordingItem> OnBoardingItems
        {
            get { return _onBoardingItems; }
            set
            {
                if (value != null)
                {
                    _onBoardingItems = value;
                }
            }
        }

        /// <summary>
        /// Curent selected carousel index value
        /// </summary>
        public int SelectedOnBoardingItemsIndex
        {
            get { return _selectedOnBoardingItemsIndex; }
            set
            {
                _selectedOnBoardingItemsIndex = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}
