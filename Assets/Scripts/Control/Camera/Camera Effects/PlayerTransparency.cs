
using DataStructures.IOC;
using UnityEngine;
using Config;
using System;

namespace Control.Camera
{
    [Serializable]
    public class PlayerTransparency : ICameraEffect, IIoCComponent<PlayerTransparency>
    {
        public IoCContainer iocContainer { get; set; }
        private IIoCComponent<PlayerTransparency> ioc => this;

        GameObject player;
        Transform playerTransform;
        [Tooltip("How close the camera can get to the player before it starts to fade their material")]
        [SerializeField] float fadeDistance = 5;

        [Tooltip("How low the players alpha can go")]
        [SerializeField] float maxFade = 0.1f;

        Material playerMaterial;
        float alpha;

        public void Initialize()
        {
            iocContainer = IoCContainer.GetInstance();

            ioc.RequestGameObject(Strings.IOC_PLAYER, out player);

            if (player != null)
            {
                playerTransform = player.transform;
                playerMaterial = player.GetComponent<MeshRenderer>().material;
            }
        }

        /// <summary>
        /// Make the players model more transparent as the camera zooms closer
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="lookAtPosition"></param>
        public void ActivateEffect(Vector3 targetPosition, Vector3 lookAtPosition)
        {
            const float NO_FADE = 1;

            float distance = Vector3.Distance(playerTransform.position, targetPosition);
            alpha = Mathf.Lerp(maxFade, NO_FADE, distance / fadeDistance);

            // keep the players old color while updating the alpha
            playerMaterial.color = new Color(
                playerMaterial.color[0],
                playerMaterial.color[1],
                playerMaterial.color[2],
                alpha
            );
        }

        /// <summary>
        /// Determine if the camera is close enough to start fading the player models alpha
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="lookatPosition"></param>
        /// <returns></returns>
        public bool ShouldTrigger(Vector3 targetPosition, Vector3 lookatPosition)
        {
            // try to repair the player gameobject reference if it can't be found
            if (player == null && !ioc.TryRepairGameObject(Strings.IOC_PLAYER, out player))
            {

                Development.Logger.Warn("Could not modify the players transparency because the player could not be found");
                return false;
            }

            // try to repair the player transform reference if it couldn't be found
            if (playerTransform == null)
            {
                playerTransform = player.transform;
                if (playerTransform == null)
                {
                    Development.Logger.Warn("Could not find the players transform");
                    return false;
                }
            }

            // try to repair the player material reference if it couldn't be found
            if (playerMaterial == null)
            {
                playerMaterial = player.GetComponent<MeshRenderer>().material;
                if (playerMaterial == null)
                {
                    Development.Logger.Warn("Could not find the players material");
                    return false;
                }
            }

            return true;
        }
    }
}