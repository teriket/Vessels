using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataStructures
{
    /// <summary>
    /// Pregenerated helper methods for MonoBehaviours to quickly get a reference to the IoC Container
    /// and safely repair broken references.  Implementing classes should call this interfaces OnDestroy
    /// method when they are destroyed.
    /// 
    /// A proper setup looks like this:
    /// <code>
    /// public class MyClass : IIoCComponent&lt;TypeToRegisterAs&gt;{
    ///     IoCContainer iocContainer {get; set;}
    ///     private IIoCComponent&lt;TypeToRegisterAs&gt; ioc =&gt; this;
    /// 
    /// void Start(){
    ///     ioc.InitializeIoCContainer(this);
    ///     ioc.MethodCallFromHere()
    ///     }
    /// 
    /// void OnDestroy(){
    ///     ioc.ExecuteCleanup();
    /// }
    /// }
    /// </code>
    /// </summary>
    public interface IIoCComponent<T>
    {
        IoCContainer iocContainer { get; set; }

        /// <summary>
        /// Gets a reference to the IoC container and registers this component to it
        /// </summary>
        /// <param name="component">A reference to the component itself</param>
        public void InitializeIoCContainer(T component)
        {
            iocContainer = IoCContainer.GetInstance();

            if (iocContainer != null)
            {
                iocContainer.RegisterComponent<T>(component);
            }
            else
            {
                Development.Logger.Warn("Failed to find an IoC Container at startup");
            }
        }

        /// <summary>
        /// Tries to get a reference to the IoC container.  Logs an error if this operation fails.
        /// </summary>
        /// <returns>true if the IoC container was found.  False if it fails to find a container</returns>
        public bool TryRepairIoCContainer()
        {
            iocContainer = IoCContainer.GetInstance();
            if (iocContainer == null)
            {
                Development.Logger.Warn("Attempted to regain a reference to the IoC container, but failed.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tries to repair a null reference to a gameobject found inside the IoC container.  Safely attempts to repair
        /// an IoC reference if it is null.  Logs any errors along the way
        /// </summary>
        /// <param name="name">The name of the gameobject to find</param>
        /// <param name="gameObject">The gameobject to save the data output to.</param>
        /// <returns>True if the gameobject is successfully recovered, false otherwise</returns>
        public bool TryRepairGameObject(string name, out GameObject gameObject)
        {
            if (iocContainer is null && !TryRepairIoCContainer())
            {
                Development.Logger.Warn($"Attempted to regain a reference to a gameobject {name}, but failed to repair a null IoC container");
                gameObject = null;
                return false;
            }

            gameObject = iocContainer.RequestObject(name);
            if (gameObject == null)
            {
                Development.Logger.Warn($"Attempted to regain a reference to a gameobject {name}, but failed to find one inside the IoC container");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to repair a null reference to a component found inside the IoC container.  Safely attempts to repair
        /// an IoC reference if it is null.  Logs any errors along the way.
        /// </summary>
        /// <param name="variable">The component variable to save to</param>
        /// <returns>True if the component is successfully recovered, false otherwise</returns>
        public bool TryRepairComponentReference<J>(out J variable)
        {
            string typeName = typeof(T).Name;
            if (iocContainer is null && !TryRepairIoCContainer())
            {
                Development.Logger.Warn($"Attempted to regain a reference to a component of type {typeName}, but failed to repair a null IoC container");
                variable = default;
                return false;
            }

            variable = iocContainer.RequestComponent<J>();
            if (variable is null)
            {
                Development.Logger.Warn($"Attempted to regain a reference to a component of type {typeName}, but failed to find one inside the IoC container");
            }

            return false;
        }

        /// <summary>
        /// Wrapper method for requesting components from the IoC container.  This has the added benefit
        /// of having a null check for the IoC container and trying to repair it.  Also, implementing classes
        /// will have both an IoCContainer member variable and access to this interfaces methods.  Giving
        /// the implementer access to this wrapper unifies the code so they only need to go through one interface.
        /// </summary>
        /// <typeparam name="T">The type to request</typeparam>
        /// <param name="component">The monobehaviour or interface variable to save the component to.</param>
        public void RequestComponent(out T component)
        {
            if (iocContainer == null && !TryRepairIoCContainer())
            {
                component = default;
                return;
            }

            component = iocContainer.RequestComponent<T>();
        }

        /// <summary>
        /// Wrapper method for requesting objects from the IoC container.  This has the added benefit
        /// of having a null check for the IoC container and trying to repair it.  Also, implementing classes
        /// will have both an IoCContainer member variable and access to this interfaces methods.  Given their
        /// similar names, switching between the two can be confusing.  Giving the implementer access to this 
        /// wrapper unifies the code so they only need to go through one interface.
        /// </summary>
        /// <param name="name">The name of the gameobject to find</param>
        /// <param name="gameObject">The gameobject variable to save the output to</param>
        public void RequestGameObject(string name, out GameObject gameObject)
        {
            if (iocContainer == null && !TryRepairIoCContainer())
            {
                gameObject = null;
                return;
            }

            gameObject = iocContainer.RequestObject(name);
        }

        /// <summary>
        /// Method to call in the components destructor so other objects don't accidentally get a null reference.
        /// </summary>
        public void ExecuteCleanup()
        {
            if (iocContainer != null)
            {
                if (iocContainer.RequestComponent<T>() != null)
                {
                    Development.Logger.Warn($"Tried to remove a component of type {typeof(T).Name} from the IOC container, but no such component exists");
                }

                iocContainer.ForgetComponent<T>();
            }
            else
            {
                Development.Logger.Warn($"Tried to remove a component of type {typeof(T).Name} from the IOC container, but could not find the container");
            }
        }
    }
}