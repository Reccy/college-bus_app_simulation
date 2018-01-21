using UnityEngine;

namespace AaronMeaney.BusStop.MainCamera
{
    /// <summary>
    /// Responsible for managing the Main Camera's components.
    /// </summary>
    [RequireComponent(typeof(OrbitCam), typeof(FlyCam))]
    public class MainCameraComponentManager : MonoBehaviour
    {
        /// <summary>
        /// The camera's type.
        /// </summary>
        private enum CameraMode
        {
            /// <summary>
            /// A fly cam / free cam.
            /// </summary>
            FlyCam,
            /// <summary>
            /// A camera that orbits the selected bus.
            /// </summary>
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
                CycleCameraMode();
                SetActiveCamera(cameraMode);
            }
        }

        /// <summary>
        /// Cycle sequentually through each camera mode.
        /// </summary>
        private void CycleCameraMode()
        {
            int enumSize = System.Enum.GetValues(typeof(CameraMode)).GetLength(0);

            cameraMode++;

            if ((int)cameraMode >= enumSize)
                cameraMode = 0;
        }

        /// <summary>
        /// Set the active camera mode.
        /// </summary>
        /// <param name="mode"></param>
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
}
