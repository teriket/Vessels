using System.Runtime.CompilerServices;
using System.IO;
using UnityEngine;

namespace Development{
/// <summary>
/// A standardized mechanism for logging issues that require developer
/// attention.  This is not a mechanism to provide feedback to users about
/// user error.
/// </summary>
public class Logger
{
    /// <summary>
    /// Log a low-priority error message with the calling class and method.  May optionally
    /// print the calling gameobject.
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <param name="go">An optional parameter.  The gameobject that implements this behaviour</param>
    /// <param name="callingMethod">An automatically generated parameter.  Determines which method called this one.</param>
    /// <param name="filePath">An automatically generated parameter.  Determines which class called this one</param>
    public static void Warn(string message, GameObject go = null, [CallerMemberName] string callingMethod = "", [CallerFilePath] string filePath = "")
    {
        string className = Path.GetFileNameWithoutExtension(filePath);
        
        if(go == null)
        {
            Debug.Log($"{className}.{callingMethod}() WARNING: {message}");
        }
        else
        {
            Debug.Log($"{go.name}.{className}.{callingMethod}() WARNING: {message}");
        }
        // in the future, send the warning to a logging service
    }

    /// <summary>
    /// Log a high-priority error message with the calling class and method.  May optionally
    /// print the calling gameobject.
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <param name="go">An optional parameter.  The gameobject that implements this behaviour</param>
    /// <param name="callingMethod">An automatically generated parameter.  Determines which method called this one.</param>
    /// <param name="filePath">An automatically generated parameter.  Determines which class called this one</param>
    public static void CriticalMessage(string message, GameObject go = null, [CallerMemberName] string callingMethod = "", [CallerFilePath] string filePath = "")
    {
        string className = Path.GetFileNameWithoutExtension(filePath);
        
        if(go == null)
        {
            Debug.Log($"{className}.{callingMethod}() CRITICAL ERROR: {message}");
        }
        else
        {
            Debug.Log($"{go.name}.{className}.{callingMethod}() CRITICAL ERROR: {message}");
        }
        // in the future, create a mechanism to safely exit the scene from irrecoverable errors
        // in the future, send the critical error to a logging service
    }
}
}