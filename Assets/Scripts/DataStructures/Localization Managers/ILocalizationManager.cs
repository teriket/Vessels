using UnityEngine;

namespace DataStructures.Localization
{
    /// <summary>
    /// Interface for database managers that are responsible for changing the displayed language
    /// of a setting at runtime
    /// </summary>
    public interface ILocalizationManager
    {
        void LoadLocalizations();
        void SubscribeToLanguageChanges(LocalizableString str);
        void UnsubscribeToLanguageChanges(LocalizableString str);
    }
}