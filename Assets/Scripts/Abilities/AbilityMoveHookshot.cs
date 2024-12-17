using System.Collections;
using System.Collections.Generic;
using Triwoinmag;
using Unity.Netcode;
using UnityEngine;

public class AbilityMoveHookshot : NetworkBehaviour
{
    [field: SerializeField] public bool CanFire { get; private set; }

    public float MaxDistanceToTarget = 100f;

    [SerializeField] private float _minHookshotSpeed = 3f;
    [SerializeField] private float _maxHookshotSpeed = 30f;
    [SerializeField] private float _hookshotSpeed = 5f;

    [SerializeField] private LayerMask _layerMask = new LayerMask();
    [SerializeField] private GameObject _hookShotObject;

    [Header("Debugging")]
    public bool Debugging;

    [Header("Links")]
    [SerializeField] private CharacterMovement _charMovement;
    [SerializeField] private LineRenderer _lineRenCircle;
    [SerializeField] private LineRenderer _lineRenShot;

    private void Start() {
        // Подписываем методы включения и отключения визуализации выстрела крюком на делегат
        // Чтобы не крюк вызывал визуализацию, а делегат в CharacterMovement
        _charMovement.StartExecutingHookshot += VisualizeFiringHookshot; // На делегат старта полета подписываем включение визуализации
        _charMovement.StopExecutingHookshot += VisualizeStopExecutingHookshot; // Не делегат остановки полета соответственно подписываем отключение
    }

    public override void OnDestroy() {
        // Переодпределяем метод, срабатывающий при удалении объекта со сцены, сначала вызываем базовый метод, затем пишем доп. логику ниже
        base.OnDestroy();

        // Отписываемся от делегатов, когда объект уничтожается
        _charMovement.StartExecutingHookshot -= VisualizeFiringHookshot;
        _charMovement.StopExecutingHookshot -= VisualizeStopExecutingHookshot;
    }

    private void Update() {
        if (IsOwner) {
            if (Input.GetKeyDown(KeyCode.F) && CanFire) { // При нажатии на кнопку крюка-кошки
                _charMovement.SwitchHookshot(); // Вызываем у CharacterMovement метод, меняющий режим движения на нужный
                                                // А не как было до этого, что режим меняла сама крюк-кошка
            }

            _lineRenShot.SetPosition(0, transform.position);
            
            // _lineRenShot.SetPosition(1, _hookshotTargetPos); // TODO: for attaching to Chars
        }
    }

    public Vector3 CheckFireHookshot() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));

        if (Physics.Raycast(ray, out hit, MaxDistanceToTarget, _layerMask)) {
            if (Debugging) {
                Debug.Log($"FireWeapons. Object: {hit.transform.gameObject.name} ray.origin: {ray.origin}, hit.point: {hit.point}");
                //Debug.DrawRay(ray.origin, Camera.main.transform.TransformDirection(Vector3.forward) * hit.distance, Color.red, 3.0f);
            }
            Instantiate(_hookShotObject, hit.point, Quaternion.identity);
            return hit.point;
        }
        else {
            return Vector3.zero;
        }
    }

    public Vector3 ExecuteHookshot(Vector3 hookshotTargetPos) {
        if (Vector3.Distance(hookshotTargetPos, transform.position) < 1.5f) {

            VisualizeStopExecutingHookshot();
            return Vector3.zero;
        }

        Vector3 hookshotDir = (hookshotTargetPos - transform.position).normalized;

        float effectiveSpeed = Mathf.Clamp(Vector3.Distance(hookshotTargetPos, transform.position) * _hookshotSpeed,
            _minHookshotSpeed, _maxHookshotSpeed) * Time.deltaTime;

        return hookshotDir * effectiveSpeed;
    }

    private void VisualizeFiringHookshot(Vector3 hitPos) {
        _lineRenCircle.enabled = true;
        _lineRenShot.enabled = true;
        _lineRenShot.SetPosition(0, transform.position);
        _lineRenShot.SetPosition(1, hitPos);
    }

    private void VisualizeStopExecutingHookshot() {
        _lineRenCircle.enabled = false;
        _lineRenShot.enabled = false;
    }
}
