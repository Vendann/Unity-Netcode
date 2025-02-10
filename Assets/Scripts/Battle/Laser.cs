using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Triwoinmag {
    [RequireComponent(typeof(LineRenderer))]
    public class Laser : NetworkBehaviour, IWeapon {
	    [field: SerializeField] public float DamageAmount { get; private set; } = 50.0f;
	    [field: SerializeField] public float MaxDistance { get; private set; } = 100.0f;

	    private Coroutine _coroutineFiring = null;
        private WaitForSeconds _waitForFiring;
        [SerializeField] private float _waitTimeFiring = 0.1f;

        // Technical
        [SerializeField] private LayerMask _layerMask = new LayerMask();

        [field: SerializeField] public bool CanFire { get; private set; }


        [Header("FX settings")]
        [SerializeField] private float _lineRenAnimSpeed = 10; // Speed for animating UV
        private float _lineRenAnimDeltaTime;

        [Header("Links")]
        [SerializeField] private LineRenderer _lineRenderer;
        //[SerializeField] private ParticleSystem _fXMuzzle;
        [SerializeField] private ParticleSystem _fXImpact;
        private CharacterWeapons _characterWeapons;

        public List<IDamageable> TargetsHit = new List<IDamageable>();
        [SerializeField] private bool _rayImpact;

        [Header("Debugging")]
        public bool Debugging;
        [SerializeField] private Vector3 _targetPosition;
        [SerializeField] private GameObject _testPrefab;

        private void Awake() {
            if (_characterWeapons == null)
                _characterWeapons = GetComponentInParent<CharacterWeapons>();
            if (_lineRenderer == null)
                _lineRenderer = GetComponent<LineRenderer>();
        }

        private void Start() {
            _waitForFiring = new WaitForSeconds(_waitTimeFiring);

            _lineRenderer.enabled = false;
            CanFire = true;
        }
        private void Update() {
	        if (!_lineRenderer.enabled) { return; }

	        _lineRenAnimDeltaTime += Time.deltaTime;
	        _lineRenderer.SetPosition(0, transform.position);
	        _fXImpact.transform.position = _targetPosition;

			if (_lineRenAnimDeltaTime > 1.0f)
				_lineRenAnimDeltaTime = 0f;

	        _lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(_lineRenAnimDeltaTime * _lineRenAnimSpeed, 0f)); //_BaseMap
        }

        public void Attack (Vector3 targetPosition) { // В CharacterWeapons вызывается этот метод
            if (IsOwner) { // Проверяем, что метод вызывает владелец, если да, то идет логика
                // Мы уже проверем это в CharacterWeapons, эта проверка нужна на случай, если стрелять будет не игрок, а другой объект, например турель
                AttackServerRpc(targetPosition); // Вызываем метод с логикой стрельбы
                VisualizeFiring(targetPosition); // Вызываем метод визуализации выстрела, это для того, чтобы игрок видел свой выстрел сразу,
                                                                                                                  // без задержки от сервера
            }
        }

        [ServerRpc] // Метод выстрела, выполняется на сервере
        public void AttackServerRpc(Vector3 targetPosition) {
	        if (!CanFire) { return; }
            VisualizeFiringClientRpc(targetPosition); // Здесь мы вызываем метод визуализации выстрела, пояснения в самом методе ниже

	        RaycastHit hitInfo;
            var direction = (targetPosition - transform.position).normalized;
            if (Physics.Raycast(transform.position, direction, out hitInfo, MaxDistance, _layerMask)) {
	            if (Debugging) {
		            Debug.Log($"Attack. Physics.Raycast == true. hitInfo.transform.name: {hitInfo.transform.name} hitInfo.point: {hitInfo.point}");
                    //Instantiate(_testPrefab, hitInfo.point, Quaternion.identity);
                }

                var targetHit = hitInfo.transform;
                if(Debugging)
					Debug.Log($"Attack. targetHit != null targetHitName: {targetHit.name} targetHitPos: {targetHit.position}");
                var damageableHit = targetHit.GetComponent<IDamageable>();
                if (damageableHit != null) {
                    TargetsHit.Add(damageableHit);
                    Damage(DamageAmount, targetHit.position, _characterWeapons.Core.Agent);
                }
                return;
            }
        }
        
        public void Damage(float damageAmount, Vector3 targetHitPosition, GameAgent sender) {
            foreach (var targetHit in TargetsHit) {
                targetHit.ReceiveDamage(damageAmount, targetHitPosition, sender);
            }
            TargetsHit.Clear();
        }

        [ClientRpc] // Метод, вызывающий логику визуализации выстрела, выполняется у клиентов
        public void VisualizeFiringClientRpc(Vector3 targetPosition) {
            if (!IsOwner) // Проверяем, что метод вызывает НЕ владелец, для того, чтобы визуализировать выстрел у всех остальных
                // Т.к. у клиента-владельца выстрел визуализировался сразу при нажатии, поэтому разделяем логику, у стреляющего все видно сразу
                                                                         // а у остальных с задержкой, пока сервер отправит результат обработки
                VisualizeFiring(targetPosition); // Собствевнно вызов визуализации выстрела
        }

        public void VisualizeFiring(Vector3 targetPosition) { // Сам метод визуализации выстрела, тут просто рисуется луч, просто знай, что он тут
            if(Debugging)
				Debug.Log($"VisualizeFiring. transform.position: {transform.position}, targetPosition: {targetPosition}");
	        _targetPosition = targetPosition;

            _lineRenderer.enabled = true;
            _lineRenderer.SetPosition(0, transform.position);
            _lineRenderer.SetPosition(1, targetPosition);

            // Adjust impact effect position
            if (_rayImpact) {
	            _fXImpact.transform.position = targetPosition; // - transform.forward * 0.2f
	            _fXImpact.gameObject.SetActive(true);
                //_particleSysImpact.Play(true);
            }
            // Adjust muzzle position
            //if (_fXMuzzle) {
            // _fXMuzzle.transform.position = transform.position + transform.forward * 0.05f;
            // _fXMuzzle.gameObject.SetActive(true);
            //}


            CanFire = false;

            _coroutineFiring = StartCoroutine(FiringCor());
        }

        private IEnumerator FiringCor() {
            yield return _waitForFiring;

            _fXImpact.gameObject.SetActive(false);
            //_fXMuzzle.gameObject.SetActive(false);
            CanFire = true;
            _lineRenderer.enabled = false;
        }
    }
}