using SmokeFree.ViewModels.Base;
using System.Threading.Tasks;

namespace SmokeFree.Abstraction.Services.General
{
    /// <summary>
    /// App Navigation Abstraction
    /// </summary>
    public interface INavigationService
    {
        Task InitializeAsync();
        Task NavigateToAsync<TViewModel>() where TViewModel : ViewModelBase;
        Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : ViewModelBase;
        Task BackToPreviousAsync();
        Task RemoveLastFromBackStackAsync();
        Task RemoveBackStackAsync();
    }
}
