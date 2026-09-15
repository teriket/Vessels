using DataStructures.IOC;
using UnityEngine;
using Events;
using UI;

namespace Control.Controller
{
    /// <summary>
    /// Class responsible for listening to events that should
    /// change the input action bindings.
    /// </summary>
    public class ControlManager : MonoBehaviour, IIoCComponent<ControlManager>
    {
        [SerializeField] MenuContextEventChannelSO menuContextChannels;
        private static PlayerControls controls;
        private static ControlManager instance;
        public IoCContainer iocContainer { get; set; }
        IIoCComponent<ControlManager> ioc => this;

        public static PlayerControls GetController()
        {
            if (controls == null)
            {
                SetupController();
            }
            return controls;
        }

        void Awake()
        {
            ioc.InitializeIoCContainer(this);

            if (instance != null && instance != this)
            {
                Destroy(this);
                return;
            }

            instance = this;
            SetupController();
        }

        void Start()
        {

            if (menuContextChannels != null)
            {
                menuContextChannels.OnRaised += OnMenuToggleEvent;
            }
            else
            {
                Development.Logger.Warn("A pause menu toggle is not configured in the control manager.  Did you remember to add the channel in the Unity Editor?");
            }
        }

        private static void SetupController()
        {
            if (controls != null)
            {
                return;
            }

            controls = new PlayerControls();

            // start in gameplay input mode
            controls.Gameplay.Enable();
            controls.UI.Disable();
            Cursor.visible = false;
        }

        private void OnMenuToggleEvent(MenuContext context)
        {
            if (context == MenuContext.NONE)
            {
                controls.Gameplay.Enable();
                controls.UI.Disable();
                Cursor.visible = false;
            }
            else
            {
                controls.Gameplay.Disable();
                controls.UI.Enable();
                Cursor.visible = true;
            }
        }
    }
}