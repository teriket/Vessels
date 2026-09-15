using UnityEngine;

namespace Config
{
    /// <summary>
    /// constant strings used internally for consistent
    /// message passing between scripts, subscribers, settings, etc.
    /// Secrets should not be stored inside this folder and
    /// player-facing strings should not be stored inside this
    /// folder
    /// </summary>
    public static class Strings
    {
        // // --------------- SETTINGS ---------------
        // // ----------------------------------------
        // // camera -----------
        // public const string SETTING_CAMERA_MOVE_SENSITIVITY = "camera_movement_sensitivity";
        // public const string SETTING_CAMERA_ZOOM_SENSITIVITY = "camera_zoom_sensitivity";

        // // audio -----------
        // public const string SETTING_MASTER_VOLUME = "master_volume";
        // public const string SETTING_MUSIC_VOLUME = "music_volume";
        // public const string SETTING_SOUND_EFFECTS_VOLUME = "sound_effect_volume";
        // public const string SETTING_AMBIENCE_VOLUME = "ambience_volume";
        // public const string SETTING_DIALOG_VOLUME = "dialog_volume";

        // // graphics -----------
        // public const string SETTING_BRIGHTNESS = "brightness";


        // // gameplay -----------
        // public const string SETTING_LANGUAGE = "language";


        // // languages ----------
        public const string SETTING_LANGUAGE_ENGLISH = "English";
        public const string SETTING_LANGUAGE_CHINESE = "Simplified Chinese";
        public const string SETTING_LANGUAGE_DUTCH = "Dutch";
        public const string SETTING_LANGUAGE_GERMAN = "German";
        public const string SETTING_LANGUAGE_SPANISH = "Spanish";
        public const string SETTING_LANGUAGE_PORTUGUESE = "Portuguese";
        public const string SETTING_LANGUAGE_RUSSIAN = "Russian";
        public const string SETTING_LANGUAGE_JAPANESE = "Japanese";
        public const string SETTING_LANGUAGE_KOREAN = "Korean";
        public const string SETTING_LANGUAGE_POLISH = "Polish";
        public const string SETTING_LANGUAGE_TURKISH = "Turkish";
        public const string SETTING_LANGUAGE_UKRANIAN = "Ukranian";


        // -------- IOC GAMEOBJECT NAMES ----------
        // ----------------------------------------
        public const string IOC_MAIN_CAMERA = "MainCamera";
        public const string IOC_PLAYER = "PlayerModel";

        // ------------- Menu Types ---------------
        // ----------------------------------------
        public const string SETTING_VIEW_SLIDER = "Slider";
        public const string SETTING_VIEW_CHECKBOX = "Checkbox";
        public const string SETTING_VIEW_DROPDOWN = "Dropdown";
        public const string SETTING_VIEW_KEYBINDING = "Keybinding";
        public const string SETTING_VIEW_CATEGORY = "Category";

    }
}