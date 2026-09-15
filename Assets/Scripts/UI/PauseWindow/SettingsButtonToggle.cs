using Events;
using UnityEngine;

public class SettingsButtonToggle : MonoBehaviour
{
    [SerializeField]
    [Tooltip("An event channel that broadcasts requests to open/close the pause menu")]
    BoolEventChannelSO settingsMenuToggleChannel;

    public void OnPress()
    {
        if (settingsMenuToggleChannel != null)
        {
            settingsMenuToggleChannel.Raise(true);
        }
        else
        {
            Development.Logger.Warn("Null setting menu toggle.  Did you remember to attach a BoolEventChannel in the unity editor?");
        }
    }
}
