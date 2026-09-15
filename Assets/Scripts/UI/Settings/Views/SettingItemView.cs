using DataStructures.Settings;
using UnityEngine;
using TMPro;

namespace UI
{
    /// <summary>
    /// Base class for view gameobjects for setting bindings
    /// </summary>
    public abstract class SettingItemView : MonoBehaviour
    {
        [Tooltip("A TMPro object that labels the setting")]
        [SerializeField] GameObject label;

        /// <summary>
        /// Sets the inner text of a text mesh pro object attached to this gameobject
        /// </summary>
        /// <param name="value"></param>
        public void SetLabelContents(string value)
        {
            if (label == null)
            {
                Development.Logger.Warn("The expected TMPro gameobject was not attached to this slider object." +
                "Did you remember to attach the gameobject in the Unity editor?");
                return;
            }

            TextMeshProUGUI tmpro = label.GetComponent<TextMeshProUGUI>();

            if (tmpro == null)
            {
                Development.Logger.Warn("The attached label gameobject did not have a TextMeshPro component.");
                return;
            }

            tmpro.SetText(value);
        }

    }
}