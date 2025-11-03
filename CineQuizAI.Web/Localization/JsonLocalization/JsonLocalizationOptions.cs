namespace CineQuizAI.Web.Localization.JsonLocalization
{
    public sealed class JsonLocalizationOptions
    {
        /// <summary>
        /// Absolute or relative folder where localization JSON files live.
        /// Example: {ContentRoot}/Localization/JsonLocalization
        /// </summary>
        public string ResourcesPath { get; set; } = string.Empty;

        /// <summary>
        /// Base filename (without culture suffix or extension).
        /// Example: "strings" -> strings.en.json / strings.sv.json
        /// </summary>
        public string FileName { get; set; } = "strings";

        /// <summary>
        /// Fallback culture if none is matched (e.g., "en").
        /// </summary>
        public string FallbackCulture { get; set; } = "en";
    }
}
