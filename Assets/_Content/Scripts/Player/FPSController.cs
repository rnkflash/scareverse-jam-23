using System.Collections;
using _Content.Scripts.Input;
using UnityEngine;
using UnityEngine.Events;

namespace _Content.Scripts.Player
{
	[RequireComponent(typeof(CharacterController))]
	public class FPSController : MonoBehaviour
	{
		public float stamina = 10.0f;
		public float staminaDepletionSpeed = 2.0f;
		public float staminaRechargeSpeed = 1.0f;
		public float staminaRechargeAfterSec = 1.0f;
		public float moveSpeed = 4.0f;
		public float sprintSpeed = 6.0f;
		public float rotationSpeed = 1.0f;
		public float speedChangeRate = 10.0f;
		public float jumpHeight = 1.2f;
		public float gravity = -15.0f;
		public float jumpTimeout = 0.1f;
		public float fallTimeout = 0.15f;
		public bool isGrounded = true;
		public float groundedOffset = -0.14f;
		public float groundedRadius = 0.5f;
		public LayerMask groundLayers;
		public float topClamp = 90.0f;
		public float bottomClamp = -90.0f;
		public float headBobSpeed = 14f;
		public float headBobAmount = 0.01f;
		public float headBobSpeedSprinting = 18f;
		public float headBobAmountSprinting = 0.05f;
		public Transform cameraRootObject;
		public Transform headBobObject;
		public Transform peakRootObject;
		public Transform peakLeft;
		public Transform peakRight;
		public Transform peakCenter;
		public float peakTime = 0.15f;

		public float standingHeight = 1.8f;
		public Vector3 standingCenter = new Vector3(0,0.90f,0);
		public float crouchHeight = 0.9f;
		public Vector3 crouchCenter = new Vector3(0,0.45f,0);
		public bool isCrouching;
		public float timeToCrouch = 0.15f;
		public bool isInCrouchingAnimation;
		public float footStepsDistance = 2.0f;
		public AudioClip[] footStepsClips;

		public UnityEvent<float> onStaminaChange;

		private float headBobTimer;
		private float headBobDefaultLocalY;
		private float cameraRootStandingHeight;
		private float cameraRootCrouchingHeight;
		private float cinemachineTargetPitch;
		private float speed;
		private float rotationVelocity;
		private float verticalVelocity;
		private readonly float terminalVelocity = 53.0f;
		private float jumpTimeoutDelta;
		private float fallTimeoutDelta;
		private float footStepsDistanceTravelled;
		private float currentStamina = 10.0f;
		private bool canSprint = true;
		private float staminaRechargeCooldown;
		private bool canRechargeStamina;
	
		private CharacterController controller;
		private InputManager input;
		private GameObject mainCamera;
		private AudioSource audioSource;

		private const float Threshold = 0.01f;

		private void Awake()
		{
			if (mainCamera == null)
				mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		}

		private void Start()
		{
			controller = GetComponent<CharacterController>();
			input = GetComponent<InputManager>();
			audioSource = GetComponent<AudioSource>();

			jumpTimeoutDelta = jumpTimeout;
			fallTimeoutDelta = fallTimeout;

			cameraRootStandingHeight = cameraRootObject.localPosition.y;
			cameraRootCrouchingHeight = crouchHeight + (cameraRootStandingHeight - controller.height);

			headBobDefaultLocalY = headBobObject.localPosition.y;
		}

		private void Update()
		{
			JumpAndGravity();
			GroundedCheck();
			Move();
			HeadBob();
			Crouch();
			FootSteps();
			Peak();
		}

		private void Peak()
		{
			var peakLeftPressed = input.IsPeakLeftPressed();
			var peakRightPressed = input.IsPeakRightPressed();

			if (peakLeftPressed)
			{
				peakRootObject.position = Vector3.Lerp(peakRootObject.position, peakLeft.position, peakTime);
				peakRootObject.rotation = Quaternion.Lerp(peakRootObject.rotation, peakLeft.rotation, peakTime);
			}
			else if (peakRightPressed)
			{
				peakRootObject.position = Vector3.Lerp(peakRootObject.position, peakRight.position, peakTime);
				peakRootObject.rotation = Quaternion.Lerp(peakRootObject.rotation, peakRight.rotation, peakTime);
			}
			else
			{
				peakRootObject.position = Vector3.Lerp(peakRootObject.position, peakCenter.position, peakTime);
				peakRootObject.rotation = Quaternion.Lerp(peakRootObject.rotation, peakCenter.rotation, peakTime);
			}
		}

		private void FootSteps()
		{
			if (!isGrounded) return;
			if (footStepsDistanceTravelled < footStepsDistance) return;
			footStepsDistanceTravelled = 0;

			if (footStepsClips.Length > 0)
			{
				var randomClip = footStepsClips[Random.Range(0, footStepsClips.Length)];
				audioSource.volume = isCrouching ? 0.5f : 1.0f;
				audioSource.PlayOneShot(randomClip);
			}
				
		}

		private void LateUpdate()
		{
			CameraRotation();
		}

