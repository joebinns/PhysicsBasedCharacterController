using UnityEngine.InputSystem;

namespace JoeBinns.Inputs
{
	public class InputActionBinding
	{
		public InputAction InputAction { get; private set; }
		public ProcessedInputAction ProcessedInputAction { get; private set; }

		public void Assign<T>(InputAction inputAction, ref T processedInputAction) where T : ProcessedInputAction
		{
			InputAction = inputAction;
			ProcessedInputAction = processedInputAction;
		}

		public void Bind()
		{
			InputAction.started += ProcessedInputAction.Start;
			InputAction.performed += ProcessedInputAction.Perform;
			InputAction.canceled += ProcessedInputAction.Cancel;

			ProcessedInputAction.Bind(InputAction);
		}

		public void Unbind()
		{
			InputAction.started -= ProcessedInputAction.Start;
			InputAction.performed -= ProcessedInputAction.Perform;
			InputAction.canceled -= ProcessedInputAction.Cancel;

			ProcessedInputAction.Unbind();
		}
    }
}