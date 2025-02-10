using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Triwoinmag.ConnectionManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Triwoinmag {
    public class UIInMatchPlayerHUD : MonoBehaviour { // ����� ��� ���������� ������ ����, � ��� ��� ���� ������

	    [SerializeField] private GameObject _inMatchPlayerHUDPanel; // ������ �� ������ � HUD

	    [Header("Links")]
	    [SerializeField] private ConnectionManager _connectionManager; // ������ �� ConnectionManager


	    private void Awake() {
			if(_connectionManager == null)
				_connectionManager = FindObjectOfType<ConnectionManager>();
		}

	    private void Start() {
		    _connectionManager.MatchStarted += TurnOnPanel; // ����������� �����, ���������� ������ � ������, �� ������� MatchStarted � ConnectionManager
															// ��� �������� ����������� � ������� � ������� ����������� ������� MatchStarted
															// � � ���������� ����� �� �������� ������ � ���-��� �����
	    }
	    // private void Update() {}

		private void OnDestroy() {
		    _connectionManager.MatchStarted -= TurnOnPanel; // ���������� ����� �� ������� ��� ����������� �������, �� ������� ����� ������
	    }

		private void SwitchPanel() {
			//_inMatchPanel.SetActive(!_inMatchPanel.activeSelf);
			if (_inMatchPlayerHUDPanel.activeSelf) {
				TurnOffPanel();
			}
			else {
				TurnOnPanel();
			}
		}

		// ��������� 2 ������ ���������� ���, ��� � ������� ��� ����, �� ����� �� �� ������� ������, ������ �������� � ������� �����
		private void TurnOnPanel() {
			_inMatchPlayerHUDPanel.SetActive(true);
		}
		private void TurnOffPanel() {
			_inMatchPlayerHUDPanel.SetActive(false);
		}
	}
}
