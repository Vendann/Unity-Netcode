using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Triwoinmag.ConnectionManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Triwoinmag {
    public class UIInMatchPlayerHUD : MonoBehaviour { // Класс для интерфейса внутри игры, у нас это очки игрока

	    [SerializeField] private GameObject _inMatchPlayerHUDPanel; // Ссылка на панель с HUD

	    [Header("Links")]
	    [SerializeField] private ConnectionManager _connectionManager; // Ссылка на ConnectionManager


	    private void Awake() {
			if(_connectionManager == null)
				_connectionManager = FindObjectOfType<ConnectionManager>();
		}

	    private void Start() {
		    _connectionManager.MatchStarted += TurnOnPanel; // Подписываем метод, включающий панель с очками, на событие MatchStarted у ConnectionManager
															// При успешном подключении к серверу у клиента срабатывает событие MatchStarted
															// И в результате здесь мы включаем панель с кол-вом очков
	    }
	    // private void Update() {}

		private void OnDestroy() {
		    _connectionManager.MatchStarted -= TurnOnPanel; // Отписываем метод от события при уничтожении объекта, на котором висит скрипт
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

		// Следующие 2 метода аналогичны тем, что в скрипте для меню, но здесь мы не трогаем курсор, просто работаем с панелью очков
		private void TurnOnPanel() {
			_inMatchPlayerHUDPanel.SetActive(true);
		}
		private void TurnOffPanel() {
			_inMatchPlayerHUDPanel.SetActive(false);
		}
	}
}
