using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Development;


namespace DataStructures.IOC
{
    /// <summary>
    /// IoC container for player objects, follows singleton pattern.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class IoCContainer : MonoBehaviour
    {
        private static IoCContainer instance;
        private Dictionary<string, object> components;
        private Dictionary<string, GameObject> gameObjects;

        /// <summary>
        /// Initialize the ioc container, delete duplicates, then setup
        /// the datastructures the container will be using.
        /// </summary>
        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
            }

            instance = this;

            // initialize the dictionaries
            instance.components = new Dictionary<string, object>();
            instance.gameObjects = new Dictionary<string, GameObject>();
        }

        /// <summary>
        /// Singleton access to the IoC container
        /// </summary>
        /// <returns>Access to the IoC container</returns>
        public static IoCContainer GetInstance()
        {
            return instance;
        }

        /// <summary>
        /// Find a registered component with a given type.
        /// </summary>
        /// <typeparam name="T">The interface or Monobehaviour type to find</typeparam>
        /// <returns>A reference to the type object</returns>
        public T RequestComponent<T>()
        {

            // check if a component of the given type has already registered with the manager
            string typeName = typeof(T).Name;
            if (instance.components.ContainsKey(typeName))
            {
                return (T)instance.components[typeName];
            }

            Development.Logger.CriticalMessage($"Component request for an object of type {typeName} that hasn't registered itself.  Does the script properly register with the PlayerManager in Awake()?");
            return default;
        }

        /// <summary>
        /// Find a registered gameobject with a given name
        /// </summary>
        /// <param name="name">The name of the gameobject in the inspector at Awake()</param>
        /// <returns>A reference to a gameobject or null if one isn't found</returns>
        public GameObject RequestObject(string name)
        {
            // check to see if this gameobject has already been cached
            if (gameObjects.ContainsKey(name))
            {
                return gameObjects[name];
            }

            // this could maybe be made more robust with a recursive search of a transform, but
            // that defeats the purpose of the IoC container
            Development.Logger.CriticalMessage($"Tried fetching a gameobject with name {name} that hasn't registered itself.  Does the gameobject properly register itself in Awake()?");
            return null;
        }

        /// <summary>
        /// Registers a component to the IoC container for a given type.  An example usage might be RegisterComponent<InterfaceIInheritFrom>(this),
        /// or RegisterComponent<MyType>(this).  The underlying dictionary is flat, so RegisterComponent<Type1>(...); RegisterComponent<Type1>(...)
        /// will overwrite the first object.
        /// </summary>
        /// <typeparam name="T">The type to register as</typeparam>
        /// <param name="component">A reference to the component that other objects can point to</param>
        public void RegisterComponent<T>(T component)
        {
            string componentName = typeof(T).Name;
            if (components.ContainsKey(componentName))
            {
                Development.Logger.Warn($"PlayerManager is caching a second reference to {componentName}, effectively hiding the previously registered object");
            }

            components.Add(componentName, component);
        }

        /// <summary>
        /// Registers a gameobject to the IoC container for an object with a given name.  The underlying dictionary is flat, so RegisterGameObject("name");
        /// RegisterGameObject("name"); will overwrite the first object
        /// </summary>
        /// <param name="gameObject"></param>
        public void RegisterGameObject(GameObject gameObject)
        {
            string objectName = gameObject.name;

            if (gameObjects.ContainsKey(objectName))
            {
                Development.Logger.Warn($"PlayerManager is caching a second reference to a gameobject with name {objectName}, effectively hiding the previously registered object");
            }

            instance.gameObjects.Add(objectName, gameObject);
        }

        /// <summary>
        /// Remove a component from the ioc container for a given type.
        /// </summary>
        /// <typeparam name="T">The type of object to remove</typeparam>
        public void ForgetComponent<T>()
        {
            string typeName = typeof(T).Name;
            if (instance.components.ContainsKey(typeName))
            {
                instance.components.Remove(typeName);
            }
            else
            {
                Development.Logger.Warn($"Tried to remove a component of type {typeName} from the IOC container, but couldn't find one.");
            }
        }

        /// <summary>
        /// Remove a gameobject from the ioc container based on its name.
        /// Gameobjects who dynamically update their names at runtime will
        /// fail to be updated.
        /// </summary>
        /// <param name="gameObject">The gameobject to remove</param>
        public void ForgetGameObject(GameObject gameObject)
        {
            string name = gameObject.name;
            if (instance.gameObjects.ContainsKey(name))
            {
                instance.gameObjects.Remove(name);
            }
            else
            {
                Development.Logger.Warn($"Tried to remove a GameObject {name} from the IOC container, but couldn't find one.");
            }
        }

    }
}