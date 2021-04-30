using SmokeFree.Abstraction.Services.General;
using System.Linq;
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

        /// <summary>
        /// Holds Application Color Sheme
        /// </summary>
        public string ColorKey
        {
            get
            {
                return Preferences.Get(nameof(ColorKey), Globals.AppColorThemes.First().Key);
            }
            set
            {
                Preferences.Set(nameof(ColorKey), value);
            }
        }
    }
}
