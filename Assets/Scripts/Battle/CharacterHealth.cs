using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Triwoinmag;

namespace Triwoinmag {
    public class CharacterHealth : NetworkBehaviour, IDamageable {
        [SerializeField] private bool Debugging;

        [SerializeField] private float _health = 100;
        public float Health => _health;
        [field: SerializeField] public float MaxHealth { get; set; } = 100;

        [SerializeField] private GameObject OnDeathPrefab;

        private void Start() {

        }


        public void ReceiveDamage(float damageAmount, Vector3 hitPosition, GameAgent sender) {
            _health -= damageAmount;
            if (Debugging) {
                Debug.Log($"CharacterHealth.ReceiveDamage. New Health: {_health}. Attacker: {sender.gameObject.name}. Attacker faction: {sender.ShipFaction}");
            }

            if (IsServer) { // Если метод вызван на сервере, то у ManagerScore вызываем метод AddScore
                // sender - это тот, кто атаковал, типа GameAgent, берем у него OwnerClientID, чтобы именно владельца найти, т.е. атаковавшего
                // И даем ему 5 очков
                ManagerScore.I.AddScore(sender.OwnerClientId, 5);
            }

            if (_health <= 0) {
                Instantiate(OnDeathPrefab, transform.position, Quaternion.identity);
                if (IsServer) { // То же самое делаем при уничтожении жертвы, но добавляем уже 100 очков
                    ManagerScore.I.AddScore(sender.OwnerClientId, 100);
                    Destroy(gameObject);
                }
            }
        }

        public void ReceiveHeal(float healAmount, Vector3 hitPosition, GameAgent sender) {
            _health += healAmount;
        }
    }
}