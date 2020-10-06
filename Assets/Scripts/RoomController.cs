using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pincushion.LD47
{
    public class RoomController : MonoBehaviour
    {
        public GameObject cameraContainer;
        public GameObject playerContainer;

        public SceneController scene;

        public new CameraController camera;
        public GameObject player;

        private void Awake()
        {
            camera = scene.mainCamera;
            player = scene.player.gameObject;
        }

        public void MoveToRoom()
        {
            camera.MoveCamera(cameraContainer);

            player.transform.parent = playerContainer.transform;
        }

    }
}