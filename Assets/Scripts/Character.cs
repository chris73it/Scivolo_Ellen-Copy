using UnityEngine;
using MenteBacata.ScivoloCharacterController;

namespace HeroicArcade.CC
{
    public sealed class Character : MonoBehaviour
    {
        public enum CameraStyle
        {
            Adventure,
            Combat,
            EricWei,
        }

        [Header("Character Parameters")]
        [SerializeField] CameraStyle camStyle;
        [SerializeField] float maxWalkSpeed; //6
        [SerializeField] float maxSprintSpeed; //11
        [SerializeField] float moveAcceleration; //20
        [SerializeField] float turnSpeed; //720
        [SerializeField] float jumpSpeed; //8
        [SerializeField] float runSpeed; //8
        [SerializeField] float gravity; //-25
        [SerializeField] AutoAiming autoAiming;
        [SerializeField] ZoomedFreeLookCamera zoomedFreeLookCamera;
        [SerializeField] CinemachineLateralAim cinemachineLateralAim;

        public CameraStyle CamStyle { get => camStyle; set => camStyle = value; }
        public float CurrentMaxWalkSpeed { get => maxWalkSpeed; set => maxWalkSpeed = value; }
        public float CurrentMaxSprintSpeed { get => maxSprintSpeed; set => maxSprintSpeed = value; }
        public float MoveAcceleration { get => moveAcceleration; set => moveAcceleration = value; }
        public float TurnSpeed { get => turnSpeed; }
        public float JumpSpeed { get => jumpSpeed; }
        public float RunSpeed { get => runSpeed; }
        public float Gravity { get => gravity; }
        public AutoAiming AutoAiming { get => autoAiming; }
        public ZoomedFreeLookCamera ZoomedFreeLookCamera { get => zoomedFreeLookCamera; }
        public CinemachineLateralAim CinemachineLateralAim { get => cinemachineLateralAim; }

        [SerializeField] GroundDetector groundDetector;
        [SerializeField] MeshRenderer groundedIndicator;
        [SerializeField] CharacterMover mover;
        [SerializeField] InputController inputController;
        public GroundDetector GroundDetector { get => groundDetector; }
        public MeshRenderer GroundedIndicator { get => groundedIndicator; }
        public CharacterMover Mover { get => mover; }
        public InputController InputController { get => inputController; }

        [Header("Required Components")]
        [SerializeField] AvatarController avatarController;
        public AvatarController AvatarController { get => avatarController; }
        [SerializeField] Animator animator;
        public Animator Animator { get => animator; }

        //Required by the AvatarController and by certain states in the FSM.
        [HideInInspector] public float CurrentMaxMoveSpeed;
        [HideInInspector] public Vector3 velocity = Vector3.zero;
        [HideInInspector] public float velocityXZ = 0f;
        [HideInInspector] public float verticalSpeed = 0f;
        [HideInInspector] public MoveContact[] moveContacts = CharacterMover.NewMoveContactArray;
        [HideInInspector] public int contactCount;
    }
}