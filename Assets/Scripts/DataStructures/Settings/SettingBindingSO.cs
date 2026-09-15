using System;
using System.Collections.Generic;
using DataStructures.Localization;
using UnityEngine;

namespace DataStructures.Settings
{
    public abstract class SettingBindingSO<T> : SettingSO
    {
        public T Value
        {
            get;
            private set;
        }

        [SerializeField]
        [Tooltip("The default value to save this settings value to if one hasn't been saved")]
        protected T defaultValue;

        [SerializeField]
        [Tooltip("The label this setting uses in the Settings menu")]
        protected LocalizableString label;

        [SerializeField]
        [Tooltip("The key that is saved to the player prefs")]
        protected string saveKey;

        public void Set(T value)
        {
            if (EqualityComparer<T>.Default.Equals(this.Value, value))
            {
                return;
            }

            Value = value;
            SaveToPrefs();
            OnSet?.Invoke(value);
        }

        // Delegate that listeners should subscribe to
        public event Action<T> OnSet;

        /// <summary>
        /// Load the saved player preference value
        /// </summary>
        void OnEnable()
        {
            LoadFromPrefs();
        }

        /// <summary>
        /// Save the currently set value to the player prefs
        /// </summary>
        private void SaveToPrefs()
        {
            if (Value is int currentIntValue)
            {
                PlayerPrefs.SetInt(saveKey, currentIntValue);
            }

            else if (Value is float currentFloatValue)
            {
                PlayerPrefs.SetFloat(saveKey, currentFloatValue);
            }

            else if (Value is string currentSaveValue)
            {
                PlayerPrefs.SetString(saveKey, currentSaveValue);
            }

            else
            {
                Development.Logger.Warn($"Tried updating a player preference {saveKey}, but implements the wrong type");
            }
        }

        /// <summary>
        /// Load the saved player preference for this setting, or generate a new one with the default value
        /// </summary>
        private void LoadFromPrefs()
        {

            if (defaultValue is int defaultInt)
            {
                int savedValue = PlayerPrefs.GetInt(saveKey, defaultInt);
                Value = (T)(object)savedValue;
            }

            else if (defaultValue is float defaultFloat)
            {
                float savedValue = PlayerPrefs.GetFloat(saveKey, defaultFloat);
                Value = (T)(object)savedValue;
            }

            else if (defaultValue is string defaultString)
            {
                string savedValue = PlayerPrefs.GetString(saveKey, defaultString);
                Value = (T)(object)savedValue;
            }

            else
            {
                Development.Logger.Warn($"Tried loading a player preference {saveKey}, but Bindable loads the wrong type ({typeof(T).Name})");
            }
        }
    }
}