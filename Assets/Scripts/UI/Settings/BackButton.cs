using Events;
using UnityEngine;

public class BackButton : MonoBehaviour
{
    [Tooltip("The event channel that raises requests for the settings event channel")]
    [SerializeField] BoolEventChannelSO settingsEventChannel;

    public void OnPress()
    {
        if (settingsEventChannel == null)
        {
            Development.Logger.Warn("A channel was not configured to raise menu events on this button.  Did you remember to configure it in the editor?");
            return;
        }

        settingsEventChannel.Raise(false);
    }
}
