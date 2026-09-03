using System;
using UnityEngine;

namespace DataStructures{
/// <summary>
/// A wrapper class for string, float, and int used in saving and updating player preferences.
/// All set operations automatically save the new value to player preferences.  This should
/// not be used to store sensitive data or data that it is critical always persists.  Modifying
/// the stored data in Bindable will be reflected in all other listeners to this object.
/// </summary>
/// <typeparam name="T">string, float, or int</typeparam>
public class Bindable<T>
{
    private readonly string saveKey;
    private T currentValue;

    public T Value
    {
        get => currentValue;
        set
        {
            currentValue = value;
            SaveToPrefs();
        }
    }

    public Bindable(string key, T defaultValue)
    {
        saveKey = key;
        currentValue = defaultValue;
        LoadFromPrefs();
    }

    private void LoadFromPrefs()
    {
        if (currentValue is int defaultInt)
        {
            int savedValue = PlayerPrefs.GetInt(saveKey, defaultInt);
            currentValue = (T)(object)savedValue;

        }

        else if (currentValue is float defaultFloat)
        {
            float savedValue = PlayerPrefs.GetFloat(saveKey, defaultFloat);
            currentValue = (T)(object)savedValue;
        }

        else if (currentValue is string defaultString)
        {
            string savedValue = PlayerPrefs.GetString(saveKey, defaultString);
            currentValue = (T)(object)savedValue;
        }

        else
        {
            Development.Logger.Warn($"Tried loading a player preference {saveKey}, but Bindable loads the wrong type ({typeof(T).Name})");
        }
    }

    private void SaveToPrefs()
    {
        if (currentValue is int currentIntValue)
        {
            PlayerPrefs.SetInt(saveKey, currentIntValue);
        }

        else if (currentValue is float currentFloatValue)
        {
            PlayerPrefs.SetFloat(saveKey, currentFloatValue);
        }

        else if (currentValue is string currentSaveValue)
        {
            PlayerPrefs.SetString(saveKey, currentSaveValue);
        }

        else
        {
            Development.Logger.Warn($"Tried updating a player preference {saveKey}, but implements the wrong type");
        }
    }

}
}