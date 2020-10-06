using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pincushion.LD47
{
    public class SceneController : MonoBehaviour
    {
        public GameObject floor0;
        public GameObject floor0_onlyforround;
        public GameObject floor1;
        public GameObject floor1_onlyforround;
        public GameObject floor2;
        public GameObject floor2_onlyforround;

        public RoomController floor1_startroom;
        public Camera minimapCamera;
        public CameraController mainCamera;
        public PlayerController player;

        public GameObject dialogCanvas;
        public GameObject dialogContainer;

        public SoundController sound;

        public bool endLevelOk = false;

        private int currentFloor = -1;


        public void Start()
        {
            NextFloor();
        }

        public void NextFloor()
        {
            switch (currentFloor)
            {
                case -1:
                    minimapCamera.orthographicSize = 10f;
                    floor0.SetActive(true);

                    minimapCamera.transform.position = new Vector3(10f, 10, 5f);

                    currentFloor = 0;

                    OpenDialog("Intro");
                    break;

                case 0:
                    minimapCamera.orthographicSize = 10f;
                    floor0_onlyforround.SetActive(false);
                    floor1.SetActive(true);

                    player.transform.parent = floor1_startroom.playerContainer.transform;
                    player.transform.localPosition = Vector3.zero;
                    floor1_startroom.MoveToRoom();

                    minimapCamera.transform.position = new Vector3(5f, 10, 5f);

                    currentFloor = 1;

                    OpenDialog("StartLevel");
                    break;

                case 1:
                    floor1_onlyforround.SetActive(false);
                    floor2.SetActive(true);

                    minimapCamera.orthographicSize = 15f;
                    minimapCamera.transform.position = new Vector3(10f, 10, 10f);

                    currentFloor = 2;

                    OpenDialog("StartLevel");
                    break;
                case 2:
                    OpenDialog("Win");
                    break;
            }
        }

        IEnumerator ChangeFloors()
        {


            yield return null;
        }



        public void OpenDialog(string name)
        {
            for (int i = 0; i < dialogContainer.transform.childCount; i++)
            {
                if (dialogContainer.transform.GetChild(i).gameObject.name == name)
                {
                    dialogContainer.transform.GetChild(i).gameObject.SetActive(true);
                    dialogCanvas.SetActive(true);
                    return;
                }
            }
        }
        public bool IsDialogOpen(string name)
        {
            for (int i = 0; i < dialogContainer.transform.childCount; i++)
            {
                if (dialogContainer.transform.GetChild(i).gameObject.name == name && dialogContainer.transform.GetChild(i).gameObject.activeSelf)
                {
                    return true;
                }
            }
            return false;
        }

        public void CloseDialog()
        {
            // hide all dialogs
            for (int i = 0; i < dialogContainer.transform.childCount; i++)
            {
                if (dialogContainer.transform.GetChild(i).gameObject.activeSelf)
                {
                    dialogContainer.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            dialogCanvas.SetActive(false);
        }

        public void CloseEndLevelDialog()
        {
            CloseDialog();
            NextFloor();
        }
    }
}