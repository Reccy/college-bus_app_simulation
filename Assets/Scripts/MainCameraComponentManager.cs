using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (OrbitCam), typeof (FlyCam))]
public class MainCameraComponentManager : MonoBehaviour
{
    private enum CameraMode
    {
        FlyCam,
        OrbitCam
    }

    private OrbitCam orbitCam;
    private FlyCam flyCam;
    private CameraMode cameraMode;

    private void Awake()
    {
        orbitCam = GetComponent<OrbitCam>();
        flyCam = GetComponent<FlyCam>();

        cameraMode = CameraMode.FlyCam;
        SetActiveCamera(cameraMode);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            IncrementCameraMode();
            SetActiveCamera(cameraMode);
        }
    }

    private void IncrementCameraMode()
    {
        int enumSize = System.Enum.GetValues(typeof(CameraMode)).GetLength(0);

        cameraMode++;

        if ((int)cameraMode >= enumSize)
            cameraMode = 0;
    }

    private void SetActiveCamera(CameraMode mode)
    {
        switch (cameraMode)
        {
            case CameraMode.FlyCam:
                orbitCam.enabled = false;
                flyCam.enabled = true;
                break;
            case CameraMode.OrbitCam:
                orbitCam.enabled = true;
                flyCam.enabled = false;
                break;
        }
    }
}
