using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;


namespace Character
{
    public class TPS_Movement : MonoBehaviour
    {


        public float WalkSpeed;
        public float RunSpeed;
        public float JumpForce;
        
        
        float CurrentSpeed;
        [SerializeField] private LayerMask JumpLayerMask;

        [SerializeField] private float JumpThreshold = 0.1f;
        [SerializeField] private float JumpLandingDelay = 0.0f;

        PlayerController PController;
        Animator PlayerAnimator;
        Rigidbody rbRef;

        Vector2 InputVector;
        Vector3 MoveDirection;

        private NavMeshAgent PlayerNavMeshAgent;


        // animator hashes
        public readonly int MovementXHash = Animator.StringToHash("MovementX");
        public readonly int MovementYHash = Animator.StringToHash("MovementY");
        public readonly int RunHash = Animator.StringToHash("Running");
        public readonly int JumpHash = Animator.StringToHash("Jumping");





        void Start()
        {
            // set ref
            PController = GetComponent<PlayerController>();
            PlayerAnimator = GetComponent<Animator>();
            rbRef = GetComponent<Rigidbody>();
            PlayerNavMeshAgent = GetComponent<NavMeshAgent>();

            // WalkSpeed = 3.0f;
            // RunSpeed = 5.0f;
            // JumpForce = 1.0f;

            PController.IsJumping = false;
            PController.IsRunning = false;
        }


        void Update()
        {
            // if is jumping dont move
            if(PController.IsJumping) return;

            // if no input dont move
            if(InputVector.magnitude <= 0) return;

            // if no input set move direction to zero
            if (!(InputVector.magnitude > 0)) MoveDirection = Vector3.zero;


            // determine walking or running 
            if (PController.IsRunning)
            {
                CurrentSpeed = RunSpeed;
            }
            else
            {
                CurrentSpeed = WalkSpeed;
            }



            // determine player direction
            MoveDirection = transform.forward * InputVector.y + transform.right * InputVector.x;

            // make movement vector
            Vector3 movement = MoveDirection * (CurrentSpeed * Time.deltaTime);

            // apply movement
            // transform.position += movement;

            // apply movement with navmesh
            PlayerNavMeshAgent.Move(movement);



        }




        // wasd input
        public void OnMovement(InputValue input)
        {

            // get input vector from input value
            InputVector = input.Get<Vector2>();




            // set animation for animator
            PlayerAnimator.SetFloat(MovementXHash, InputVector.x);
            PlayerAnimator.SetFloat(MovementYHash, InputVector.y);


        }





        // sprint input
        public void OnRun(InputValue input)
        {
            if (input.Get().ToString()=="1")
            {
                PController.IsRunning = true;
                PlayerAnimator.SetBool(RunHash, PController.IsRunning);
            }
            else
            {
                PController.IsRunning = false;
                PlayerAnimator.SetBool(RunHash, PController.IsRunning);
            }
        }








        private void OnCollisionEnter(Collision other)
        {

            // do nothing if not jumping
            if (!other.gameObject.CompareTag("Ground") && !PController.IsJumping) return;

            // stop jumping if jumping 
            PController.IsJumping = false;
            PlayerAnimator.SetBool(JumpHash, false);



        }




        public void OnJump(InputValue input)
        {
            if (PController.IsJumping) return;

            print(input.Get());

            PlayerNavMeshAgent.isStopped = true;
            PlayerNavMeshAgent.enabled = false;

            PController.IsJumping = input.isPressed;
            PlayerAnimator.SetBool(JumpHash, input.isPressed);

            // add force to rigidbody
            rbRef.AddForce((transform.up + MoveDirection) * JumpForce, ForceMode.Impulse);

            // Check if landed
            InvokeRepeating(nameof(LandingCheck), JumpLandingDelay, 0.1f);
        }




        private void LandingCheck()
        {
            print("Checked");
            if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 100f, JumpLayerMask))
            {
                if (!(hit.distance < JumpThreshold) || !PController.IsJumping) return;
                print("stopped");

                PlayerNavMeshAgent.enabled = true;
                PlayerNavMeshAgent.isStopped = false;

                PController.IsJumping = false;
                PlayerAnimator.SetBool(JumpHash, false);

                CancelInvoke(nameof(LandingCheck));
            

            }
        }


    }
}
