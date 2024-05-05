using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using JoeBinns.Spacetime.Inputs;

namespace JoeBinns.Inputs
{
	public class InputManager : Singleton<InputManager>
	{
		// CONTROLS

		public Controls Controls { get; private set; }

		// INPUT ACTION MAPS

		private Dictionary<InputActionMap, int> _inputActionMapToNumAssignedInputActionBindings = new(); 
		private Dictionary<InputActionMap, InputActionBinding[]> _inputActionMapToInputActionBindings = new();
		private List<InputActionMap> _boundInputActionMaps = new();

		// PROCESSED INPUT ACTIONS

		//// CHARACTER CONTROLLER

		private ProcessedInputAction<Vector2> _move = new();
		private ProcessedInputAction<Vector2> _look = new();
		private ProcessedInputAction<float> _jump = new();
		private ProcessedInputAction<float> _throw = new();
		private ProcessedInputActionPress _sprint = new();

		public ProcessedInputAction<Vector2> Move => _move;
		public ProcessedInputAction<Vector2> Look => _look; // NOTE: The corresponding Unity input action is being accessed directly by Cinemachine.
		public ProcessedInputAction<float> Jump => _jump;
		public ProcessedInputAction<float> Throw => _throw;
		public ProcessedInputActionPress Sprint => _sprint;

		//// INTERACTION

		private ProcessedInputActionPress _interact = new();
		private ProcessedInputActionPress _grab = new();
		private ProcessedInputActionPress _drop = new();

		public ProcessedInputActionPress Interact => _interact;
		public ProcessedInputActionPress Grab => _grab;
		public ProcessedInputActionPress Drop => _drop;

		//// VEHICLE

		private ProcessedInputAction<float> _steer = new();
		private ProcessedInputAction<float> _drive = new();
		private ProcessedInputAction<float> _brake = new();

		public ProcessedInputAction<float> Steer => _steer;
		public ProcessedInputAction<float> Drive => _drive;
		public ProcessedInputAction<float> Brake => _brake;

		//// PAUSE

		private ProcessedInputActionPress _pause = new();

		public ProcessedInputActionPress Pause => _pause;

		//// UI
		// NOTE: The corresponding Unity input action map is being accessed directly by Event System.

		private ProcessedInputAction<Vector2> _navigate = new();
		private ProcessedInputAction<Vector2> _point = new();
		private ProcessedInputActionPress _submit = new();
		private ProcessedInputActionPress _reset = new();
		private ProcessedInputActionPress _cancel = new();
		private ProcessedInputActionPress _click = new();

		public ProcessedInputAction<Vector2> Navigate => _navigate;
		public ProcessedInputAction<Vector2> Point => _point;
		public ProcessedInputActionPress Submit => _submit;
		public ProcessedInputActionPress Reset => _reset;
		public ProcessedInputActionPress Cancel => _cancel;
		public ProcessedInputActionPress Click => _click;

		// DEVICE

		private InputDevice[] _devices = new InputDevice[1] { null };

		// CONTROL SCHEME

		private InputControlScheme _controlScheme;
		public InputControlScheme ControlScheme
		{
			get => _controlScheme;
			private set
			{
				if (_controlScheme == value) return;

				_controlScheme = value;
				OnControlSchemeChanged?.Invoke(_controlScheme);
				Debug.Log($"New control scheme detected: {_controlScheme.name}.");
			}
		}

		// EVENTS

		public event Action<InputControlScheme?> OnControlSchemeChanged;

		// INITIALIZATION

		protected override void Awake()
		{
			base.Awake();

			InitializeInputActionBindings();
			SetCursorActive(false);
		}

		private void OnEnable()
		{
			InputSystem.onEvent += OnInputSystemEvent;
			BindInputActionMap(Controls.CharacterController);
			BindInputActionMap(Controls.Interaction);
			BindInputActionMap(Controls.Pause);
			BindInputActionMap(Controls.UI);
			Controls.Enable();
		}

		private void OnDisable()
		{
			Controls.Disable();
			UnbindInputActions();
			InputSystem.onEvent -= OnInputSystemEvent;
		}

		// LATE UPDATE

		private void LateUpdate()
		{
			ResetStateMasks();
		}

		// INITIALIZE INPUT ACTION BINDINGS

		private void InitializeInputActionBindings()
		{
			Controls = new Controls();
			CreateInputActionBindings();
			AssignInputActionBindings();

#if !SHIPPING
			foreach (var inputActionMap in Controls.asset.actionMaps)
			{
				var numAssignedInputActionBindings = _inputActionMapToNumAssignedInputActionBindings[inputActionMap];
				var numInputActionBindings = _inputActionMapToInputActionBindings[inputActionMap].Length;
				if (numAssignedInputActionBindings < numInputActionBindings)
				{
					Debug.LogWarning(
						$"Assigned fewer InputActionBindings ({numAssignedInputActionBindings}) than the total number of InputActions ({numInputActionBindings}) in the InputActionMap ({inputActionMap.name}). \n" +
						$"You may find that some ProcessedInputActions do not work; check that AssignInputActionBinding() is called exactly once for each input action in {Controls.asset.name}.inputactions."
						);
				}
			}
#endif
		}

		private void CreateInputActionBindings()
		{
			foreach (var actionMap in Controls.asset.actionMaps)
			{
				var inputActionBindings = new InputActionBinding[actionMap.actions.Count];
				for (var i = 0; i < inputActionBindings.Length; i++)
				{
					inputActionBindings[i] = new InputActionBinding();
				}
				_inputActionMapToNumAssignedInputActionBindings.Add(actionMap, 0);
				_inputActionMapToInputActionBindings.Add(actionMap, inputActionBindings);
			}
		}

