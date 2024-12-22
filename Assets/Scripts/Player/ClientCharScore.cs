using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Triwoinmag {
    public class ClientCharScore : NetworkBehaviour {
        [SerializeField] private UIPlayerCharScore _uIPlayerCharScore;
        [SerializeField] private NetworkVariable<int> _charScore = new NetworkVariable<int>();

        public override void OnNetworkSpawn() {
            if (!IsOwner) return;

            _uIPlayerCharScore = FindObjectOfType<UIPlayerCharScore>();
            _charScore.OnValueChanged += OnCharScoreChanged;
        }

        public override void OnNetworkDespawn() {
            // base.OnNetworkDespawn();
            _charScore.OnValueChanged -= OnCharScoreChanged;
        }

        public void ReceiveNewScore(int newScore) {
            _charScore.Value = newScore;
        }

        private void OnCharScoreChanged(int previousValue, int newValue) {
            UpdateScore(newValue);
        }

        private void UpdateScore(int newScore) {
            _uIPlayerCharScore.CharScoreChanged(newScore);
        }
    }
}