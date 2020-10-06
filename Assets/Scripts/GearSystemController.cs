using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pincushion.LD47
{
    public class GearSystemController : MonoBehaviour
    {
        private GearController[] gears;
        public SceneController scene;

        private void Awake()
        {
            gears = GetComponentsInChildren<GearController>();
            foreach (GearController gear in gears)
            {
                gear.gearSystemController = this;
            }
        }

        public void turn(GearController gear, int teeth, bool directionRight)
        {
            if (gear.turning)
            {
                //TODO maybe need to give some feedback
                return;
            }
            HashSet<GearController> turns = new HashSet<GearController>();
            GetAllGearTurns(gear, teeth, directionRight, ref turns);

            StartCoroutine(TurnCoroutine(turns, teeth));
        }

        private void GetAllGearTurns(GearController driver, int teeth, bool directionRight, ref HashSet<GearController> turns)
        {
            int direction = directionRight ? 1 : -1;
            float rotationPerTooth = 1 / (float)driver.teeth * 360 * direction;
            float totalDriverRotation = rotationPerTooth * teeth;

            driver.turning = true;
            driver.initialRotation = driver.transform.localRotation.eulerAngles;
            driver.desiredRotation = driver.initialRotation + driver.GetRotationVector(totalDriverRotation);

            turns.Add(driver);

            foreach (GearController connectedGear in driver.connectedGears)
            {
                GearController gear = connectedGear;
                if (!turns.Contains(gear) && gear.isActiveAndEnabled)
                {
                    bool connectedSpinDirection = !directionRight;// != connectedGear.connectedRight;
                    GetAllGearTurns(gear, teeth, connectedSpinDirection, ref turns);
                }
            }
        }

        IEnumerator TurnCoroutine(HashSet<GearController> turns, int teeth)
        {
            float rotateSpeed = 1f; // seconds per driver tooth
            float progress = 0;

            scene.sound.playSound("rotate");

            while (progress < 1) {
                yield return null;

                float frameProgress = Time.deltaTime / rotateSpeed / (float)teeth;
                progress += frameProgress;

                foreach (GearController gear in turns)
                {
                    Vector3 currentRotation = Vector3.Lerp(gear.initialRotation, gear.desiredRotation, progress);
                    gear.transform.localRotation = Quaternion.Euler(currentRotation);
                }
            }

            foreach (GearController gear in turns)
            {
                gear.turning = false;
            }

            scene.sound.stopSound("rotate");

            yield return null;
        }
    }
}