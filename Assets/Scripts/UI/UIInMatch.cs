using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Triwoinmag.ConnectionManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Triwoinmag {
    public class UIInMatch : MonoBehaviour {

	    [SerializeField] private GameObject _inMatchPanel;
		[SerializeField] private Button _exitButton;



	    [Header("Links")]
	    [SerializeField] private ConnectionManager _connectionManager;


	    private void Awake() {
			if(_connectionManager == null)
				_connectionManager = FindObjectOfType<ConnectionManager>();
		}

	    private void Start() {
		    _connectionManager.MatchStarted += TurnOnPanel;
	    }
	    private void Update() {
			if (Input.GetKeyDown(KeyCode.Escape)) {
				SwitchPanel();
			}
		}

		private void OnDestroy() {
		    _connectionManager.MatchStarted -= TurnOnPanel;
	    }


		public void ButtonExit() {
			_connectionManager.Shutdown();

			SwitchPanel();
		}

		private void SwitchPanel() {
			//_inMatchPanel.SetActive(!_inMatchPanel.activeSelf);
			if (_inMatchPanel.activeSelf) {
				TurnOffPanel();
			}
			else {
				TurnOnPanel();
			}
		}
		private void TurnOnPanel() {
			_inMatchPanel.SetActive(true);
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		private void TurnOffPanel() {
			_inMatchPanel.SetActive(false);
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}
}
