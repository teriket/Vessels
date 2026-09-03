using UnityEngine;
using Development;
using System.Collections.Generic;
using System;
using DataStructures;

namespace Control
{
    /// <summary>
    /// Manages settings that may be updated dynamically during runtime,
    /// such as audio settings, camera settings, language, etc.  Bindings that subscribe
    /// to this class are automatically updated and reflected in the player preferences.
    /// This class should not be used to save gameplay elements, like upgrades,
    /// jump height, etc. or to store secrets.
    /// </summary>
    /// <remarks>
    /// Consumers should use the class as follows <br/>
    /// <code>
    /// public someClass{
    ///     SettingsManager settings;
    ///     Bindable&lt; float | int | string &gt; myVariable;
    /// 
    ///     void Start(){
    ///         settings = SettingsManager.GetInstance(); <br/>
    ///         if(settings != null){ <br/>
    ///             myVariable = settings.Register("someKey", someValue); <br/>
    ///         }    
    ///         // if myVariable != null, do something with myVariable.Value <br/>
    ///         // modify the value via settings.Modify&lt;int&gt;(key, newValue);
    /// </code>
    /// </remarks>
    public class SettingsManager : MonoBehaviour, IIoCComponent<SettingsManager>
    {
        // im concerned about data bindings falling out of sync with
        // eachother if multiple settings managers are created, so I'm
        // using singleton access here.  Consumers should still use the IoC
        // container for code consistency, so I'm not exposing a getter
        private static SettingsManager instance;

        // There are 3 dicionaries because managing one dictionary with a generic
        // bindable becomes a nightmare when they need to be dynamically created and
        // saved based on consumer-script inputs
        private Dictionary<string, Bindable<int>> integerSettings = new();
        private Dictionary<string, Bindable<float>> floatSettings = new();
        private Dictionary<string, Bindable<string>> stringSettings = new();

        public IoCContainer iocContainer { get; set; }
        private IIoCComponent<SettingsManager> ioc => this;

        /// <summary>
        /// Make sure there aren't any duplicate instances of this,
        /// then register to the IoC container
        /// </summary>
        void Start()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
            }

