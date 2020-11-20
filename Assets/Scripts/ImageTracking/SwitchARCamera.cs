using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
public class SwitchARCamera : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private ARSessionOrigin session;

    ARCameraManager cameraManager;

    void Start()
    {
        cameraManager = session.GetComponentInChildren<ARCameraManager>();
        cameraManager.requestedFacingDirection = CameraFacingDirection.World;
    }
    
    public void switchCamera()
    {
        Debug.Log("Switching camera, currently facing: " + cameraManager.currentFacingDirection);
        if (cameraManager.currentFacingDirection == CameraFacingDirection.World)
        {
            
            cameraManager.requestedFacingDirection = CameraFacingDirection.User;
        } else
        {
            cameraManager.requestedFacingDirection = CameraFacingDirection.World;
        }
        
    }

    
}
