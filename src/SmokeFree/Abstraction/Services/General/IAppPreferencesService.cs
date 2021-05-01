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

        /// <summary>
        /// Holds Application Color Sheme
        /// </summary>
        string ColorKey { get; set; }
    }
}
