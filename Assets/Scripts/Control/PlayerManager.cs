using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Development;

namespace Control
{
    /// <summary>
    /// IoC container for player objects, follows singleton pattern.
    /// </summary>
    public class PlayerManager : MonoBehaviour
    {
        private static PlayerManager instance;

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
            }

            instance = this;
        }

        public static PlayerManager GetInstance()
        {
            return instance;
        }

        public T RequestComponent<T>()
        {
            T[] components = GetComponentsInChildren<T>();

            if (components == null || components.Length == 0)
            {
                Development.Logger.CriticalMessage("could not find an instance of " + typeof(T).Name);
                return default;
            }

            if (components.Length > 1)
            {
                Development.Logger.Warn("found more than one instance of " + typeof(T).Name);
            }

            return components[0];
        }

        public GameObject RequestObject(string tag)
        {
            foreach(Transform child in transform)
            {
                if (child.CompareTag(tag))
                {
                    return child.gameObject;
                }
            }

            Development.Logger.Warn("could not find a gameobject with tag " + tag);
            return null;
        }
    }
}