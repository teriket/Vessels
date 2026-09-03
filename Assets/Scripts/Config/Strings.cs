using UnityEngine;

namespace DataStructures
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
        // --------------- SETTINGS ---------------
        // ----------------------------------------
        // CAMERA -----------
        public const string SETTING_CAMERA_MOVE_SENSITIVITY = "camera_movement_sensitivity";
        public const string SETTING_CAMERA_ZOOM_SENSITIVITY = "camera_zoom_sensitivity";

        // AUDIO -----------
        public const string SETTING_MASTER_VOLUME           = "master_volume";
        public const string SETTING_MUSIC_VOLUME            = "music_volume";
        public const string SETTING_SOUND_EFFECTS_VOLUME    = "sound_effect_volume";
        public const string SETTING_AMBIENCE_VOLUME         = "ambience_volume";
        public const string SETTING_DIALOG_VOLUME           = "dialog_volume";

        // GRAPHICS -----------
        public const string SETTING_BRIGHTNESS              = "brightness";


        // GAMEPLAY -----------
        public const string SETTING_LANGUAGE                = "language";

    }
}