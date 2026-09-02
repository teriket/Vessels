using System.Collections;
using System.Collections.Generic;
using Control;
using UnityEngine;

namespace DataStructures
{
    public class AutoRegisterGameObjectToIoC : MonoBehaviour
    {
        private IoCContainer iocContainer;

        void Awake()
        {
            iocContainer = IoCContainer.GetInstance();
            if (iocContainer != null)
            {
                iocContainer.RegisterGameObject(this.gameObject);
                return;
            }

            Development.Logger.Warn($"Tried to auto-register {this.gameObject.name}, but could not find the IoC container");
        }

        void OnDestroy()
        {
            if (iocContainer is null)
            {
                Development.Logger.Warn($"Tried to remove gameobject {gameObject.name} from the IoC container, but could not find the container");
            }

            iocContainer.ForgetGameObject(this.gameObject);
        }
    }
}