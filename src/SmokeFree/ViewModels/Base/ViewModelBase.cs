﻿using SmokeFree.Abstraction.Services.General;
using SmokeFree.Abstraction.Utility.Logging;
using SmokeFree.Abstraction.Utility.Wrappers;
using System.Threading.Tasks;

namespace SmokeFree.ViewModels.Base
{
    /// <summary>
    /// Base Model For ViewModels
    /// </summary>
    public class ViewModelBase
    {
        /// <summary>
        /// Application Navigation Service
        /// </summary>
        protected readonly INavigationService _navigationService;

        /// <summary>
        /// Application DateTime Provider
        /// </summary>
        protected readonly IDateTimeWrapper _dateTime;

        /// <summary>
        /// Application Logger
        /// </summary>
        protected readonly IAppLogger _appLogger;

        /// <summary>
        /// Application Dialog Service
        /// </summary>
        protected readonly IDialogService _dialogService;

        public ViewModelBase(
            INavigationService navigationService,
            IDateTimeWrapper dateTimeWrapper,
            IAppLogger appLogger,
            IDialogService dialogService)
        {
            _navigationService = navigationService;
            _dateTime = dateTimeWrapper;
            _appLogger = appLogger;
            _dialogService = dialogService;
        }

        public virtual Task InitializeAsync(object parameter)
        {
            return Task.CompletedTask;
        }
    }
}
