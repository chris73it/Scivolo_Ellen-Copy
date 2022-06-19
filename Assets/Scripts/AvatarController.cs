using UnityEngine;
using MenteBacata.ScivoloCharacterController;
using HeroicArcade.CC.Demo;

namespace HeroicArcade.CC
{
    [RequireComponent(typeof(Animator))]
    public class AvatarController : MonoBehaviour
    {
        public Character Character { get; private set; }

        public enum FSMState
        {
            Bootstrap = 0,
            Ambulation = 1, //Combines Idle, Walk and Run animations using a blend state.
            Jump = 2,
            Sprinting = 3, //Combines Idle, Walk and Run animations using a blend state.
            //Shooting = 4, //Combines Idle, Walk and Run animations using a blend state.
        }

        public enum VCamState
        {
            Center = 0,
            Ambulation = 1, //Combines Idle, Walk and Run animations using a blend state.
            Jump = 2,
            Sprinting = 3, //Combines Idle, Walk and Run animations using a blend state.
            //Shooting = 4, //Combines Idle, Walk and Run animations using a blend state.
        }

        FSMState fsmState = FSMState.Bootstrap;
        public void SpawnAnimationIsOver()
        {
            Debug.Log("Ellen spawning animation is over");
            fsmState = FSMState.Ambulation;
        }

        const float minVerticalSpeed = -12f;
        const float timeBeforeUngrounded = 0.02f;

        float deltaTime;

        float nextUngroundedTime = -1f;

        private void Awake()
        {
            Character = GetComponent<Character>();
        }

        Transform cameraTransform;
        public Transform CameraTransform { get => cameraTransform; }
        private void Start()
        {
            cameraTransform = Camera.main.transform;
            Character.Mover.canClimbSteepSlope = true;
        }

        MovingPlatform movingPlatform;

        Vector3 movementInput;
        bool groundDetected;
        bool isOnMovingPlatform = false;
        Target target2;
        private void Update()
        {
            Character.CamStyle = Character.InputController.IsAimingPressed ?
                Character.CameraStyle.Combat : Character.CameraStyle.Adventure;

            Character.ZoomedFreeLookCamera.Zoom(Character.InputController.IsAimingPressed);

            if (Character.InputController.IsAimSwitchingPressed)
            {
                Character.ZoomedFreeLookCamera.ZoomedInLateralAimOffset *= -1;
                Character.CinemachineLateralAim.offset *= -1;
                Character.InputController.IsAimSwitchingPressed = false;
            }

            if (fsmState == FSMState.Bootstrap)
            {
                return;
            }

            //This probably needs to be resolved to indicate what weapon has been selected...
            //Character.Animator.SetBool("WeaponSelected", Character.InputController.WeaponSelected != 0);

            //First things first: sample delta time once for this coming frame.
            deltaTime = Time.deltaTime;

            movementInput = GetMovementInput();

            Character.velocityXZ += Character.MoveAcceleration * deltaTime;
            if (Character.velocityXZ > Character.CurrentMaxMoveSpeed)
                Character.velocityXZ = Character.CurrentMaxMoveSpeed;

            Character.velocity = Character.velocityXZ * movementInput;
            //Debug.Log($"velocity {velocity}");

            groundDetected = DetectGroundAndCheckIfGrounded(out bool isGrounded, out GroundInfo groundInfo);

            SetGroundedIndicatorColor(isGrounded);

            isOnMovingPlatform = false;

            if (isGrounded && Character.InputController.IsJumpPressed)
            {
                Character.verticalSpeed = Character.JumpSpeed;
                nextUngroundedTime = -1f;
                isGrounded = false;
            }

            if (isGrounded)
            {
                Character.Mover.isInWalkMode = true;
                Character.verticalSpeed = 0f;

                if (groundDetected)
                {
                    isOnMovingPlatform = groundInfo.collider.TryGetComponent(out movingPlatform);
                }
            }
            else
            {
                Character.Mover.isInWalkMode = false;

                BounceDownIfTouchedCeiling();

                Character.verticalSpeed += Character.Gravity * deltaTime;

                //Limit the maximum downward speed
                if (Character.verticalSpeed < minVerticalSpeed)
                    Character.verticalSpeed = minVerticalSpeed;

                Character.velocity += Character.verticalSpeed * transform.up;
            }

            //Perform the right movement and play the corresponding animation, i.e. detect
            //      when to use the jumping/falling, idling/walking/running animations.
            Character.Animator.SetBool("IsJumpPressed", !isGrounded);
            fsmState = (isGrounded ? FSMState.Ambulation : FSMState.Jump);
            if (isGrounded)
            {
                //Character.Animator.SetBool("IsShootPressed", Character.InputController.IsShootPressed);
                if (Character.InputController.IsAimingPressed)
                {
                    Character.Animator.SetBool("IsAimingPressed", Character.InputController.IsAimingPressed);
                    target2 = Character.AutoAiming.StartAiming();

                    Character.Animator.SetBool("IsShootPressed", target2 != null && Character.InputController.IsShootPressed);
                    if (target2 != null && Character.InputController.IsShootPressed)
                    {
                        Character.AutoAiming.StartFiring(target2);
                    }
                }
                else
                {
                    Character.Animator.SetBool("IsAimingPressed", Character.InputController.IsAimingPressed);
                    Character.Animator.SetBool("IsShootPressed", Character.InputController.IsShootPressed);
                    Character.AutoAiming.StopAiming();
                }

                if (movementInput.sqrMagnitude < 1E-06f)
                {
                    Character.velocityXZ = 0f;
                    Character.Animator.SetBool("IsSprintPressed", false);
                }

                Character.Animator.SetFloat("MoveSpeed",
                    new Vector3(Character.velocity.x, 0, Character.velocity.z).magnitude / Character.CurrentMaxMoveSpeed);
                
                if (Character.velocityXZ >= 1E-06f)
                {
                    Character.Animator.SetBool("IsSprintPressed", Character.InputController.IsSprintPressed);
                }
                fsmState = (Character.InputController.IsSprintPressed ? FSMState.Sprinting : FSMState.Ambulation);

                Character.CurrentMaxMoveSpeed =
                    Character.InputController.IsSprintPressed ? Character.CurrentMaxSprintSpeed : Character.CurrentMaxWalkSpeed;
            }

            RotateTowards(Character.velocity);
            Character.Mover.Move(Character.velocity * deltaTime, Character.moveContacts, out Character.contactCount);
        }

