 using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.Audio;
using TMPro;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviourPun, IPunObservable
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        public GameObject mainCam;
        public GameObject playerFollowCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private float skillTime ;
        private float skillcoolTime ;
        private float skill지속시간;
        private float skill쿨타임;
        //public GameObject skillTimeObject;
        public Image skillImage;
        private bool isAttackingSkill = false;
        public Renderer[] renderers;

        private int 쥐목숨 = 2;
        private bool 쥐맞음 = false;
        private float 쥐무적time = 2f;
        public bool live = true;
        private bool 순간이동live = true;

        public TextMesh nickName;
        InGameNetworkManager inGameNetworkManager;

        bool 치즈flag = false;
        Chees temp;
        bool 문flag = false;
        DoorOpen DoorOpenscript;
        bool boxflag = false;
        GiftBox giftBoxscript;

        private GameObject 살리기TEXT;
        private Slider 발전기;
        private GameObject 발전기Obejct;

        [Header("cat")]
        private int 쥐덫개수 = 3;
        private GameObject 쥐덫;
        private Text 쥐덫개수text;
        public GameObject 쥐덫생성position;
        private bool lightflag=false;
        public GameObject lightPanel;

        GameObject falshImage;
        public GameObject flash;
        public GameObject flashlight;

        [Header("Sound")]
        public AudioSource AttackSound;
        public AudioSource FlashSound;
        
        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }

        public bool isAttacking = false;

        private void OnCollisionEnter(Collision collision)
        {

        }
        private void OnTriggerEnter(Collider other)
        {
            ThirdPersonController parentScript = GetComponent<ThirdPersonController>();

            if(photonView.IsMine && gameObject.CompareTag("cat")&& other.gameObject.CompareTag("light")&& !lightflag)
            {
                StartCoroutine(빛맞음());
            }

            if (photonView.IsMine && gameObject.CompareTag("mouse"))
            {
                Debug.Log("충돌 발생: " + other.name);
                if (other.gameObject.CompareTag("mouseTrap"))//만약 cat이 공격하고 있다면
                {
                    photonView.RPC("공격받음RPC", RpcTarget.All);
                }
            }

            /*if (photonView.IsMine && gameObject.CompareTag("mouse") && other.gameObject.CompareTag("mouse"))
            {
                Debug.Log("충돌 발생: " + other.name);
                ThirdPersonController temp = other.GetComponent<ThirdPersonController>();
                if (temp != null && temp != parentScript)
                {
                    print("널아님?");
                    print(temp.live);

                    if (temp.live == false)
                    {
                        print("살리긴함");
                        temp.살림();
                    }
                }

            }*/
            if (photonView.IsMine && gameObject.CompareTag("mouse") && other.gameObject.CompareTag("cheese"))
            {
                치즈flag = true;
                temp = other.gameObject.GetComponent<Chees>();
                //temp.게이지증가();
            }

            if (photonView.IsMine && gameObject.CompareTag("mouse") && other.gameObject.CompareTag("box"))
            {
                boxflag = true;
                giftBoxscript = other.gameObject.GetComponent<GiftBox>();
                //temp.게이지증가();
            }

            if (photonView.IsMine && gameObject.CompareTag("mouse") && other.gameObject.CompareTag("door"))
            {
                문flag = true;
                DoorOpenscript = other.gameObject.GetComponent<DoorOpen>();
                //temp.게이지증가();
            }

            if (photonView.IsMine && gameObject.CompareTag("mouse") && other.gameObject.CompareTag("Escape")&&쥐목숨==0)
            {
                쥐목숨 = 2;
                if (photonView.IsMine)//이 부분을 감옥 로직으로 바꿔야하는데 감옥 문을 열면 문에 콜라이더를 하나 생성해서 그거에 들어가면 목숨을 2로 바꾸고 UPDATE한다?? 만약 목숨이 0이면 충돌이 가능하게
                {
                    inGameNetworkManager.쥐목숨Update(PhotonNetwork.LocalPlayer.NickName, 쥐목숨);//업데이트 하면서 감옥에 있는 사람의 수를 조정
                }//
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (photonView.IsMine && gameObject.CompareTag("mouse") && other.gameObject.CompareTag("Prison")&& 쥐목숨 == 0)
            {
                inGameNetworkManager.탈출문Open();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (photonView.IsMine && gameObject.CompareTag("mouse") && other.gameObject.CompareTag("cheese"))
            {
                치즈flag = false;
                temp = null;
            }

            if (photonView.IsMine && gameObject.CompareTag("mouse") && other.gameObject.CompareTag("box"))
            {
                boxflag = false;
                giftBoxscript = null;
            }

            if (photonView.IsMine && gameObject.CompareTag("mouse") && other.gameObject.CompareTag("door"))
            {
                문flag = false;
                DoorOpenscript = null;
            }
            if (photonView.IsMine && gameObject.CompareTag("mouse") && other.gameObject.CompareTag("Prison"))
            {
                inGameNetworkManager.탈출문Close();
            }
        }

        private void Awake()
        {
            if (PhotonNetwork.IsMasterClient)//닉네임을 아군만 표시
            {
                if (gameObject.CompareTag("mouse"))
                {
                    nickName.gameObject.SetActive(false);
                }
            }
            else
            {
                if (gameObject.CompareTag("cat"))
                {
                    nickName.gameObject.SetActive(false);
                }
            }

            if (photonView.IsMine)
            {
                // get a reference to our main camera
                if (_mainCamera == null)
                {
                    _mainCamera = mainCam;
                }

            }
            else
            {
                mainCam.SetActive(false);
                playerFollowCamera.SetActive(false);
            }
        }

        private void Start()
        {
            _hasAnimator = TryGetComponent(out _animator);
            if (photonView.IsMine)
            {
                _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

                _hasAnimator = TryGetComponent(out _animator);
                _controller = GetComponent<CharacterController>();
                _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
                _playerInput = GetComponent<PlayerInput>();
#else
			            Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

                AssignAnimationIDs();

                // reset our timeouts on start
                _jumpTimeoutDelta = JumpTimeout;
                _fallTimeoutDelta = FallTimeout;

                inGameNetworkManager = FindObjectOfType<InGameNetworkManager>();
                살리기TEXT = GameObject.Find("발전기Image");
                GameObject 발전기Obejct = GameObject.Find("게이지Slider");
                발전기 = 발전기Obejct.GetComponent<Slider>();
                발전기Obejct.SetActive(false);

                lightPanel= GameObject.Find("lightPanel");
                lightPanel.SetActive(false);
                //skillTimeObject = GameObject.Find("스킬");
                if (networkManager.MySkill == 0 || gameObject.CompareTag("mouse"))
                {
                    GameObject temp1 = GameObject.Find("헤이스트");
                    temp1.SetActive(false);
                    GameObject temp = GameObject.Find("다크사이트");
                    temp.SetActive(false);
                    falshImage = GameObject.Find("후레쉬");
                    falshImage.SetActive(false);
                    if (gameObject.CompareTag("mouse"))
                    {
                        쥐덫 = GameObject.Find("쥐덫");
                        쥐덫.SetActive(false);
                    }

                }
                if (gameObject.CompareTag("cat"))
                {
                    쥐덫 = GameObject.Find("쥐덫");
                    쥐덫개수text = 쥐덫.GetComponentInChildren<Text>();
                }
                if (networkManager.MySkill == 1 && gameObject.CompareTag("cat"))
                {
                    GameObject temp1 = GameObject.Find("헤이스트");
                    temp1.SetActive(false);
                    GameObject temp2 = GameObject.Find("후레쉬");
                    temp2.SetActive(false);
                    GameObject temp = GameObject.Find("다크사이트게이지");
                    skillImage = temp.GetComponent<Image>();
                    skill지속시간 = 10f;
                    skill쿨타임 = 30f;
                    skillTime = skill지속시간;
                    skillcoolTime = skill쿨타임;
                }
                else if (networkManager.MySkill == 2 && gameObject.CompareTag("cat"))
                {
                    GameObject temp1 = GameObject.Find("다크사이트");
                    temp1.SetActive(false);
                    GameObject temp2 = GameObject.Find("후레쉬");
                    temp2.SetActive(false);
                    GameObject temp = GameObject.Find("헤이스트게이지");
                    skillImage = temp.GetComponent<Image>();
                    skill지속시간 = 4f;
                    skill쿨타임 = 20f;
                    skillTime = skill지속시간;
                    skillcoolTime = skill쿨타임;
                }

            }
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                _hasAnimator = TryGetComponent(out _animator);
                if (순간이동live)
                {
                    JumpAndGravity();
                    GroundedCheck();
                    Move();
                }

                if (live)//!_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")
                {

                }

                if (Input.GetMouseButtonDown(0) && !isAttacking && (gameObject.CompareTag("cat")))
                {
                    photonView.RPC("AttackSoundRPC", RpcTarget.All);
                    _animator.SetTrigger("MoveToAttack");
                    isAttacking = true;
                }

                if (Input.GetMouseButtonDown(1) && gameObject.CompareTag("mouse")&& falshImage.activeSelf)
                {
                    //RPC
                    photonView.RPC("falshOn", RpcTarget.All);
                }
               /* if (Input.GetKeyDown(KeyCode.P) & gameObject.CompareTag("mouse"))
                {
                    falshImage.SetActive(false);
                    photonView.RPC("falshOff", RpcTarget.All);
                }
                if (Input.GetKeyDown(KeyCode.L) & gameObject.CompareTag("mouse"))
                {
                    falshImage.SetActive(true);
                    photonView.RPC("falshGet", RpcTarget.All);
                }
*/
                if (Input.GetMouseButtonDown(1) && !isAttackingSkill && gameObject.CompareTag("cat"))
                {
                    // _animator.SetTrigger("MoveToAttack");
                    isAttackingSkill = true;
                    if (networkManager.MySkill == 1)
                    {
                        photonView.RPC("Start은신", RpcTarget.All);

                    }
                    else if (networkManager.MySkill == 2)
                    {
                        MoveSpeed = 4;
                        SprintSpeed = 10;
                    }
                }
                if (Input.GetKeyDown(KeyCode.T) && gameObject.CompareTag("cat") && 쥐덫개수 > 0)
                {
                    Vector3 trapPosition = 쥐덫생성position.transform.position;
                    // y 값을 0으로 설정
                    trapPosition.y = 0;
                    PhotonNetwork.Instantiate("쥐덫1", trapPosition, Quaternion.identity);
                    쥐덫개수--;
                    쥐덫개수text.text = "쥐덫개수:" + 쥐덫개수.ToString();
                }
                if (isAttackingSkill && skillImage != null)//만약 스킬을 가지고있다면 
                {
                    //float time = skillTime / 4;
                    float skilltime = skillcoolTime / skill쿨타임;
                    skillImage.fillAmount = skilltime;
                    //skillImage.fillAmount = time;
                    if (skillTime > 0)
                    {
                        skillTime -= Time.deltaTime;
                        
                        if (skillTime < 0)
                        {
                            //스킬 time을 초기화
                            //isAttackingSkill = false;
                            //skillImage.fillAmount = 0;
                            if (networkManager.MySkill == 1)
                            {
                                photonView.RPC("End은신", RpcTarget.All);
                            }
                            else if (networkManager.MySkill == 2)
                            {
                                MoveSpeed = 2.0f;
                                SprintSpeed = 5.335f;
                            }
                        }
                    }
                    if(skillcoolTime>0)
                    {
                        skillcoolTime -= Time.deltaTime;
                        if (skillcoolTime < 0)
                        {
                            skillTime = skill지속시간;
                            skillcoolTime = skill쿨타임;
                            skillImage.fillAmount = 0;
                            isAttackingSkill = false;
                        }
                    }
                }
                /*if (Input.GetKeyDown(KeyCode.Q))
                {
                    공격받음();
                }*/

                if (쥐맞음)
                {
                    if (쥐무적time > 0)
                    {
                        쥐무적time -= Time.deltaTime;
                        if (쥐무적time < 0)
                        {
                            쥐무적time = 2;//스킬 time을 초기화
                            쥐맞음 = false;
                            MoveSpeed = 2.0f;
                            SprintSpeed = 5.335f;
                        }
                    }
                }

                
                if (치즈flag && temp != null && Input.GetKey(KeyCode.E))
                {
                    
                    print("됨");
                    temp.게이지증가();
                }

                if(boxflag && giftBoxscript != null && Input.GetKey(KeyCode.E))
                {
                    print("됨");
                    int num=giftBoxscript.게이지증가();
                    if(num==1)
                    {
                        FlashSound.Play();
                        falshImage.SetActive(true);
                        photonView.RPC("falshGet", RpcTarget.All);
                    }
                }

                if (문flag && DoorOpenscript != null && Input.GetKey(KeyCode.E))
                {
                    print("됨");
                    DoorOpenscript.게이지증가();
                }
            }
        }

        private void LateUpdate()
        {
            if (photonView.IsMine)
            {
                CameraRotation();
            }
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            if (live)
            {
                // set target speed based on move speed, sprint speed and if sprint is pressed
                float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

                // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

                // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
                // if there is no input, set the target speed to 0
                if (_input.move == Vector2.zero) targetSpeed = 0.0f;

                // a reference to the players current horizontal velocity
                float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

                float speedOffset = 0.1f;
                float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

                // accelerate or decelerate to target speed
                if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                    currentHorizontalSpeed > targetSpeed + speedOffset)
                {
                    // creates curved result rather than a linear one giving a more organic speed change
                    // note T in Lerp is clamped, so we don't need to clamp our speed
                    _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                        Time.deltaTime * SpeedChangeRate);

                    // round speed to 3 decimal places
                    _speed = Mathf.Round(_speed * 1000f) / 1000f;
                }
                else
                {
                    _speed = targetSpeed;
                }

                _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
                if (_animationBlend < 0.01f) _animationBlend = 0f;

                // normalise input direction
                Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

                // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
                // if there is a move input rotate player when the player is moving
                if (_input.move != Vector2.zero)
                {
                    _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                        _mainCamera.transform.eulerAngles.y;
                    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                        RotationSmoothTime);

                    // rotate to face input direction relative to camera position
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }
                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetFloat(_animIDSpeed, _animationBlend);
                    _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
                }
            }



            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            if (!live)
            {
                targetDirection = Vector3.zero;
            }

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                                new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);


        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f && live)//살아있다면 점프가능
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (photonView.IsMine)
            {
                if (animationEvent.animatorClipInfo.weight > 0.5f)
                {
                    if (FootstepAudioClips.Length > 0)
                    {
                        var index = Random.Range(0, FootstepAudioClips.Length);
                        AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                    }
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (photonView.IsMine)
            {
                if (animationEvent.animatorClipInfo.weight > 0.5f)
                {
                    AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }




        public void 살림()
        {
            photonView.RPC("OnLive", RpcTarget.All);
        }

        [PunRPC]
        public void OnLive()
        {
            print("살아남");
            live = true;
            
            _animator.SetTrigger("DieToMove");
             /*쥐목숨 = 2;
            if (photonView.IsMine)//이 부분을 감옥 로직으로 바꿔야하는데 감옥 문을 열면 문에 콜라이더를 하나 생성해서 그거에 들어가면 목숨을 2로 바꾸고 UPDATE한다?? 만약 목숨이 0이면 충돌이 가능하게
            {
                inGameNetworkManager.쥐목숨Update(PhotonNetwork.LocalPlayer.NickName, 쥐목숨);
            }*/
        }

        public void OnAttackEnd()
        {
            print("실행딤");
            isAttacking = false;
        }



        [PunRPC]
        public void Start은신()
        {
            if (photonView.IsMine)
            {
                foreach (var renderer in renderers)
                {
                    foreach (var material in renderer.materials)
                    {
                        material.SetFloat("_Surface", 1.0f); // 0은 Opaque, 1은 Transparent
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetInt("_ZWrite", 0);
                        material.DisableKeyword("_ALPHATEST_ON");
                        material.EnableKeyword("_ALPHABLEND_ON");
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                        // 알파(투명도)를 0으로 설정하여 투명하게 만듭니다.
                        Color color = material.color;
                        color.a = 0.4f;
                        material.color = color;
                    }
                }
            }
            else
            {
                foreach (var renderer in renderers)
                {
                    foreach (var material in renderer.materials)
                    {
                        material.SetFloat("_Surface", 1.0f); // 0은 Opaque, 1은 Transparent
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetInt("_ZWrite", 0);
                        material.DisableKeyword("_ALPHATEST_ON");
                        material.EnableKeyword("_ALPHABLEND_ON");
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                        // 알파(투명도)를 0으로 설정하여 투명하게 만듭니다.
                        Color color = material.color;
                        color.a = 0.0f;
                        material.color = color;
                    }
                }
            }
        }

        [PunRPC]
        void End은신()
        {
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.materials)
                {
                    material.SetFloat("_Surface", 0f); // 0은 Opaque, 1은 Transparent
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                    // 알파(투명도)를 0으로 설정하여 투명하게 만듭니다.
                    Color color = material.color;
                    color.a = 1f;
                    material.color = color;
                }
            }
        }

        public void 공격받음()
        {
            
            if (!쥐맞음)
            {
                print("여러번");
                photonView.RPC("공격받음RPC", RpcTarget.All);
                쥐맞음 = true;
            }
                
        }

        [PunRPC]
        void 공격받음RPC()//이 코드 반응속도 때문에 수정필요할듯
        {
            if (!쥐맞음)
            {
                쥐목숨--;
                if (쥐목숨 == 1)
                {
                    //이속증가
                    쥐맞음 = true;
                    MoveSpeed = 4;
                    SprintSpeed = 10;
                    if (photonView.IsMine)
                    {
                        inGameNetworkManager.쥐목숨Update(PhotonNetwork.LocalPlayer.NickName, 쥐목숨);
                    }
                }
                if (쥐목숨 == 0)//왜 따로 if문을 사용?? 흠
                {
                    쥐맞음 = true;//여기서도 해줘야함.
                    if (photonView.IsMine)
                    {
                        _animator.ResetTrigger("DieToMove");
                        _animator.SetTrigger("MoveToDie");
                    }
                    live = false;
                    print(live);
                    
                    if (photonView.IsMine)
                    {
                        inGameNetworkManager.쥐목숨Update(PhotonNetwork.LocalPlayer.NickName, 쥐목숨);
                        Vector3 감옥position= new Vector3(-39, 4, -27);
                        StartCoroutine(감옥GO(감옥position));
                        inGameNetworkManager.사망수UP();

                        //여기서 감옥으로 이동시킨다. 2초뒤에
                        //그리고 다시 살린다??
                    }
                    //기절


                }
                print(쥐목숨);
            }

        }
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(쥐맞음); // bool
                stream.SendNext(쥐목숨); // int
                stream.SendNext(live);   // bool
            }
            else
            {
                쥐맞음 = (bool)stream.ReceiveNext();
                쥐목숨 = (int)stream.ReceiveNext();
                live = (bool)stream.ReceiveNext();
            }
        }

        public void 순간이동(Vector3 spawnPositioni,int num)
        {
            photonView.RPC("순간이동RPC", RpcTarget.All, spawnPositioni,num);
        }

        [PunRPC]
        private void 순간이동RPC(Vector3 spawnPositioni, int num)
        {
            if (photonView.IsMine)
            {
                순간이동live = false;
               
                print("진짜이동함");
                transform.position = spawnPositioni;
                StartCoroutine(순간이동코루틴(spawnPositioni, num));

            }
        }

        IEnumerator 순간이동코루틴(Vector3 spawnPositioni, int num)
        {
            while(true)
            {
                yield return new WaitForSeconds(0.5f);
                if (transform.position == spawnPositioni)
                {
                    순간이동live = true;
                    if(num==1)//num으로 확인을 해줘야 됨.......
                    {
                        //살림();
                    }
                    break;
                }
                else
                {
                    transform.position = spawnPositioni;
                }               
            }
        }

        IEnumerator 감옥GO(Vector3 spawnPositioni)
        {
            yield return new WaitForSeconds(2f);
            _animator.ResetTrigger("DieToMove");
            살림();
            순간이동(spawnPositioni,1);
            
        }

        [PunRPC]
        void falshGet()
        {
            flash.SetActive(true);
        }


        [PunRPC]
        void falshOn()
        {
            flashlight.SetActive(true);
            StartCoroutine(falshOff2());
        }

        [PunRPC]
        void falshOff()
        {
            flash.SetActive(false);
            flashlight.SetActive(false);

        }

        IEnumerator falshOff2()
        {
            yield return new WaitForSeconds(2f);
            flash.SetActive(false);
            flashlight.SetActive(false);
            if (photonView.IsMine)
            {
                falshImage.SetActive(false);
            }
        }

        IEnumerator 빛맞음()
        {
            lightflag = true;
            lightPanel.SetActive(true);
            yield return new WaitForSeconds(3f);
            lightflag = false;
            lightPanel.SetActive(false);
        }

        [PunRPC]
        void AttackSoundRPC()
        {
            AttackSound.Play();
        }
        //만약 후래쉬를 획득하면 후래쉬를 RPC로 true하고 후래쉬를 가지고 있는 상태에서 사용을 하면 false한다.
    }
}
