//using System.Numerics;
using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
using UnityEngine.UI;
using test;
#endif



namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    // add hunger system eventually
    public class ThirdPersonController : MonoBehaviour
    {
        [SerializeField] Transform aim;
        [SerializeField] LineRenderer lineRenderer;
        //probably should add a teleport animation at some point
        public InputAction tp;
        public Material playerMat;
        public InputAction ignite;
        public GameObject stealthParticles;
        public InputAction hasten;
        public InputAction mouseLoc;
        public bool isSpeed = false;
        public UnityEngine.UI.Image bar;
       
        private bool isStealth = false;
        public float mana = 100;
        public float saturation = 100;
        public float saturationChange;

        public float manaChange = 1;
        public Camera cam;
        public GameObject fire;
        [Header("Player")]
        //[Tooltip("Move speed of the character in m/s")]
        //public float MoveSpeed = 2.0f;
        //removed to fit the needs of the game

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
        public InputAction pos;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        public Monster monster;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

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


        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

        }

        private void Start()
        {
            stealthParticles.SetActive(false);

            ignite = InputSystem.actions.FindAction("Light");
            hasten = InputSystem.actions.FindAction("Speed");
            tp = InputSystem.actions.FindAction("Teleport");
            pos = InputSystem.actions.FindAction("Position");
            mouseLoc = InputSystem.actions.FindAction("Look");
            fire.SetActive(false);
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
        }
        

        private void Update()
        {
            RaycastHit hit1;
            RaycastHit hit2;
            RaycastHit hit3;
            RaycastHit hit4;
            Physics.Raycast(transform.position, Vector3.forward, out hit1, 1);
            Physics.Raycast(transform.position + Vector3.up, Vector3.back, out hit2, 1);
            Physics.Raycast(transform.position + Vector3.up, Vector3.left, out hit3, 1);
            Physics.Raycast(transform.position + Vector3.up, Vector3.right, out hit4, 1);
            Debug.DrawRay(transform.position + Vector3.up, Vector3.forward, Color.red, 100);
            //Debug.Log(hit1.collider);
            if (hit1.collider != null)
            {
                //Debug.Log("yes");
                if (hit1.collider.gameObject.GetComponent<Monster>())
                {
                    monster.GetComponent<Monster>().Jumpscare();
                }

            }
            if (hit2.collider != null)
            {
                if (hit2.collider.gameObject.GetComponent<Monster>())
                {
                    monster.GetComponent<Monster>().Jumpscare();
                }

            }
            if (hit3.collider != null)
            {
                if (hit3.collider.gameObject.GetComponent<Monster>())
                {
                    monster.GetComponent<Monster>().Jumpscare();
                }

            }
            if (hit4.collider != null)
            {
                if (hit4.collider.gameObject.GetComponent<Monster>())
                {
                    monster.GetComponent<Monster>().Jumpscare();
                }

            }
            _hasAnimator = TryGetComponent(out _animator);
            ManaUpdate();
            //RaycastHit hit;
            //Physics.Raycast(aim.position, cam.ScreenToWorldPoint(new UnityEngine.Vector3(mouseLoc.ReadValue<UnityEngine.Vector2>().x, mouseLoc.ReadValue<UnityEngine.Vector2>().y, 10)), out hit, 10);
            //Debug.Log(Mouse.current.position);
            //Ray r = cam.ScreenPointToRay(new UnityEngine.Vector3(440, 217.5f, 0));

            //Physics.Raycast(r, out hit);
            //lineRenderer.enabled = true;
            //lineRenderer.SetPosition(0, aim.position);
            //lineRenderer.SetPosition(1, hit.point);
            JumpAndGravity();
            GroundedCheck();
            Move();
        }
        public void ManaUpdate()
        {
            mana += manaChange / 100;
            saturation += saturationChange / 100;
            if (mana > 100)
            {
                mana = 100;
            }
            if (mana <= 0)
            {
                DeactivateAll();
            }
            if (saturation > 100)
            {
                saturation = 100;
            }
            if (saturation < 0)
            {
                // call death function
            }

            bar.fillAmount = mana / 100;
        }
        public void ChangeMana(float m)
        {
            mana += m;
        }

        private void LateUpdate()
        {
            CameraRotation();
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
            UnityEngine.Vector3 spherePosition = new UnityEngine.Vector3(transform.position.x, transform.position.y - GroundedOffset,
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

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * 2;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * 2;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = UnityEngine.Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {

            float targetSpeed = SprintSpeed;
            // removed walking and kept at sprinting for the horror game feel
            //I wanted the speed up to be attached to the mana system too


            if (_input.move == UnityEngine.Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new UnityEngine.Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

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
            UnityEngine.Vector3 inputDirection = new UnityEngine.Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != UnityEngine.Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = UnityEngine.Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            UnityEngine.Vector3 targetDirection = UnityEngine.Quaternion.Euler(0.0f, _targetRotation, 0.0f) * UnityEngine.Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new UnityEngine.Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
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
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
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
                new UnityEngine.Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = UnityEngine.Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
        private void OnStealth()
        {
            if (isStealth)
            {
                stealthParticles.SetActive(false);
                isStealth = false;
                manaChange += 1;
            }
            else
            {
                stealthParticles.SetActive(true);
                isStealth = true;
                manaChange -= 1;
            }
            //Debug.Log(gameObject.GetComponent<Renderer>().material.color.a);
            //Color b = gameObject.GetComponent<Renderer>().material.color;
            //Color col = new Color(b.r, b.g, b.b, 0);
            //gameObject.GetComponent<Renderer>().material.SetColor("_Color", col);
            //Debug.Log(gameObject.GetComponent<Renderer>().material.color.a);

            //tester.GetComponent<Renderer>().material.ChangeAlpha(0);
        }
        public bool InStealth()
        {
            return isStealth;
        }

        private void OnLight()
        {
            fire.SetActive(!fire.gameObject.activeSelf);
            if (fire.activeSelf)
            {
                manaChange -= 2f;
            }
            else
            {
                manaChange += 2f;
            }
            Debug.Log(mana);
        }
        public void LightOff()
        {
            if (fire.activeSelf)
            {
                fire.SetActive(false);
                manaChange += 2f;
            }

        }
        public void DeactivateAll()
        {
            LightOff();
            SpeedOff();
        }
        // manipulation of speed variable in spell as game mechanic
        private void OnSpeed()
        {
            isSpeed = !isSpeed;
            if (isSpeed)
            {

                SprintSpeed *= 2;
                manaChange -= 3;
            }
            else
            {

                SprintSpeed /= 2;
                manaChange += 3;
            }
        }
        public void SpeedOff()
        {
            if (isSpeed)
            {
                isSpeed = false;

                SprintSpeed /= 2;
                manaChange += 3;
            }


        }
        public void OnTeleport()
        {
            if (mana > 40)
            {
                RaycastHit hit;
                //Physics.Raycast(Camera.main.transform.position, new Vector3(Camera.main.ScreenToWorldPoint(mouseLoc.ReadValue<Vector2>()).x, Camera.main.ScreenToWorldPoint(mouseLoc.ReadValue<Vector2>()).y, 100), out hit, 100);
                //Physics.Raycast(Camera.main.transform.position, )
                Ray r = Camera.main.ScreenPointToRay(new UnityEngine.Vector3(440, 217.5f, 0));
                Physics.Raycast(r, out hit, 10);
                if (hit.collider != null)
                {
                    _controller.enabled = false;
                    transform.position = hit.point;

                    _controller.enabled = true;

                    //Debug.Log(transform.position);
                    //Debug.Log(hit.transform.position);

                }
                else
                {
                    Debug.Log("yes");
                    _controller.enabled = false;
                    transform.position += (transform.position - aim.position).normalized * 5;


                    _controller.enabled = true;
                }
                mana -= 40;
            }



        }




    }


}
namespace test {
    public static class ex
    {
        public static void ChangeAlpha(this Material mat, float alphaValue)
        {
            Color oldColor = mat.color;
            Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alphaValue);         
            mat.SetColor("_Color", newColor);               
        }
    }


}