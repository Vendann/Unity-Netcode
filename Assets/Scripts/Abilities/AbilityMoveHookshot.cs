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

    // Momentum
    [SerializeField] private Vector3 _momentumAfterHookshotAborted; // Вектор направления, когда полет на крюк-кошке прерван
    [SerializeField] private Vector3 _momentumAfterHookshotFinished; // Вектор направления, когда полет завершен нормально

    [Header("Links")]
    [SerializeField] private CharacterMovement _charMovement;
    [SerializeField] private LineRenderer _lineRenCircle;
    [SerializeField] private LineRenderer _lineRenShot;
    
    [Header("Debugging")]
    public bool Debugging;
    
    private void Start() {
        _charMovement.StartExecutingHookshot += VisualizeFiringHookshotServerRpc;
        _charMovement.StopExecutingHookshot += VisualizeStopExecutingHookshotServerRpc;
    }

    public override void OnDestroy() {
        base.OnDestroy();

        _charMovement.StartExecutingHookshot -= VisualizeFiringHookshotServerRpc;
        _charMovement.StopExecutingHookshot -= VisualizeStopExecutingHookshotServerRpc;
    }

    private void Update() {
        if (IsOwner) {
            if (Input.GetKeyDown(KeyCode.F) && CanFire) {
                _charMovement.SwitchHookshot();
            }
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

            var hookshotDir = (hit.point - transform.position).normalized; // От точки попадания отнимаем наши координаты, получаем вектор направления
            _momentumAfterHookshotFinished = hookshotDir * 1.5f; // Второй переменной присваиваем результат выше и умножаем
            // Ограничиваем у вектора ось Y, к которой в скобках прибавили 0.05, и присваиваем результат самой себе
            _momentumAfterHookshotFinished.y = Mathf.Clamp(_momentumAfterHookshotFinished.y + 0.05f, 0.05f, 0.5f);

            return hit.point;
        }
        else {
            return Vector3.zero;
        }
    }

    public Vector3 ExecuteHookshot(Vector3 hookshotTargetPos) {
        VisualizeExecutingHookshotServerRpc();

        if (Vector3.Distance(hookshotTargetPos, transform.position) < 1.5f) {

            VisualizeStopExecutingHookshotServerRpc(); // Теперь здесь вызываем визуализацию сервера и дальше по цепочке
            return Vector3.zero;
        }

        Vector3 hookshotDir = (hookshotTargetPos - transform.position).normalized;

        // Momentum during movement
        hookshotDir.y += _momentumAfterHookshotFinished.y / 2; // ХЗ

        float effectiveSpeed = Mathf.Clamp(Vector3.Distance(hookshotTargetPos, transform.position) * _hookshotSpeed,
            _minHookshotSpeed, _maxHookshotSpeed) * Time.deltaTime;

        return hookshotDir * effectiveSpeed;
    }

    // Метод для вычисления вектора направления
    public Vector3 CalculateMomentumAfterHookshot(bool aborted) {
        if (aborted) { // Если полет на крюк-кошке был прерван и переменная равна true (aborted - преравнный, все логично)
            // То находим направление взгляда камеры (типа игрок туда смотрит в этот момент)
            // Нормализуем полученный вектор и умножаем его на 2, результат всего этого присваиваем переменной слева
            _momentumAfterHookshotAborted = Camera.main.ScreenPointToRay(
                new Vector2(Screen.width * 0.5f, Screen.height * 0.5f)).direction.normalized * 2f;
            return _momentumAfterHookshotAborted; // Возвращаем переменную
        }
        else {
            return _momentumAfterHookshotFinished; // Иначе возвращаем другую переменню, которая будет вычисляться в методе CheckFireHookshot
        }
    }

    // Сделали метод, выполняющийся на сервере, т.е. сервер получает всю информацию и вызывает у клиентов метод для визуализации
    [ServerRpc]
    private void VisualizeFiringHookshotServerRpc(Vector3 hitPos) {
        // Перенесли создание шарика в точке попадания сюда, при этом это будет сетевой объект
        // Т.е. переменной слева мы приваиваем результат метода GetComponent, который ищет NetworkObject на объекте, созданном с помощью Instantiate
        var hookshotNetObj = Instantiate(_hookShotObject, hitPos, Quaternion.identity).GetComponent<NetworkObject>();
        hookshotNetObj.Spawn(); // Метод у класса NetworkObject, просто спавнит объект на всех машинах

        VisualizeFiringHookshotClientRpc(hitPos);
    }

    // Визуализация у клиентов
    [ClientRpc]
    private void VisualizeFiringHookshotClientRpc(Vector3 hitPos) {
        _lineRenCircle.enabled = true;
        _lineRenShot.enabled = true;
        _lineRenShot.SetPosition(0, transform.position);
        _lineRenShot.SetPosition(1, hitPos);
    }

    // Тоже выполняется на сервере, вызывает метод отключения визуализации
    [ServerRpc]
    private void VisualizeStopExecutingHookshotServerRpc() {
        VisualizeStopExecutingHookshotClientRpc();
    }

    // Выключение визуализации у клиентов
    [ClientRpc]
    private void VisualizeStopExecutingHookshotClientRpc() {
        _lineRenCircle.enabled = false;
        _lineRenShot.enabled = false;
    }

    // Метод, вызывающий обновление координат луча, работает на сервере
    [ServerRpc]
    private void VisualizeExecutingHookshotServerRpc() {
        VisualizeExecutingHookshotClientRpc();
    }

    // Код из Update, двигающий координаты луча в соответствии с игроком, вынесли в отдельный метод для клиента
    [ClientRpc]
    private void VisualizeExecutingHookshotClientRpc() {
        _lineRenShot.SetPosition(0, transform.position);
        // _lineRenShot.SetPosition(1, _hookshotTargetPos);
    }
}
