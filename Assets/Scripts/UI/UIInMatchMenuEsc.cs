using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Triwoinmag.ConnectionManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Triwoinmag {
    public class UIInMatchMenuEsc : MonoBehaviour {

	    [SerializeField] private GameObject _inMatchMenuEscPanel;
		[SerializeField] private Button _exitButton;

	    [Header("Links")]
	    [SerializeField] private ConnectionManager _connectionManager;

	    private void Awake() {
			if(_connectionManager == null)
				_connectionManager = FindObjectOfType<ConnectionManager>();
		}

	    private void Start() {}

	    private void Update() {
			if (Input.GetKeyDown(KeyCode.Escape)) {
				SwitchPanel();
			}
		}

		private void OnDestroy() {}

		public void ButtonExit() {
			_connectionManager.Shutdown();

			SwitchPanel();
		}

		private void SwitchPanel() {
			//_inMatchPanel.SetActive(!_inMatchPanel.activeSelf);
			if (_inMatchMenuEscPanel.activeSelf) {
				TurnOffPanel();
			}
			else {
				TurnOnPanel();
			}
		}
		private void TurnOnPanel() {
			_inMatchMenuEscPanel.SetActive(true);
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		private void TurnOffPanel() {
			_inMatchMenuEscPanel.SetActive(false);
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}
}
