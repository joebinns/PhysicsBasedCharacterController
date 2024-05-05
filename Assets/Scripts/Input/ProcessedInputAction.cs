using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using JoeBinns.Utilities;

namespace JoeBinns.Inputs
{
	/// <summary>
	/// The integer values are assigned to powers of 2, for ease of use with masks.
	/// </summary>
	public enum InputState
	{
		Start = 1,
		Perform = 2,
		Cancel = 4
	}

	public abstract class ProcessedInputAction
	{
		public InputAction InputAction { get; protected set; }

		public bool IsInProgress { get; protected set; } = false;

		/// <summary>
		/// NOTE: A state mask is used to allow consecutive multiple states to be stored.
		/// This is because states changing in the same frame are problematic for detecting the input state from update.
		/// For instance, context.started and context.performed are called during the same frame for buttons without a hold processor.
		/// </summary>
		public StateMask StateMask { get; private set; } = new();

		protected void AddState(InputState inputState)
		{
			StateMask.Add((int)inputState);

			switch (inputState)
			{
				case InputState.Start:
					IsInProgress = true;
					break;
				case InputState.Perform:
					IsInProgress = true;
					break;
				case InputState.Cancel:
					IsInProgress = false;
					break;
			}
		}

		public abstract void Start(InputAction.CallbackContext context);
		public abstract void Perform(InputAction.CallbackContext context);
		public abstract void Cancel(InputAction.CallbackContext context);
		public abstract void Bind(InputAction inputAction);
		public abstract void Unbind();
	}

	public class ProcessedInputAction<T> : ProcessedInputAction where T : struct
	{
		public T Value { get; private set; }

		public event Action<T> OnStart;
		public event Action<T> OnPerform;
		public event Action<T> OnCancel;
		public event Action OnBind;
		public event Action OnUnbind;

		public override void Start(InputAction.CallbackContext context)
		{
			Value = context.ReadValue<T>();
			AddState(InputState.Start);
			OnStart?.Invoke(Value);
		}

		public override void Perform(InputAction.CallbackContext context)
		{
			Value = context.ReadValue<T>();
			AddState(InputState.Perform);
			OnPerform?.Invoke(Value);
		}

		public override void Cancel(InputAction.CallbackContext context)
		{
			Value = context.ReadValue<T>();
			AddState(InputState.Cancel);
			OnCancel?.Invoke(Value);
		}

		public override void Bind(InputAction inputAction)
		{
			InputAction = inputAction;
			Value = inputAction.ReadValue<T>();

			// Reset state mask
			if (inputAction.WasPressedThisFrame()) AddState(InputState.Start);
			if (inputAction.WasPerformedThisFrame()) AddState(InputState.Perform);
			if (inputAction.WasReleasedThisFrame()) AddState(InputState.Cancel);
			IsInProgress = inputAction.IsInProgress();

			OnBind?.Invoke();
		}

		public override void Unbind()
		{
			Value = default;

			// Clear state mask
			StateMask.Clear();
			IsInProgress = false;

			OnUnbind?.Invoke();
		}
	}

	public class ProcessedInputActionPress : ProcessedInputAction<float>
	{
		public event Action OnPress;

		public override void Perform(InputAction.CallbackContext context)
		{
			base.Perform(context);

			if (context.interaction is not PressInteraction) return;

			OnPress?.Invoke();
		}
	}
}
