using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Control;
using DataStructures.IOC;
using DataStructures.Settings;
using Config;

namespace DataStructures.Localization
{
    /// <summary>
    /// File and data manager for string localizations.  This iteration stores all strings for the current language
    /// in memory, so should be used for games with a light text footprint for memory optimizations.  Better performance
    /// for reducing disk IO between scenes and loads.  Does not support text fields with newline characters or strings
    /// with the pipe character in them.  Does not save new LocalizableStrings to the database.  That should be done
    /// using a separate script that searches unities yaml files (I tried it in here, it was a nightmare)
    /// </summary>
    public class CSVLocalizationsManager : MonoBehaviour, ILocalizationManager, IReloadable
    {
        private string filePath = Path.Combine(Application.dataPath, "localizations.json");

        // english hash-value key, data binding with display value
        private HashSet<LocalizableString> displayStringBindings = new();

        // intermediate data structure to store the current languages strings
        private Dictionary<string, string> storedStrings = new();

        protected static CSVLocalizationsManager instance { get; private set; }

        // putting a pin into this property.  It may be necessary if localizable strings are subscribing
        // before the data is properly loaded to put them into a queue, then subscribe them to the datastructure
        // private Queue<LocalizableString> awaitingToBeBoundToData;

        [SerializeField] StringBindingSO language;

        bool isLoaded = false;

        // TODO: A better implementation would have a custom dropdown of loaded languages to select
        // as the default language.
        private const string defaultLanguage = Strings.SETTING_LANGUAGE_ENGLISH;

        // TODO: There is maybe a way to make sure the delimiter selected here is the same as the one
        // in the csv file.  Gemini recommends doing character-frequency analysis (yikes), or
        // a built-in configuration with CsvHelper.  Maybe I could stick to the first line is sep={delimiter}
        // to determine this
        private const char DELIMITER = '|';

        /// <summary>
        /// Manage singleton access to this component.  Then, register this gameobject to
        /// the IoC container
        /// </summary>
        void Awake()
        {
            // manage singleton access of the localization settings
            if (instance != null && instance != this)
            {
                Development.Logger.Warn("Detected duplicate instances of the Localizations Manager, deleting the duplicate instance");
                Destroy(this);
            }

            instance = this;

        }

        /// <summary>
        /// Subscribe to the language settings.  Create a localizations folder if in development mode,
        /// then load any localization data from memory.
        /// </summary>
        void Start()
        {

            if(language == null)
            {
                Development.Logger.Warn("Could not find a binding to the currently configured language binding.  Did you remember to attach the ScriptableObject in the editor?");
                return;
            }

            LoadLocalizations();
            isLoaded = true;

        }

        /// <summary>
        /// Load localizations for user-facing strings based on the currently selected language
        /// </summary>
        public void LoadLocalizations()
        {
            if (!File.Exists(filePath))
            {
                Development.Logger.Warn("Was unable to find localizations to update string text");
                return;
            }

            using (StreamReader reader = new StreamReader(filePath))
            {
                string headerLine = reader.ReadLine();

                // Escape early if the headers are incorrectly formatted
                if (headerLine == null || headerLine.Trim().Length == 0)
                {
                    Development.Logger.Warn("Localizations file is missing its header row");
                    return;
                }

                // Get the column index for the currently selected language
                string[] headers = headerLine.Split(DELIMITER);
                int selectedLanguageColumnIndex = -1;

                for (int i = 0; i < headers.Length; i++)
                {
                    if (headers[i] == language.Value)
                    {
                        selectedLanguageColumnIndex = i;
                        break;
                    }
                }

                if (selectedLanguageColumnIndex == -1)
                {
                    Development.Logger.Warn($"Could not find a column in the localizations.csv with for the selected language {language.Value}");
                }

                // save each local string variant to the strings dictionary for its ID key
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    // skip empty lines
                    if (line == null || line.Trim().Length == 0)
                    {
                        continue;
                    }

                    // split the row ID and languages
                    string[] rowData = line.Split(DELIMITER);

                    string id = rowData[0];
                    string localLanguageString = rowData[selectedLanguageColumnIndex];

                    // cache the string for the selected language against the ID
                    storedStrings[id] = localLanguageString;
                }
            }

        }

        /// <summary>
        /// Change the language stored in-memory.  Prevents new LocalizableStrings from
        /// subscribing while the data is reloading.
        /// </summary>
        public void OnReload()
        {
            isLoaded = false;

            // clear the previously cached language data, then pull the new data in
            storedStrings.Clear();
            LoadLocalizations();

            // remove any LocalizableStrings that have been destroyed and not properly cleaned up before updating observers
            displayStringBindings.RemoveWhere(item => item == null);

            // update all loaded strings with the localized version for the selected language
            // need to consider data consistency and safety across scene changes for this entire system
            foreach (LocalizableString localization in displayStringBindings)
            {
                UpdateStringBinding(localization);
            }

            isLoaded = true;

        }

        /// <summary>
        /// A method for LocalizableStrings to subscribe to be notified of language changes.  Logs
        /// a developer warning if localizable strings are subscribing before any data is loaded,
        /// and will fail to subscribe the string.
        /// </summary>
        /// <param name="str"></param>
        public void SubscribeToLanguageChanges(LocalizableString str)
        {
            // It's dangerous to try subscribing before the data structure is finished with IO operations
            if (!isLoaded)
            {
                Development.Logger.Warn("Trying to subscribe a localizable string while the data structure is loading");
                return;
            }

            // null parameters, booo!
            if (str == null)
            {
                Development.Logger.Warn("Trying to subscribe a null localizable string to this manager");
                return;
            }

            // subscribe to be updated when the language is changed
            displayStringBindings.Add(str);

            // initial updates to the string binding
            UpdateStringBinding(str);
        }

        /// <summary>
        /// Helper method that safely updates the text to display to the user for a given language.  Safely
        /// defaults to the strings english text if it fails to update and logs an error.
        /// </summary>
        /// <param name="str">The localizable string to update to the current language</param>
        private void UpdateStringBinding(LocalizableString str)
        {
            if (str == null)
            {
                Development.Logger.Warn("Trying to update the bindings on a null localizable string");
                return;
            }

            // determine what string to show to the user
            string stringToDisplayForCurrentLanguage;
            if (storedStrings.ContainsKey(str.GetId()))
            {
                stringToDisplayForCurrentLanguage = storedStrings[str.GetId()];
            }
            else
            {
                Development.Logger.Warn($"Could not update the language for stored string with value {str.value}");
                stringToDisplayForCurrentLanguage = str.value;
            }

            // set the display to whatever language the player has selected first
            str.SetDisplay(stringToDisplayForCurrentLanguage);
        }

        public void UnsubscribeToLanguageChanges(LocalizableString str)
        {
            displayStringBindings.Remove(str);
        }
    }
}