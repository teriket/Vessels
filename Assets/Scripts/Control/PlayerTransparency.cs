using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
Author:         Tanner Hunt
Date:           5/3/2024
Version:        0.1.0
Description:    Increases the player transparency as the camera gets closer to them
ChangeLog:      V 0.1.0 -- 5/3/2024
                    --Implemented Code
                    --Dev time: 0.25 Hours
*/
public class PlayerTransparency : MonoBehaviour
{
    Transform playerCam;
    Material playerMaterial;
    float alpha;
    [SerializeField] float fadeDistance;
    // Start is called before the first frame update
    void Start()
    {
        alpha = 1;
        playerCam = transform.parent.GetChild(1);
        playerMaterial = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        alpha = Mathf.Clamp01(
            Vector3.Distance(transform.position, playerCam.position) / fadeDistance
        );
        playerMaterial.color = new Color(
            playerMaterial.color[0],
            playerMaterial.color[1],
            playerMaterial.color[2],
            alpha
        );
    }
}
