using Events;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Controls animations and the visibility of settings.  Listens to
    /// and broadcasts menu events for the settings menu.
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The master event channel that broadcasts the expected state menus should be in.")]
        MenuContextEventChannelSO masterMenuChannel;

        void Awake()
        {
            if (masterMenuChannel == null)
            {
                Development.Logger.Warn("Null menu channel listener.  Did you remember to configure the event listener in the unity editor?");
                return;
            }

            masterMenuChannel.OnRaised += OnMenuContextChange;
        }

        /// <summary>
        /// Activate or deactivate this menu based on the broadcasted
        /// expected menu
        /// </summary>
        /// <param name="context"></param>
        void OnMenuContextChange(MenuContext context)
        {
            if (context != MenuContext.SETTINGS)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }
    }
}