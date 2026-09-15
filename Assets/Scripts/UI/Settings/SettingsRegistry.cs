using Config;
using DataStructures.Settings;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Generate an array of views for Settings and bind them to their scriptable object implementations
    /// </summary>
    public class SettingsRegistry : MonoBehaviour
    {
        [Tooltip("The settings to be populated into the settings menu")]
        [SerializeField] SettingSO[] registry;

        [Header("Individual UI prefabs that act as a template for settings")]

        [Tooltip("A Slider UI element that supports float bindings")]
        [SerializeField] GameObject sliderPrefab;

        [Tooltip("A checkbox UI element that supports int bindings")]
        [SerializeField] GameObject checkboxPrefab;

        [Tooltip("A clickable box with an input listener to rebind keys")]
        [SerializeField] GameObject keybindingPrefab;

        [Tooltip("A dropdown menu of possible strings")]
        [SerializeField] GameObject dropdownPrefab;

        [Tooltip("A divider for the different categories of settings")]
        [SerializeField] GameObject settingCategory;

        void Start()
        {
            CheckForNullSerializedFields();

            // generate each settings view, then child to the current gameobject
            foreach (SettingSO item in registry)
            {
                GameObject view = GenerateViewAndBindToSetting(item);

                if (view == null) return;
                PositionView(view);
            }
        }

        /// <summary>
        /// Determine which prefab menu view gameobject to instantiate for a given
        /// setting
        /// </summary>
        /// <param name="binding">A scriptable object implementation of SettingSO, usually an object of SettingBindingSO<T></param>
        /// <returns>A prefab template to instantiate</returns>
        private GameObject GetCorrectPrefab(SettingSO binding)
        {
            switch (binding.RequestMenuType())
            {
                case Strings.SETTING_VIEW_SLIDER:
                    return sliderPrefab;
                case Strings.SETTING_VIEW_CHECKBOX:
                    return checkboxPrefab;
                case Strings.SETTING_VIEW_DROPDOWN:
                    return dropdownPrefab;
                case Strings.SETTING_VIEW_KEYBINDING:
                    return keybindingPrefab;
                case Strings.SETTING_VIEW_CATEGORY:
                    return settingCategory;
                default:
                    Development.Logger.Warn($"Could not generate a settings menu view of type {binding.RequestMenuType()}");
                    return null;
            }
        }

        /// <summary>
        /// Instantiates a gameobject to represent the view of a setting and bind its variable
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private GameObject GenerateViewAndBindToSetting(SettingSO item)
        {
            GameObject settingView = GetCorrectPrefab(item);

            // The view being null would be caused by the template prefab not being assigned in the editor,
            // which is logged in the GetCorrectPrefab(...).  It isn't necessary to log here.
            if (settingView == null) return null;

            // gemerate the menu view gameobject.  Rename it in the inspector for clarity
            GameObject instantiatedObject = Instantiate(settingView, gameObject.transform);
            instantiatedObject.name = item.name;

            // get the component on the menu view gameobject that is responsible for configuring the rendering 
            SettingItemView instantiatedViewComponent = instantiatedObject.GetComponent<SettingItemView>();
            if (instantiatedViewComponent == null)
            {
                Development.Logger.CriticalMessage($"The menu view for {item.name} does not have a component of type SettingItemView.  Make sure the template prefab has a component that inherits from this class at the root of the gameobject.");
                return instantiatedObject;
            }

            // have the setting configure the view options
            item.BindTo(instantiatedViewComponent);

            return instantiatedObject;
        }

        /// <summary>
        /// Assign the rect transform of the setting view
        /// </summary>
        /// <param name="view"></param>
        private void PositionView(GameObject view) { }

        /// <summary>
        /// Check that all variables that should be assigned in the unity editor are assigned.  Notify the
        /// user if they are not assigned.
        /// </summary>
        private void CheckForNullSerializedFields()
        {
            if (sliderPrefab == null)
            {
                Development.Logger.Warn("A template slider prefab was not configured in the editor.  Did you remember to assign a gameobject as the slider prefab?");
            }

            if (checkboxPrefab == null)
            {
                Development.Logger.Warn("A template checkbox prefab was not configured in the editor.  Did you remember to assign a gameobject as the checkbox prefab?");
            }

            if (keybindingPrefab == null)
            {
                Development.Logger.Warn("A template keybinding prefab was not configured in the editor.  Did you remember to assign a gameobject as the keybinding prefab?");
            }

            if (dropdownPrefab == null)
            {
                Development.Logger.Warn("A template dropdown prefab was not configured in the editor.  Did you remember to assign a gameobject as the dropdwon prefab?");
            }

            if (settingCategory == null)
            {
                Development.Logger.Warn("A template category prefab was not configured in the editor.  Did you remember to assign a gameobject as the dropdown prefab?");
            }
        }

    }
}