		private void Crouch()
		{
			if (!isGrounded || isInCrouchingAnimation || !input.IsCrouchTriggered())
				return;
			
			isCrouching = !isCrouching;
			StartCoroutine(CrouchOrStandAnimation());
		}

		private IEnumerator CrouchOrStandAnimation()
		{
			isInCrouchingAnimation = true;

			float timeElapsed = 0;
			float targetHeight = isCrouching ? crouchHeight : standingHeight;
			float currentHeight = controller.height;
			Vector3 targetCenter = isCrouching ? crouchCenter : standingCenter;
			Vector3 currentCenter = controller.center;
			float targetCameraHeight = isCrouching ? cameraRootCrouchingHeight : cameraRootStandingHeight;
			Vector3 currentCameraLocalPosition = cameraRootObject.localPosition;

			while (timeElapsed < timeToCrouch)
			{
				controller.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
				controller.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
				cameraRootObject.localPosition = Vector3.Lerp(currentCameraLocalPosition,
					new Vector3(currentCameraLocalPosition.x, targetCameraHeight, currentCameraLocalPosition.z), timeElapsed / timeToCrouch);
				timeElapsed += Time.deltaTime;
				yield return null;
			}

			controller.height = targetHeight;
			controller.center = targetCenter;
			cameraRootObject.localPosition = new Vector3(currentCameraLocalPosition.x, targetCameraHeight,
				currentCameraLocalPosition.z);
			
			isInCrouchingAnimation = false;
		}

		private void GroundedCheck()
		{
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
			isGrounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
		}

		private void CameraRotation()
		{
			var look = input.GetLook();
			if (!(look.sqrMagnitude >= Threshold)) return;
			
			cinemachineTargetPitch += look.y * rotationSpeed;
			rotationVelocity = look.x * rotationSpeed;
			cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, bottomClamp, topClamp);
			cameraRootObject.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0.0f, 0.0f);
			transform.Rotate(Vector3.up * rotationVelocity);
		}

		private void Move()
		{
			var sprint = input.IsRunPressed() && canSprint;
			var move = input.GetMovement();
			float targetSpeed = sprint ? sprintSpeed : moveSpeed;
			if (move == Vector2.zero) 
				targetSpeed = 0.0f;

			float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = move.magnitude;

			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);
				speed = Mathf.Round(speed * 1000f) / 1000f;
			}
			else
			{
				speed = targetSpeed;
			}

			Vector3 inputDirection = new Vector3(move.x, 0.0f, move.y).normalized;
			if (move != Vector2.zero)
				inputDirection = transform.right * move.x + transform.forward * move.y;

			var horizontalVelocity = speed * Time.deltaTime;

			controller.Move(inputDirection.normalized * horizontalVelocity + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);

			if (isGrounded)
			{
				footStepsDistanceTravelled += horizontalVelocity;
			}
			
			if (sprint && targetSpeed > 0)
			{
				currentStamina -= staminaDepletionSpeed * Time.deltaTime;
				canRechargeStamina = false;
				staminaRechargeCooldown = staminaRechargeAfterSec;
			}
			else
			{
				if (canRechargeStamina)
					currentStamina += staminaRechargeSpeed * Time.deltaTime;
				else
				{
					staminaRechargeCooldown -= Time.deltaTime;
					if (staminaRechargeCooldown <= 0)
					{
						canRechargeStamina = true;
						staminaRechargeCooldown = 0;
					}
				}
				
			}
			currentStamina = Mathf.Clamp(currentStamina, 0, stamina);
			onStaminaChange?.Invoke(currentStamina / stamina);
			canSprint = currentStamina > 0;

		}

		private void JumpAndGravity()
		{
			var jump = input.IsJumpTriggered();
			if (isGrounded)
			{
				fallTimeoutDelta = fallTimeout;
				if (verticalVelocity < 0.0f)
					verticalVelocity = -2f;

				if (jump && jumpTimeoutDelta <= 0.0f)
					verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

				if (jumpTimeoutDelta >= 0.0f)
					jumpTimeoutDelta -= Time.deltaTime;
			}
			else
			{
				jumpTimeoutDelta = jumpTimeout;
				if (fallTimeoutDelta >= 0.0f)
					fallTimeoutDelta -= Time.deltaTime;
			}

			if (verticalVelocity < terminalVelocity)
				verticalVelocity += gravity * Time.deltaTime;
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

			if (isGrounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z), groundedRadius);
		}

		private void HeadBob()
		{
			var sprint = input.IsRunPressed() && canSprint;
			
			if (!isGrounded || controller.velocity.magnitude < 0.1f)
				return;

			headBobTimer += Time.deltaTime * (sprint ? headBobSpeedSprinting : headBobSpeed);
			headBobObject.localPosition = new Vector3(
				headBobObject.transform.localPosition.x,
				headBobDefaultLocalY + Mathf.Sin(headBobTimer) * (sprint ? headBobAmountSprinting : headBobAmount),
				headBobObject.localPosition.z
			);
		}
	}
}