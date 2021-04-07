using System.Threading.Tasks;

namespace SmokeFree.ViewModels.Base
{
    /// <summary>
    /// Base Model For ViewModels
    /// </summary>
    public class ViewModelBase
    {
        public virtual Task InitializeAsync(object parameter)
        {
            return Task.CompletedTask;
        }
    }
}
