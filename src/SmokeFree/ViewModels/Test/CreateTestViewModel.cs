using Realms;
using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using SmokeFree.Models.Views.Test;
using SmokeFree.Resx;
using SmokeFree.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SmokeFree.ViewModels.Test
{
    public class CreateTestViewModel : ViewModelBase
    {
        #region FIELDS

        /// <summary>
        /// Test Time Duration Collection
        /// </summary>
        private ObservableCollection<TestDurationItem> _testTimeDurations;

        /// <summary>
        /// Selected Test Duration Item
        /// </summary>
        private TestDurationItem _selectedTestTimeDurationItem;

        /// <summary>
        /// Initial Goal To Stop Smoking DateTime Value
        /// </summary>
        private DateTime _goalDateTime;

        /// <summary>
        /// Database
        /// </summary>
        private readonly Realm _realm;

        #endregion

        #region CTOR

        public CreateTestViewModel(
           Realm realm,
           INavigationService navigationService,
           IDateTimeWrapper dateTimeWrapper,
           IAppLogger appLogger,
           IDialogService dialogService)
           : base(navigationService, dateTimeWrapper, appLogger, dialogService)
        {
            // Set View Title
            ViewTitle = AppResources.CreateTestViewTiitle;

            // Set Database
            _realm = realm;

            // Collection Of Test Times that user can chose frome
            this._testTimeDurations = new ObservableCollection<TestDurationItem>();

            // Goal Date Completition
            this.GoalDateTime = _dateTime.Now();

            // Init Collection Of Test Durations
            InitiateTestTimeDurations();
        }

        #endregion

        #region INIT

        /// <summary>
        /// Initialize Test Time Durations Collection
        /// </summary>
        private void InitiateTestTimeDurations()
        {
            this.TestTimeDurations.Clear();

            var durations = new List<TestDurationItem>()
            {
                new TestDurationItem()
                {
                     DayValue = 1,
                     DisplayText =  string.Format("1 {0}", AppResources.SingleDay)
                },
                new TestDurationItem()
                {
                     DayValue = 2,
                     DisplayText =  string.Format("2 {0}", AppResources.PluralDay)
                },
                new TestDurationItem()
                {
                     DayValue = 3,
                     DisplayText =  string.Format("3 {0}", AppResources.PluralDay)
                },
                new TestDurationItem()
                {
                     DayValue = 4,
                     DisplayText =  string.Format("4 {0}", AppResources.PluralDay)
                },
                new TestDurationItem()
                {
                     DayValue = 5,
                     DisplayText =  string.Format("5 {0}", AppResources.PluralDay)
                }
            };

            foreach (var duration in durations)
            {
                this.TestTimeDurations.Add(duration);
            }
        }

        #endregion

        #region COMMANDS


        #endregion

        #region PROPS

        /// <summary>
        /// Test Time Duration for Picker
        /// </summary>
        public ObservableCollection<TestDurationItem> TestTimeDurations
        {
            get { return _testTimeDurations; }
            set
            {
                if (value != null)
                {
                    _testTimeDurations = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Selected Test Time Duration Item
        /// </summary>
        public TestDurationItem SelectedTestTimeDurationItem
        {
            get { return _selectedTestTimeDurationItem; }
            set
            {
                if (value != null)
                {
                    _selectedTestTimeDurationItem = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Initial Goal To Stop Smoking DateTime Value
        /// </summary>
        public DateTime GoalDateTime
        {
            get { return _goalDateTime; }
            set
            {
                _goalDateTime = value;
                OnPropertyChanged();
            }
        }

        #endregion

    }
}