            instance = this;
            ioc.InitializeIoCContainer(instance);
        }

        /// <summary>
        /// Fetches a setting binding or creates a new one if it doesn't exist
        /// for string settings.  This method is overloaded because dynamically
        /// returning generic values in boxes is poorly supported
        /// </summary>
        /// <param name="key">The setting the binding comes from</param>
        /// <param name="defaultValue">The default value if a new setting is created</param>
        /// <returns>A boxed integer that updates universally across gameobjects</returns>
        public Bindable<int> Register(string key, int defaultValue)
        {
            if (integerSettings.ContainsKey(key))
            {
                return integerSettings[key];
            }

            WarnDeveloperIfSettingIsAlreadySavedToAnotherType<int>(key);
            Bindable<int> newBinding = new Bindable<int>(key, defaultValue);
            integerSettings.Add(key, newBinding);

            //TODO: Find the settings menu and automatically push
            // the binding to be rendered

            return newBinding;
        }

        /// <summary>
        /// Fetches a setting binding or creates a new one if it doesn't exist
        /// for string settings.  This method is overloaded because dynamically
        /// returning generic values in boxes is poorly supported
        /// </summary>
        /// <param name="key">The setting the binding comes from</param>
        /// <param name="defaultValue">The default value if a new setting is created</param>
        /// <returns>A boxed float that updates universally across gameobjects</returns>
        public Bindable<float> Register(string key, float defaultValue)
        {
            if (floatSettings.ContainsKey(key))
            {
                return floatSettings[key];
            }

            WarnDeveloperIfSettingIsAlreadySavedToAnotherType<float>(key);
            Bindable<float> newBinding = new Bindable<float>(key, defaultValue);
            floatSettings.Add(key, newBinding);

            //TODO: Find the settings menu and automatically push
            // the binding to be rendered

            return newBinding;
        }

        /// <summary>
        /// Fetches a setting binding or creates a new one if it doesn't exist
        /// for string settings.  This method is overloaded because dynamically
        /// returning generic values in boxes is poorly supported
        /// </summary>
        /// <param name="key">The setting the binding comes from</param>
        /// <param name="defaultValue">The default value if a new setting is created</param>
        /// <returns>A boxed string that updates universally across gameobjects</returns>
        public Bindable<string> Register(string key, string defaultValue)
        {
            if (stringSettings.ContainsKey(key))
            {
                return stringSettings[key];
            }

            WarnDeveloperIfSettingIsAlreadySavedToAnotherType<string>(key);
            Bindable<string> newBinding = new Bindable<string>(key, defaultValue);
            stringSettings.Add(key, newBinding);

            //TODO: Find the settings menu and automatically push
            // the binding to be rendered

            return newBinding;
        }

        /// <summary>
        /// update a setting with a given value and update all subscribers to that value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public void Modify<T>(string key, T value)
        {
            // update the value to the correct dictionary
            if (value is string strValue)
            {
                stringSettings[key].Value = strValue;
            }

            else if (value is float floatValue)
            {
                floatSettings[key].Value = floatValue;
            }

            else if (value is int intValue)
            {
                integerSettings[key].Value = intValue;
            }

            // failed to update, notify developer
            else
            {
                Development.Logger.Warn($"Tried modifying a setting with an unsupported type {typeof(T).Name}");
            }
        }

        /// <summary>
        /// A sequence of build-settings to prebuild into the settings
        /// editor
        /// </summary>
        public void PreComputeSettings()
        {
            //TODO: when the game is first launched, the settings
            // menu will only have access to the settings that are
            // in that scene.  Adding a precompute step lets users
            // change settings before they encounter them in gameplay
        }

        /// <summary>
        /// A helper method that sends a binding and the expected way
        /// to render it to the settings menu.
        /// </summary>
        /// <typeparam name="T">The type of data stored inside the binding</typeparam>
        /// <param name="binding">The data binding the setting updates</param>
        /// <param name="renderSettings">How the binding should be rendered</param>
        private void RegisterToSettingsMenu<T>(Bindable<T> binding, BindingRenderSettings renderSettings)
        {
            // TODO: helper method that sends the binding creation request
            // to the settings manager
        }

        /// <summary>
        /// Check if a consumer is trying to register a binding to the wrong
        /// data type.  The consumer is allowed to do so, but a warning will log
        /// to the console.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        private void WarnDeveloperIfSettingIsAlreadySavedToAnotherType<T>(string key)
        {
            bool inStringSettings = false;
            bool inIntSettings = false;
            bool inFloatSettings = false;

            // search all three dictionaries to see which ones contain this key
            if (stringSettings.ContainsKey(key))
            {
                inStringSettings = true;
            }

            if (floatSettings.ContainsKey(key))
            {
                inFloatSettings = true;
            }

            if (integerSettings.ContainsKey(key))
            {
                inIntSettings = true;
            }

            // log if this key is in a dictionary of another type
            if (typeof(T) == typeof(string) && (inIntSettings || inFloatSettings))
            {
                Development.Logger.Warn($"Tried registering a key {key} of type string, but it was found to be saved under another type.");
            }

            if (typeof(T) == typeof(int) && (inStringSettings || inFloatSettings))
            {
                Development.Logger.Warn($"Tried registering a key {key} of type int, but it was found to be saved under another type.");
            }

            if (typeof(T) == typeof(float) && (inStringSettings || inStringSettings))
            {
                Development.Logger.Warn($"Tried registering a key {key} of type float, but it was found to be saved under another type.");
            }
        }

        /// <summary>
        /// Remove this from the IoC container at shutdown
        /// </summary>
        void OnDestroy()
        {
            ioc.ExecuteCleanup();
        }

    }
}