        private void LateUpdate()
        {
            if (isOnMovingPlatform)
            {
                ApplyPlatformMovement(movingPlatform);
            }
        }

        private void RotateTowards(Vector3 direction)
        {
            switch (Character.CamStyle)
            {
                case Character.CameraStyle.Adventure:
                    //Do nothing
                    break;

                case Character.CameraStyle.Combat:
                    direction = Character.AvatarController.CameraTransform.forward;
                    break;

                case Character.CameraStyle.EricWei:
                    if (direction.sqrMagnitude == 0)
                    {
                        goto case Character.CameraStyle.Adventure;
                    }
                    goto case Character.CameraStyle.Combat;

                default:
                    Debug.LogError($"Unexpected CameraStyle {Character.CamStyle}");
                    return;
            }

            Vector3 flatDirection = Vector3.ProjectOnPlane(direction, transform.up);
            if (flatDirection.sqrMagnitude < 1E-06f)
                return;
            Quaternion targetRotation = Quaternion.LookRotation(flatDirection, transform.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Character.TurnSpeed * Time.deltaTime);
        }

        Vector3 projectedCameraForward;
        Quaternion rotationToCamera;
        private Vector3 GetMovementInput()
        {
            projectedCameraForward = Vector3.ProjectOnPlane(cameraTransform.forward, transform.up);
            rotationToCamera = Quaternion.LookRotation(projectedCameraForward, transform.up);
            return rotationToCamera * (currentMovement.x * Vector3.right + currentMovement.z * Vector3.forward);
        }

        private bool DetectGroundAndCheckIfGrounded(out bool isGrounded, out GroundInfo groundInfo)
        {
            bool groundDetected = Character.GroundDetector.DetectGround(out groundInfo);

            if (groundDetected)
            {
                if (groundInfo.isOnFloor && Character.verticalSpeed < 0.1f)
                    nextUngroundedTime = Time.time + timeBeforeUngrounded;
            }
            else
                nextUngroundedTime = -1f;

            isGrounded = Time.time < nextUngroundedTime;
            return groundDetected;
        }

        private void SetGroundedIndicatorColor(bool isGrounded)
        {
            if (Character.GroundedIndicator != null)
                Character.GroundedIndicator.material.color = isGrounded ? Color.green : Color.blue;
        }

        private void BounceDownIfTouchedCeiling()
        {
            for (int i = 0; i < Character.contactCount; i++)
            {
                if (Vector3.Dot(Character.moveContacts[i].normal, transform.up) < -0.7f)
                {
                    Character.verticalSpeed = -0.25f * Character.verticalSpeed;
                    break;
                }
            }
        }

        private void ApplyPlatformMovement(MovingPlatform movingPlatform)
        {
            GetMovementFromMovingPlatform(movingPlatform, out Vector3 movement, out float upRotation);

            transform.Translate(movement, Space.World);
            transform.Rotate(0f, upRotation, 0f, Space.Self);
        }

        private void GetMovementFromMovingPlatform(MovingPlatform movingPlatform, out Vector3 movement, out float deltaAngleUp)
        {
            movingPlatform.GetDeltaPositionAndRotation(out Vector3 platformDeltaPosition, out Quaternion platformDeltaRotation);
            Vector3 localPosition = transform.position - movingPlatform.transform.position;
            movement = platformDeltaPosition + platformDeltaRotation * localPosition - localPosition;

            platformDeltaRotation.ToAngleAxis(out float platformDeltaAngle, out Vector3 axis);
            float axisDotUp = Vector3.Dot(axis, transform.up);

            if (-0.1f < axisDotUp && axisDotUp < 0.1f)
                deltaAngleUp = 0f;
            else
                deltaAngleUp = platformDeltaAngle * Mathf.Sign(axisDotUp);
        }

        private Vector3 currentMovement;
        public void OnMoveInput(Vector2 moveInput)
        {
            //y needs to preserve its value from the previous Update.
            currentMovement.x = moveInput.x;
            currentMovement.z = moveInput.y;
        }
    }
}