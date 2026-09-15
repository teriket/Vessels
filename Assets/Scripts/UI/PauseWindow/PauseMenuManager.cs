using UnityEngine;
using Events;

namespace UI
{
    /// <summary>
    /// Listens to and publishes events related to the pause menu
    /// </summary>
    public class PauseMenuManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The master event channel that broadcasts the expected state menus should be in.")]
        MenuContextEventChannelSO masterMenuChannel;

        void Awake()
        {
            masterMenuChannel.OnRaised += OnMenuContextChange;
        }

        /// <summary>
        /// Activate or deactivate this menu based on the broadcasted
        /// expected menu
        /// </summary>
        /// <param name="context"></param>
        void OnMenuContextChange(MenuContext context)
        {
            if (context != MenuContext.PAUSE_MENU)
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