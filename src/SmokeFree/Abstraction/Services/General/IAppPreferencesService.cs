using System;
using System.Collections.Generic;
using System.Text;

namespace SmokeFree.Abstraction.Services.General
{
    /// <summary>
    ///  Application preferences store abstraction.
    /// </summary>
    public interface IAppPreferencesService
    {
        /// <summary>
        /// Holds Application CultureInfo Language Value
        /// </summary>
        string LanguageValue { get; set; }
    }
}
