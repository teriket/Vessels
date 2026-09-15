using UnityEngine;
using UI;

namespace DataStructures.Settings
{
    /// <summary>
    /// Abstract wrapper class that allows generic setting object types
    /// to be serialized together for building a settings menu
    /// </summary>
    public abstract class SettingSO : ScriptableObject
    {
        /// <summary>
        /// Updates the view for this setting to render this scriptable objects
        /// settings and to update the value of this scriptable object
        /// </summary>
        /// <param name="view">The view for this setting to modify</param>
        public abstract void BindTo(SettingItemView view);

        /// <summary>
        /// The type of view to generate for a setting of this type, usually specified
        /// in the strings folder, i.e. Strings.SETTING_VEW_SLIDER
        /// </summary>
        /// <returns>A string message requesting a component to render</returns>
        public abstract string RequestMenuType();
    }
}