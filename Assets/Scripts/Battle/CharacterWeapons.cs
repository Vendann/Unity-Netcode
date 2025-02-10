using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Triwoinmag {
	public class CharacterWeapons : NetworkBehaviour {
		public CharacterCore Core;

		public List<IWeapon> Weapons = new List<IWeapon>();
		[field: SerializeField] public int SelectedWeaponId { get; private set; } = 0;

		public float MaxDistanceToTarget = 250f;
		[SerializeField] private LayerMask _layerMask = new LayerMask();

		[Header("Debugging")]
		public bool Debugging;
		[SerializeField] private GameObject _testPrefab;

		private void Awake() {
			if (Core == null)
				Core = GetComponentInParent<CharacterCore>();
			Weapons = GetComponentsInChildren<IWeapon>().ToList(); // На всякий случай, список оружий формируется именно здесь
										// Берутся дочерние объекты у CharacterWeapons из префаба игрока и переносятся в список
		}

		private void Update() {
			if (IsOwner) { // Добавляем проверку на владельца, чтобы метод выстрела вызывался только у него
				if (Input.GetMouseButtonDown(0)) {
					FireWeapons(); // Этот метод перебирает все оружия в списке и вызывает метод выстрела у каждого
				}
			}
		}

		public void FireWeapons() {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));

			if (Physics.Raycast(ray, out hit, MaxDistanceToTarget, _layerMask)) {
				if (Debugging) {
					Debug.Log($"FireWeapons. Object: {hit.transform.gameObject.name} ray.origin: {ray.origin}, hit.point: {hit.point}");
					//Debug.DrawRay(ray.origin, Camera.main.transform.TransformDirection(Vector3.forward) * hit.distance, Color.red, 3.0f);
					//Instantiate(_testPrefab, hit.point, Quaternion.identity);
				}
				Weapons[SelectedWeaponId].Attack(hit.point);

			}
			else {
				Weapons[SelectedWeaponId].Attack(ray.origin + ray.direction * MaxDistanceToTarget);
			}

		}
	}
}
