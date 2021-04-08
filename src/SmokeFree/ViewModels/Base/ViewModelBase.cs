using SmokeFree.Abstraction.Services.General;
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

        public ViewModelBase(
            INavigationService navigationService,
            IDateTimeWrapper dateTimeWrapper)
        {
            _navigationService = navigationService;
            _dateTime = dateTimeWrapper;
        }

        public virtual Task InitializeAsync(object parameter)
        {
            return Task.CompletedTask;
        }
    }
}
