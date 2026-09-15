using UnityEngine;
using Events;

namespace UI
{
    public class ResumeButton : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("An event channel that broadcasts requests to open/close the pause menu")]
        BoolEventChannelSO pauseMenuToggleChannel;

        public void OnPress()
        {
            if (pauseMenuToggleChannel != null)
            {
                pauseMenuToggleChannel.Raise(false);
            }
            else
            {
                Development.Logger.Warn("Null escape menu toggle.  Did you remember to attach a BoolEventChannelSO instance to this button?");
            }
        }
    }
}