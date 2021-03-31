using Character.UI;
using InputMonoBehaviorParent;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Weapons;

namespace Character
{
    public class WeaponHolder : InputMonoBehavior
    {
        [Header("weapon to spawn"), SerializeField]
        private GameObject WeapontoSpawn;



        [SerializeField]
        private Transform WeaponSocketLocation;
        
        private WeaponComponent EquippedWeapon;

        private Transform GripIKLocation;
        private bool WasFiring = false;
        private bool FiringPressed = false;


        // player component references
        public PlayerController Controller => PController;
        private PlayerController PController;
        private CrosshairScript PCrosshair;
        private Animator PAnimator;
        private Camera viewCam;
        public GameObject AmmoIndicator;


        // animation hash
        private static readonly int AimHorizontalHash = Animator.StringToHash("AimHorizontal");
        private static readonly int AimVerticalHash = Animator.StringToHash("AimVertical");
        private static readonly int FiringHash = Animator.StringToHash("IsFiring");
        private static readonly int ReloadingHash = Animator.StringToHash("IsReloading");
        private static readonly int WeaponTypeHash = Animator.StringToHash("WeaponType");




        private new void Awake()
        {  
            base.Awake();

            // set references
            PController = GetComponent<PlayerController>();
            PAnimator = GetComponent<Animator>();

            if (PController)
            {
                PCrosshair = PController.chs;
            }

            // set camera ref
            viewCam = Camera.main;




        }


        void Start()
        {
            // spawn weapon and attach to player's weapon socket 
            GameObject spawnedweapon = Instantiate(
                WeapontoSpawn, 
                WeaponSocketLocation.position, 
                WeaponSocketLocation.rotation,
                WeaponSocketLocation
            );
            if (!spawnedweapon)return;

            // set equipped weapon to spwaned weapon
            EquippedWeapon = spawnedweapon.GetComponent<WeaponComponent>();
            if (!EquippedWeapon) return;
            print(EquippedWeapon);

            // initialize weapon
            EquippedWeapon.Initialize(this, PCrosshair);
            

            PlayerEvent.Invoke_OnWeaponEquipped(EquippedWeapon);

            // set grip inverse kinematic location
            GripIKLocation = EquippedWeapon.GripLocation;

            // set weapon type for animator
            PAnimator.SetInteger(WeaponTypeHash, (int)EquippedWeapon.WeaponInformation.WeaponType);

            // UpdateAmmoIndicator();

            // if (spawnedweapon)
            // {
            //     // set spawned weapon's parent to player's weapon socket
            //     spawnedweapon.transform.parent = WeaponSocketLocation;
            //     print("Weaponspawned");
            // }
        }




        private void Update() 
        {
            // print(EquippedWeapon.WeaponInformation.BulletsAvailable);
        }

        // public void UpdateAmmoIndicator()
        // {
        //     AmmoIndicator.transform.Find("BulletInClip").GetComponent<Text>().text = EquippedWeapon.WeaponInformation.BulletsInClip.ToString();
        //     AmmoIndicator.transform.Find("BulletInPouch").GetComponent<Text>().text = EquippedWeapon.WeaponInformation.BulletsAvailable.ToString();

        // }






        private void OnAnimatorIK(int layerIndex)
        {
            PAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            PAnimator.SetIKPosition(AvatarIKGoal.LeftHand, GripIKLocation.position);
        }
        


        // firing function
        private void OnFire2(InputAction.CallbackContext pressed)
        {
            // print(pressed);

            FiringPressed = pressed.ReadValue<float>() == 1f ? true : false;
            
            if (FiringPressed)
            {
                StartFiring();
            }
            else
            {
                StopFiring();
            }

            // if (!PController.IsReloading)
            // {
            //     UpdateAmmoIndicator();
            // }
        }

        private void StartFiring()
        {
            
            // if nobullet, stop
            if (EquippedWeapon.WeaponInformation.BulletsAvailable <= 0 
            && EquippedWeapon.WeaponInformation.BulletsInClip <= 0) return;

            // set player controller status
            PController.IsFiring = true;

            // play firing animation
            PAnimator.SetBool(FiringHash, true);

            // call start firing on equipped weapon
            EquippedWeapon.StartFiringWeapon();

        }

        private void StopFiring()
        {
            PController.IsFiring = false;
            PAnimator.SetBool(FiringHash, false);
            EquippedWeapon.StopFiringWeapon();
        }


        
        private void OnReload(InputValue button)
        {
            if(EquippedWeapon.WeaponInformation.BulletsAvailable >= 0)
            {
                StartReloading();

            }
        }


        public void StartReloading()
        {
            if (EquippedWeapon.WeaponInformation.BulletsAvailable <= 0 && PController.IsFiring)
            {
                StopFiring();
                return;
            }

            PController.IsReloading = true;
            PAnimator.SetBool(ReloadingHash, true);
            EquippedWeapon.StartReloading();
            
            InvokeRepeating(nameof(StopReloading), 0, .1f);
        }
        
        private void StopReloading()
        {
            
            // if is reloading, dont stop
            if (PAnimator.GetBool(ReloadingHash)) return;
            
            // UpdateAmmoIndicator();
            
            PController.IsReloading = false;
            EquippedWeapon.StopReloading();
            CancelInvoke(nameof(StopReloading));
            

            if (!WasFiring || !FiringPressed) return;
            
            StartFiring();
            WasFiring = false;
            
        }









        private void OnLook(InputAction.CallbackContext obj)
        {
            // print(obj.action);
            Vector3 independentMousePosition = viewCam.ScreenToViewportPoint(PCrosshair.CurrentAimPosition);
            

            PAnimator.SetFloat(AimHorizontalHash, independentMousePosition.x);
            PAnimator.SetFloat(AimVerticalHash, independentMousePosition.y);
        }

        private new void OnEnable()
        {
            base.OnEnable();
            GameInput.PlayerActionMap.Look.performed += OnLook;
            GameInput.PlayerActionMap.Fire.performed += OnFire2;
        }

        private new void OnDisable()
        {
            base.OnDisable();
            GameInput.PlayerActionMap.Look.performed -= OnLook;
            GameInput.PlayerActionMap.Fire.performed -= OnFire2;
        
        }



    }    
}
