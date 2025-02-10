using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HelloWorld;
using IG;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Triwoinmag {
    public class ManagerScore : SingletonManager<ManagerScore>
    {
        private Dictionary<ulong, int> _playersClientIdToPlayerScore = new Dictionary<ulong, int>(); // Словарь, ключ - ID, значение - кол-во очков

        // Debugging
        [SerializeField] private List<ulong> _listPlayersClientId = new List<ulong>(); // Список ID, чтобы видеть в окне движка
        [SerializeField] private List<int> _listPlayersScore = new List<int>(); // Аналогично для очков

        public Action<ulong, int> OnAddScore;
        public Action<ulong, int> OnNewScore;

        public UnityEvent<ulong, int> EventAddScore;

        private void Start() {
        }

        private void OnEnable() {
            OnAddScore += IncreaseScore;
        }
        private void OnDisable() {
            OnAddScore -= IncreaseScore;
        }

        public void AddScore(ulong id, int scoreDelta) {
            if(Debugging)
                Debug.Log("AddScore", this);
            OnAddScore?.Invoke(id, scoreDelta);
        }

        public void IncreaseScore(ulong id, int scoreDelta) {
            if (!_playersClientIdToPlayerScore.TryGetValue(id, out int score)) {
                _playersClientIdToPlayerScore[id] = scoreDelta;
            }
            else {
                _playersClientIdToPlayerScore[id] += scoreDelta;
            }

            foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds) {
                if (uid == id) {
                    NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<ClientCharScore>().
                        ReceiveNewScore(_playersClientIdToPlayerScore[id]);
                }
            }
            // Debug
            if (Debugging) {
                _listPlayersClientId = _playersClientIdToPlayerScore.Select(x => x.Key).ToList();
                _listPlayersScore = _playersClientIdToPlayerScore.Select(x => x.Value).ToList();
            }

            OnNewScore?.Invoke(id, _playersClientIdToPlayerScore[id]);
            
            EventAddScore.Invoke(id, scoreDelta);
        }
    }
}