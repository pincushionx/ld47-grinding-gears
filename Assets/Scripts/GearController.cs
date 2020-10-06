using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pincushion.LD47
{
    public class GearController : MonoBehaviour
    {
        public int teeth = 12;

        public GameObject gearModel;

        public RotationAxis rotationAxis = RotationAxis.X;
        public GearController[] connectedGears;
        public GearSystemController gearSystemController;

        public int currentPosition = 0;
        public PositionConnections[] positions;

        [System.Serializable]
        public class PositionConnections
        {
            public GameObject position;
            public GearController[] connectedGears;
        }

        // move variables
        private bool moving = false;
        private Vector3 initialPosition;
        private Vector3 desiredPosition;

        // turn variables
        public bool turning = false;
        public Vector3 initialRotation;
        public Vector3 desiredRotation;

        public Vector3 GetRotationVector(float rotation)
        {
            switch (rotationAxis)
            {
                case RotationAxis.X:
                    return new Vector3(rotation, 0, 0);
                case RotationAxis.Y:
                    return new Vector3(0, rotation, 0);
                case RotationAxis.Z:
                    return new Vector3(0, 0, rotation);
            }
            return Vector3.zero;
        }

        private void Awake()
        {
            if (positions.Length > 0)
            {
                SetNewConnections(positions[currentPosition].connectedGears);
            }
        }

        public void Highlight(bool highlight)
        {
            MeshRenderer renderer = gearModel.GetComponent<MeshRenderer>();

            if (highlight) {
                Material highlightMaterial = Resources.Load<Material>("Materials/Gear Highlight");
                renderer.sharedMaterial = highlightMaterial;
            }
            else
            {
                Material defaultMaterial = Resources.Load<Material>("Materials/Gear");
                renderer.sharedMaterial = defaultMaterial;
            }
        }

        public void TogglePosition()
        {
            initialPosition = positions[currentPosition].position.transform.position;

            currentPosition = ++currentPosition % positions.Length;
            desiredPosition = positions[currentPosition].position.transform.position;

            StartCoroutine(MoveGearToPosition());
        }

        IEnumerator MoveGearToPosition()
        {
            gearSystemController.scene.sound.playSound("rotate");

            // the gear is moving, disconnect it
            moving = true;
            SetNewConnections(new GearController[0]);

            float moveSpeed = 1f; // seconds per unit distance
            float progress = 0;

            float distance = (desiredPosition - initialPosition).magnitude;
            
            while (progress < 1)
            {
                yield return null;

                float frameProgress = Time.deltaTime / moveSpeed / distance;
                progress += frameProgress;

                transform.position = Vector3.Slerp(initialPosition, desiredPosition, progress);
            }

            // stop the gear, and set its new connections
            moving = false;
            SetNewConnections(positions[currentPosition].connectedGears);

            gearSystemController.scene.sound.stopSound("rotate");

            yield return null;
        }

        private void SetNewConnections(GearController[] gears) {
            // disconnect this gear from connected gears
            foreach (GearController gear in connectedGears)
            {
                gear.RemoveConnectedGear(this);
            }

            // set the new gears
            connectedGears = gears;

            // connect new gears
            foreach (GearController gear in gears)
            {
                gear.AddConnectedGear(this);
            }
        }
        private void AddConnectedGear(GearController gear)
        {
            List<GearController> list = new List<GearController>(connectedGears);
            list.Add(gear);
            connectedGears = list.ToArray();
        }
        private void RemoveConnectedGear(GearController gear) {
            List<GearController> list = new List<GearController>(connectedGears);

            if (list.Contains(gear))
            {
                list.Remove(gear);
            }

            connectedGears = list.ToArray();
        }
    }
}