		/// <summary>
		/// When new input actions are added, make sure to add them here.
		/// </summary>
		private void AssignInputActionBindings()
		{
			AssignInputActionBinding(Controls.CharacterController.Move, ref _move);
			AssignInputActionBinding(Controls.CharacterController.Look, ref _look);
			AssignInputActionBinding(Controls.CharacterController.Jump, ref _jump);
			AssignInputActionBinding(Controls.CharacterController.Throw, ref _throw);
			AssignInputActionBinding(Controls.CharacterController.Sprint, ref _sprint);

			AssignInputActionBinding(Controls.Interaction.Interact, ref _interact);
			AssignInputActionBinding(Controls.Interaction.Grab, ref _grab);
			AssignInputActionBinding(Controls.Interaction.Drop, ref _drop);

			AssignInputActionBinding(Controls.Vehicle.Steer, ref _steer);
			AssignInputActionBinding(Controls.Vehicle.Drive, ref _drive);
			AssignInputActionBinding(Controls.Vehicle.Brake, ref _brake);

			AssignInputActionBinding(Controls.Pause.Pause, ref _pause);

			AssignInputActionBinding(Controls.UI.Navigate, ref _navigate);
			AssignInputActionBinding(Controls.UI.Point, ref _point);
			AssignInputActionBinding(Controls.UI.Submit, ref _submit);
			AssignInputActionBinding(Controls.UI.Reset, ref _reset);
			AssignInputActionBinding(Controls.UI.Cancel, ref _cancel);
			AssignInputActionBinding(Controls.UI.Click, ref _click);
		}

		private void AssignInputActionBinding<T>(InputAction inputAction, ref T processedInputAction) where T : ProcessedInputAction
		{
			var numAssignedInputActionBindings = _inputActionMapToNumAssignedInputActionBindings[inputAction.actionMap];
			var inputActionBindings = _inputActionMapToInputActionBindings[inputAction.actionMap];

			if (numAssignedInputActionBindings >= inputActionBindings.Length)
			{
#if !SHIPPING
				Debug.LogError(
					$"Attempted to assign more InputActionBindings than the total number of InputActions ({inputActionBindings.Length}) in the InputActionMap ({inputAction.actionMap.name}). \n" +
					$"Check that AssignInputActionBinding() is called exactly once for each InputAction in {Controls.asset.name}.inputactions."
					);
#endif
				return;
			}

			inputActionBindings[numAssignedInputActionBindings].Assign(inputAction, ref processedInputAction);
			_inputActionMapToNumAssignedInputActionBindings[inputAction.actionMap] = numAssignedInputActionBindings + 1;
		}

		// BINDING

		public void BindInputActionMap(InputActionMap inputActionMap)
		{
			var inputActionBindings = _inputActionMapToInputActionBindings[inputActionMap];
			foreach (var inputActionBinding in inputActionBindings)
			{
				inputActionBinding.Bind();
			}
			_boundInputActionMaps.Add(inputActionMap);
		}

		// UNBINDING

		public void UnbindInputActions()
		{
			foreach (var inputActionMap in _boundInputActionMaps.ToArray())
			{
				UnbindInputActionMap(inputActionMap);
			}
		}

		public void UnbindInputActionMap(InputActionMap inputActionMap)
		{
			var inputActionBindings = _inputActionMapToInputActionBindings[inputActionMap];
			foreach (var inputActionBinding in inputActionBindings)
			{
				inputActionBinding.Unbind();
			}
			_boundInputActionMaps.Remove(inputActionMap);
		}

		// STATE MASKS

		/// <summary>
		/// Reset state masks to support checking if a button began being pressed (etc.) on the next update.
		/// </summary>
		private void ResetStateMasks()
		{
			foreach (var inputActionMap in _boundInputActionMaps)
			{
				ResetStateMasks(inputActionMap);
			}
		}

		private void ResetStateMasks(InputActionMap inputActionMap)
		{
			var inputActionBindings = _inputActionMapToInputActionBindings[inputActionMap];
			foreach (var inputActionBinding in inputActionBindings)
			{
				var processedInputAction = inputActionBinding.ProcessedInputAction;
				processedInputAction.StateMask.Clear();
			}
		}

		// MOUSE

		public Vector3 GetMouseWorldPosition()
		{
			Vector3 mouseScreenPosition = GetMouseScreenPosition();
			mouseScreenPosition.z = Camera.main.nearClipPlane;
			return Camera.main.ScreenToWorldPoint(mouseScreenPosition);
		}

		public Vector2 GetMouseScreenPosition()
		{
			return Mouse.current.position.ReadValue();
		}

		public void SetCursorActive(bool enabled)
		{
			Cursor.lockState = enabled ? CursorLockMode.Confined : CursorLockMode.Locked;
			Cursor.visible = enabled;
		}

		// CONTROL SCHEME SWITCHING

		private void OnInputSystemEvent(InputEventPtr eventPtr, InputDevice device)
		{
			if (TryGetControlSchemeForDevice(device, out var controlScheme))
			{
				ControlScheme = controlScheme;
			}
		}

		private bool TryGetControlSchemeForDevice(InputDevice device, out InputControlScheme controlScheme)
		{
			_devices[0] = device;
			var nullableControlScheme = InputControlScheme.FindControlSchemeForDevices(_devices, Controls.controlSchemes, allowUnsuccesfulMatch: true);
			var wasControlSchemeFound = nullableControlScheme.HasValue;
			if (wasControlSchemeFound)
			{
				controlScheme = nullableControlScheme.Value;
			}
			return wasControlSchemeFound;
		}
	}
}
