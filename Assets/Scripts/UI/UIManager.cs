using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Control.Controller;
using Events;

namespace UI
{
    /// <summary>
    /// Delegates menu events based on player inputs, changes the input context appropriately, and
    /// manages any logic surrounding coordinating multiple menus.  Individual menus should have
    /// a MenuContextEventChanelSO listening for the current menu context to enable /
    /// disable themselves, while this monobehaviour will raise the appropriate menu context changes.
    /// Only this class should publish events to the menu context event channel, and the outputs
    /// from that channel should be treated as an authority on what menus should be open.  Messages
    /// in other menu channels should be treated as requests that may or may not be respected.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        private PlayerControls controls;

        // event channel that represents the master state the menu should be in at any given time, raised by this class 
        [SerializeField]
        [Tooltip("The master channel which broadcasts the expected state menus should be in")]
        private MenuContextEventChannelSO menuContextToggleChannel;

        // event channel for receiving notifications about changes to the settings menu
        [SerializeField]
        [Tooltip("An event channel that receives requests to open and close the settings menu")]
        private BoolEventChannelSO settingsMenuChannel;

        // event channel for receiving notifications about changes to the main pause menu
        [SerializeField]
        [Tooltip("An event channel that receives requests to open and close the pause menu.")]
        private BoolEventChannelSO pauseMenuChannel;

        // keeps track of the UI stack the player has navigated through, used for navigating back in menus
        private Stack<MenuContext> menuPath = new();


        void Awake()
        {
            controls = ControlManager.GetController();

            // the default value on the menus stack.
            menuPath.Push(MenuContext.NONE);

            // set a listener to changes in other menus
            if (settingsMenuChannel != null)
            {
                settingsMenuChannel.OnRaised += SettingsMenuToggle;
            }
            else
            {
                Development.Logger.Warn("A channel for listening to settings menu events was not configured.  Did you remember to assign it in the Unity inspector?");
            }

            if (pauseMenuChannel != null)
            {
                pauseMenuChannel.OnRaised += PauseMenuToggle;
            }
            else
            {
                Development.Logger.Warn("A channel for listening to pause menu events was not configured.  Did you remember to assign it in the Unity inspector?");
            }
        }

        void Start()
        {
            // Notify other menus that they shouldn't be activated when they load in.
            menuContextToggleChannel.Raise(MenuContext.NONE);
        }

        void OnEnable()
        {
            // register for input events
            controls.Gameplay.MenuToggle.performed += AcceptKeybindInput;
            controls.UI.MenuToggle.performed += AcceptKeybindInput;
        }

        void OnDisable()
        {
            // disable inputs when this object is disabled
            controls.Gameplay.MenuToggle.performed -= AcceptKeybindInput;
            controls.UI.MenuToggle.performed -= AcceptKeybindInput;
        }

        /// <summary>
        /// Toggles the current menu when a player enters the correct keybinding.  Either
        /// opens the initial menu if no menus are open, or closes the highest menu in the
        /// menu stack to navigate back a menu (to potentially no menus)
        /// </summary>
        /// <param name="callbackContext"></param>
        private void AcceptKeybindInput(InputAction.CallbackContext callbackContext)
        {
            if (menuPath.Peek() == MenuContext.NONE)
            {
                OpenMenu(MenuContext.PAUSE_MENU);
            }
            else
            {
                CloseMenu();
            }
        }

        /// <summary>
        /// Change the currently selected menu
        /// </summary>
        /// <param name="context"></param>
        void OpenMenu(MenuContext context)
        {
            // error-guarding clause.  This could insert a no-menu is open onto the stack of menus
            // which would make back-navigation weird.  The None type should only be the first in the
            // stack, so trying to exit all menus at once should clear the stack rather than insert into it
            if (context == MenuContext.NONE)
            {
                menuPath.Clear();
            }

            menuPath.Push(context);
            menuContextToggleChannel.Raise(context);
        }

        void CloseMenu()
        {
            // error-catching clause, in case a menu is tried to be closed when no menus are open.  The NONE type is the first entry in the stack,
            // so results aren't guaranteed if this is removed.
            if (menuPath.Count == 1)
            {
                Development.Logger.Warn("An improper call to close a menu was made.  Attempting to close a nonopen menu empties the menus stack and cause null issues later.");
                // rebroadcasting the currently selected menu, in the case some state has been corrupted
                menuContextToggleChannel.Raise(menuPath.Peek());
                return;
            }

            // navigate backwards a menu, which may be to no menus are open
            menuPath.Pop();
            menuContextToggleChannel.Raise(menuPath.Peek());

        }

        void OnDestroy()
        {
            // unregister from keybind listeners
            controls.Gameplay.MenuToggle.performed -= AcceptKeybindInput;
            controls.UI.MenuToggle.performed -= AcceptKeybindInput;

            // unregister from menu request events
            settingsMenuChannel.OnRaised -= SettingsMenuToggle;
            pauseMenuChannel.OnRaised -= PauseMenuToggle;
        }

        /// <summary>
        /// Logic that executes when an object requests to toggle the settings menu.
        /// </summary>
        /// <param name="isToggled"></param>
        void SettingsMenuToggle(bool isToggled)
        {
            if (isToggled)
            {
                OpenMenu(MenuContext.SETTINGS);
            }
            else
            {
                CloseMenu();
            }
        }

        /// <summary>
        /// Logic that executres when the pause menu raises a request to change menus.
        /// </summary>
        /// <param name="isToggled"></param>
        void PauseMenuToggle(bool isToggled)
        {
            if (isToggled)
            {
                OpenMenu(MenuContext.PAUSE_MENU);
            }
            else
            {
                CloseMenu();
            }
        }
    }
}