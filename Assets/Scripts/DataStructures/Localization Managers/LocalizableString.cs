using UnityEngine;
using System;
using Development;
using DataStructures.IOC;
using Unity.VisualScripting;

namespace DataStructures.Localization
{
    /// <summary>
    /// Data binding that updates based on the users selected language.  <br/><br/> Use the Display() method
    /// to show the localized language string.
    /// </summary>
    [Serializable]
    public class LocalizableString : ISerializationCallbackReceiver
    {
        [SerializeField] private string id;
        [SerializeField] public string value = "";

        [DoNotSerialize] public string displayValue = null;
        bool initialized = false;

        public string Display()
        {
            if (!initialized)
            {
                IoCContainer.GetInstance().RequestComponent<ILocalizationManager>().SubscribeToLanguageChanges(this);
                initialized = true;
            }

            // null display value suggests there was a failure in the LocalizationManager to pull
            // the selected language, so revert to base language implementation via the value field (which is set in the Unity editor)
            if (displayValue == null)
            {
                return value;
            }


            return displayValue;
        }

        public void SetDisplay(string localizedValue)
        {
            this.displayValue = localizedValue;
        }

        public string GetId()
        {
            return id;
        }

        // Operator overloading to allow LocalizableString to be treated like strings in code
        public static implicit operator string(LocalizableString localizableString) => localizableString?.value;
        public static implicit operator LocalizableString(string str) => new LocalizableString { value = str };

        public override bool Equals(object obj) => Equals(obj as LocalizableString);

        public bool Equals(LocalizableString other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return this.value == other.value;
        }

        public override int GetHashCode()
        {
            // C# injects randomness into their string hash codes (as a security feature against user-inserted strings)
            // 

            unchecked
            {
                // FNV-1a constants for 32-bit offsets
                const uint fnvPrime = 16777619;
                const uint fnvOffsetBasis = 2166136261;

                uint hash = fnvOffsetBasis;

                // Iterate over characters using UTF-16 code units (safe and fast for C# strings)
                foreach (char c in value)
                {
                    // Process the lower byte of the char
                    hash ^= (byte)(c & 0xFF);
                    hash *= fnvPrime;

                    // Process the upper byte of the char
                    hash ^= (byte)(c >> 8);
                    hash *= fnvPrime;
                }

                return (int)hash;
            }
        }

        // serializing any complex data before unity saves it to its  yaml file.
        // not necessary in this case
        public void OnBeforeSerialize()
        {

        }

        // triggered right after unity injects yaml data into the fields, but before Awake() or OnValidate().
        // only auto-generate this value if unity doesn't inject a pre-saved value
        public void OnAfterDeserialize()
        {
            if (id == null || id == "")
            {
                Guid uuid = Guid.NewGuid();
                id = uuid.ToString();
            }
        }

        ~LocalizableString()
        {
            IoCContainer ioc = IoCContainer.GetInstance();
            if (ioc != null)
            {
                ILocalizationManager localizations = ioc.RequestComponent<ILocalizationManager>();

                if (localizations != null)
                {
                    localizations.UnsubscribeToLanguageChanges(this);
                    return;
                }
            }
        }
    }
}