using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Content.Scripts.Input
{
	public class InputManager : MonoBehaviour
	{
		private InputSettings inputSettings;
		private void Awake()
		{
			inputSettings = new InputSettings();
		}

		private void Start()
		{
			SetCursorState(true);
			EnablePlayerInput(true);
			inputSettings.UI.Enable();
		}

		public void EnablePlayerInput(bool on)
		{
			if (on)
				inputSettings.Player.Enable();
			else
				inputSettings.Player.Disable();
		}

		public Vector2 GetMovement()
		{
			return inputSettings.Player.Move.ReadValue<Vector2>();
		}

		public Vector2 GetLook()
		{
			return inputSettings.Player.Look.ReadValue<Vector2>();
		}

		public bool IsJumpTriggered()
		{
			return inputSettings.Player.Jump.WasPressedThisFrame();
		}
		
		public bool IsUseTriggered()
		{
			return inputSettings.Player.Use.WasPressedThisFrame();
		}

		public bool IsRunPressed()
		{
			return inputSettings.Player.Sprint.IsPressed();
		}
		
		public bool IsPeakLeftPressed()
		{
			return inputSettings.Player.PeakLeft.IsPressed();
		}
		
		public bool IsPeakRightPressed()
		{
			return inputSettings.Player.PeakRight.IsPressed();
		}

		public bool IsCrouchTriggered()
		{
			return inputSettings.Player.Crouch.WasPressedThisFrame();
		}

		public bool IsPauseTriggered()
		{
			return inputSettings.UI.Pause.WasPressedThisFrame();
		}
		
		public void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}