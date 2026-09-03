using UnityEngine;
using DataStructures;

namespace Control
{
    public class MouseManager : MonoBehaviour, IInputDevice, IIoCComponent<IInputDevice>
    {
        Vector3 initialMousePosition;
        // a reference to the actual iocContainer that the helper interface requires
        public IoCContainer iocContainer { get; set; }

        // faster access to helper methods from the IoCComponent interface.  Also includes wrapper
        // functions for the IoCContainer itself with built-in safety checks, so IoC operations should
        // be defaulted to this variable instead of the iocContainer variable.
        IIoCComponent<IInputDevice> ioc => this;

        void Awake()
        {
            ioc.InitializeIoCContainer(this);
        }

        void Start()
        {
            initialMousePosition = Input.mousePosition;
        }

        public Vector2 GetMouseDelta()
        {
            // get the total delta of the mouse from the first frame
            // of gameplay
            return initialMousePosition - Input.mousePosition;
        }

        public Vector2 GetMousePosition()
        {
            return Input.mousePosition;
        }

        public float GetScroll()
        {
            return Input.GetAxis("Mouse ScrollWheel");
        }

        void OnDestroy()
        {
            ioc.ExecuteCleanup();
        }
    }
}