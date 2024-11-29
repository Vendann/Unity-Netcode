using UnityEngine;
using Unity.Netcode;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Triwoinmag {
	public class ClientPlayerInput : NetworkBehaviour {
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		[SerializeField] private bool jump; // Защищенная переменная ввода прыжка
		public bool Jump { // Свойство, через которое осуществляется доступ к переменной
			get { return jump; } // чтение, просто возвращаем jump
			set {
				jump = value; // запись, устанавливаем значение при вводе от игрока
				if (IsOwner) // Если клиент - владелец объекта, то устанавливаем NetworkVariable для синхронизации
					NetVarJump.Value = value;
			}
		}
		public bool sprint;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

        [Header("Refs")]
        [SerializeField] private PlayerInput _playerInput;

        public NetworkVariable<bool> NetVarJump = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner); // Переменная NetworkVariable для прыжка

        public override void OnNetworkSpawn() {
			if (!IsOwner) return;
			_playerInput.enabled = true;
		}

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			Jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}