using SmokeFree.Abstraction.Services.General;
using Xamarin.Essentials;

namespace SmokeFree.Services.General
{
    /// <summary>
    ///  Application preferences store.
    /// </summary>
    public class AppPreferencesService : IAppPreferencesService
    {
        /// <summary>
        /// Holds Application CultureInfo Language Value
        /// </summary>
        public string LanguageValue
        {
            get
            {
                return Preferences.Get(nameof(LanguageValue), string.Empty);
            }
            set
            {
                Preferences.Set(nameof(LanguageValue), value);
            }
        }
    }
}
