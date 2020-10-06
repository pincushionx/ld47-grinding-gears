using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pincushion.LD47
{
    public class CameraController : MonoBehaviour
    {
        public void MoveCamera(GameObject cameraContainer)
        {
            transform.parent = cameraContainer.transform;
            transform.localScale = Vector3.one;

            StartCoroutine(MoveCameraCoroutine());
        }

        IEnumerator MoveCameraCoroutine()
        {
            float moveSpeed = .05f; // seconds per unit distance
            float progress = 0;

            Vector3 initialPosition = transform.localPosition;
            Quaternion initialRotation = transform.localRotation;

            Vector3 desiredPosition = Vector3.zero;
            Quaternion desiredRotation = Quaternion.identity;

            float distance = (desiredPosition - initialPosition).magnitude;

            while (progress < 1)
            {
                yield return null;

                float frameProgress = Time.deltaTime / moveSpeed / distance;
                progress += frameProgress;

                transform.localPosition = Vector3.Slerp(initialPosition, desiredPosition, progress);
                transform.localRotation = Quaternion.Slerp(initialRotation, desiredRotation, progress);
            }
        }
    }
}