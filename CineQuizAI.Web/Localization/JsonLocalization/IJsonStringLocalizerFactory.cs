namespace CineQuizAI.Web.Localization.JsonLocalization;

public interface IJsonStringLocalizerFactory
{
    /// <summary>Create a localizer for a "source" (e.g., "Register" or "Shared").</summary>
    IJsonStringLocalizer Create(string resourceName);
}
