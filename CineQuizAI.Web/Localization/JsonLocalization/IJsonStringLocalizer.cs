namespace CineQuizAI.Web.Localization.JsonLocalization;

public interface IJsonStringLocalizer
{
    string this[string key] { get; }
    string Format(string key, params object[] args);
}
