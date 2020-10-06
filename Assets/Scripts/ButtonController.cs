using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pincushion.LD47
{
    public class ButtonController : MonoBehaviour
    {
        public GearController gear;
        public GameObject lever; //.. or wheel
        public ButtonType type;

        private bool positionLeft = true;

        public void Highlight(bool highlight)
        {
            gear.Highlight(highlight);
        }

        public void PressButton()
        {
            if (type == ButtonType.Rotation)
            {
                StartCoroutine(MoveWheel());
                gear.gearSystemController.turn(gear, 1, true);
            }
            else if (type == ButtonType.Position) {
                StartCoroutine(MoveLever());
                gear.TogglePosition();
            }
        }

        IEnumerator MoveWheel()
        {
            // the gear is moving, disconnect it

            float moveSpeed = 1f; // seconds per unit distance
            float progress = 0;

            Vector3 initialRotation = lever.transform.localRotation.eulerAngles;
            Vector3 desiredRotation = initialRotation + new Vector3(0,0,360);

            while (progress < 1)
            {
                yield return null;

                float frameProgress = Time.deltaTime / moveSpeed;
                progress += frameProgress;

                Vector3 currentRotation = Vector3.Lerp(initialRotation, desiredRotation, progress);
                lever.transform.localRotation = Quaternion.Euler(currentRotation);
            }
        }

        IEnumerator MoveLever()
        {
            // the gear is moving, disconnect it

            float moveSpeed = 1f; // seconds per unit distance
            float progress = 0;

            Quaternion initialRotation = lever.transform.localRotation;
            Quaternion desiredRotation = (positionLeft) ? Quaternion.Euler(-110, 0, 0) : Quaternion.Euler(-70, 0, 0);

            while (progress < 1)
            {
                yield return null;

                float frameProgress = Time.deltaTime / moveSpeed;
                progress += frameProgress;

                lever.transform.localRotation = Quaternion.Slerp(initialRotation, desiredRotation, progress);
            }

            positionLeft = !positionLeft;

            yield return null;
        }
    }
}