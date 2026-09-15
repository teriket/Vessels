using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class QuitGame : MonoBehaviour
    {
        /// <summary>
        /// Quits the application.
        /// </summary>
        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }
    }
}