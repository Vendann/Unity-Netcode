using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Triwoinmag.ConnectionManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Triwoinmag {
    public class UIInMatchMenuEsc : MonoBehaviour { // Класс для меню

	    [SerializeField] private GameObject _inMatchMenuEscPanel; // Ссылка на понель меню, открывающееся при нажатии на Esc
		[SerializeField] private Button _exitButton; // Ссылка на кнопку выхода

	    [Header("Links")]
	    [SerializeField] private ConnectionManager _connectionManager; // Ссылка на ConnectionManager

	    private void Awake() {
			if(_connectionManager == null)
				_connectionManager = FindObjectOfType<ConnectionManager>();
		}

	    private void Start() {}

	    private void Update() {
			if (Input.GetKeyDown(KeyCode.Escape)) {
				SwitchPanel(); // При нажатии на Esc вызывается метод
			}
		}

		private void OnDestroy() {}

		public void ButtonExit() { // Метод для кнопки выхода, вызывает у ConnectionManager метод Shutdown, который все выключает
			_connectionManager.Shutdown();

			SwitchPanel();
		}

		private void SwitchPanel() { // Метод для переключения режима меню, если активно - выключить, или наоборот
			//_inMatchPanel.SetActive(!_inMatchPanel.activeSelf);
			if (_inMatchMenuEscPanel.activeSelf) {
				TurnOffPanel();
			}
			else {
				TurnOnPanel();
			}
		}
		private void TurnOnPanel() { // Метод для включения меню
			_inMatchMenuEscPanel.SetActive(true); // Включаем саму панель
			Cursor.lockState = CursorLockMode.None; // Разрешаем курсору двигаться
			Cursor.visible = true; // Делаем курсор видимым
		}
		private void TurnOffPanel() { // Метод для выключения меню, все противоположно предыдущему методу
			_inMatchMenuEscPanel.SetActive(false);
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}
}
