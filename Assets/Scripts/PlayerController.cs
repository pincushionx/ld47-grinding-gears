using UnityEngine;
using System.Collections;

namespace Pincushion.LD47 {
    public class PlayerController : MonoBehaviour
    {
        public SceneController scene;

        public bool running = false;
        public bool sneaking = false;

        public GameObject raycastSource;

        float walkSpeed = 3;
        float crouchSpeed = 1.5f;
        float runSpeed = 6;

        float gravity = -12;
        float jumpHeight = 1f;
        [Range(0, 1)]
        float airControlPercent;

        float turnSmoothTime = 0.2f;
        float turnSmoothVelocity;

        float speedSmoothTime = 0.1f;
        float speedSmoothVelocity;
        float currentSpeed;
        float velocityY;

        Animator animator;
        Transform cameraT;
        CharacterController controller;






        ButtonController targetedButton = null;
        RoomController currentRoom = null;
        RopeController targetedRope = null;

        void Start()
        {
            animator = GetComponent<Animator>();
            cameraT = Camera.main.transform;
            controller = GetComponent<CharacterController>();

            StartCoroutine(RaycastCoroutine());
        }


        private void Update()
        {
            UpdateMovement();

            if (targetedButton != null)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    targetedButton.PressButton();
                    scene.CloseDialog();
                }
            }
            /*else if (targetedRope != null)
            {
                if (scene.endLevelOk)
                {
                    targetedRope = null;
                    scene.endLevelOk = false;

                    scene.CloseDialog();
                    scene.NextFloor();
                }
            }*/
        }

        void UpdateMovement()
        {
            // input
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector2 inputDir = input.normalized;
            running = Input.GetKey(KeyCode.LeftShift);
            sneaking = Input.GetKey(KeyCode.LeftControl);


            float desiredSpeed = 0f;

            if (running)
            {
                scene.sound.playSound("running");
                desiredSpeed = runSpeed;
                controller.height = 1.5f;
                controller.center = new Vector3(0, 0.76f, 0);
                animator.Play("Run");

            }
            else if (sneaking)
            {
                scene.sound.playSound("walking");
                desiredSpeed = crouchSpeed;
                controller.height = .75f;
                controller.center = new Vector3(0, 0.376f, 0);
                animator.Play("Sneak");
            }
            else if (inputDir.magnitude > 0f)
            {
                scene.sound.playSound("walking");
                desiredSpeed = walkSpeed;
                controller.height = 1.5f;
                controller.center = new Vector3(0, 0.76f, 0);
                animator.Play("Walk");
            }
            else
            {
                desiredSpeed = 0;
                controller.height = 1.5f;
                controller.center = new Vector3(0, 0.76f, 0);
                animator.Play("Idle");
            }

            Move(inputDir, desiredSpeed);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }

        public void SetPosition(Vector3 position)
        {
            controller.Move(position - transform.position);
        }

        void Move(Vector2 inputDir, float desiredSpeed)
        {
            if (inputDir != Vector2.zero)
            {
                float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
            }

            float targetSpeed = desiredSpeed * inputDir.magnitude;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

            velocityY += Time.deltaTime * gravity;
            Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;

            controller.Move(velocity * Time.deltaTime);
            currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

            if (controller.isGrounded)
            {
                velocityY = 0;
            }

        }

        void Jump()
        {
            if (controller.isGrounded)
            {
                float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
                velocityY = jumpVelocity;
            }
        }

        float GetModifiedSmoothTime(float smoothTime)
        {
            if (controller.isGrounded)
            {
                return smoothTime;
            }

            if (airControlPercent == 0)
            {
                return float.MaxValue;
            }
            return smoothTime / airControlPercent;
        }










        IEnumerator RaycastCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.25f);

                int layerMask = LayerMask.GetMask("Interactable");
                RaycastHit hit;
                if (Physics.Raycast(raycastSource.transform.position, transform.forward, out hit, 2f, layerMask))
                {
                    Debug.Log("hit:" + hit.collider.gameObject.name);
                    targetedButton = hit.collider.gameObject.GetComponentInParent<ButtonController>();

                    if (targetedButton != null)
                    {
                        targetedButton.Highlight(true);
                        scene.OpenDialog((targetedButton.type == ButtonType.Rotation) ? "UseGearWheel" : "UseGearLever");
                    }
                    else
                    {
                        targetedRope = hit.collider.gameObject.GetComponentInParent<RopeController>();

                        if (targetedRope != null)
                        {
                            scene.OpenDialog("EndLevel");
                        }
                    }

                }
                else if(targetedButton != null)
                {
                    // TODO maybe coroutine the untargetting
                    targetedButton.Highlight(false);
                    targetedButton = null;

                    scene.CloseDialog();
                }
                else if (targetedRope != null)
                {
                    targetedRope = null;
                }


                layerMask = LayerMask.GetMask("Room");
                if (Physics.Raycast(raycastSource.transform.position, -transform.up, out hit, 2f, layerMask))
                {
                    Debug.Log("hit:" + hit.collider.gameObject.name);
                    RoomController room = hit.collider.gameObject.GetComponentInParent<RoomController>();
                    if (room != currentRoom)
                    {
                        currentRoom = room;
                        currentRoom.MoveToRoom();
                    }
                }
                else
                {
                    // currently happens when between rooms
                    currentRoom = null;
                }
            }
        }
    }
}