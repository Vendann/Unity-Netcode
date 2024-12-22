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
    [SerializeField] private Vector3 _momentumAfterHookshotAborted;
    [SerializeField] private Vector3 _momentumAfterHookshotFinished;

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

            var hookshotDir = (hit.point - transform.position).normalized;
            _momentumAfterHookshotFinished = hookshotDir * 1.5f;
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

            VisualizeStopExecutingHookshotServerRpc();
            return Vector3.zero;
        }

        Vector3 hookshotDir = (hookshotTargetPos - transform.position).normalized;

        // Momentum during movement
        hookshotDir.y += _momentumAfterHookshotFinished.y / 2; // уг

        float effectiveSpeed = Mathf.Clamp(Vector3.Distance(hookshotTargetPos, transform.position) * _hookshotSpeed,
            _minHookshotSpeed, _maxHookshotSpeed) * Time.deltaTime;

        return hookshotDir * effectiveSpeed;
    }

    public Vector3 CalculateMomentumAfterHookshot(bool aborted) {
        if (aborted) {
            _momentumAfterHookshotAborted = Camera.main.ScreenPointToRay(
                new Vector2(Screen.width * 0.5f, Screen.height * 0.5f)).direction.normalized * 2f;
            return _momentumAfterHookshotAborted;
        }
        else {
            return _momentumAfterHookshotFinished;
        }
    }

    [ServerRpc]
    private void VisualizeFiringHookshotServerRpc(Vector3 hitPos) {
        var hookshotNetObj = Instantiate(_hookShotObject, hitPos, Quaternion.identity).GetComponent<NetworkObject>();
        hookshotNetObj.Spawn();

        VisualizeFiringHookshotClientRpc(hitPos);
    }

    [ClientRpc]
    private void VisualizeFiringHookshotClientRpc(Vector3 hitPos) {
        _lineRenCircle.enabled = true;
        _lineRenShot.enabled = true;
        _lineRenShot.SetPosition(0, transform.position);
        _lineRenShot.SetPosition(1, hitPos);
    }

    [ServerRpc]
    private void VisualizeStopExecutingHookshotServerRpc() {
        VisualizeStopExecutingHookshotClientRpc();
    }

    [ClientRpc]
    private void VisualizeStopExecutingHookshotClientRpc() {
        _lineRenCircle.enabled = false;
        _lineRenShot.enabled = false;
    }

    [ServerRpc]
    private void VisualizeExecutingHookshotServerRpc() {
        VisualizeExecutingHookshotClientRpc();
    }

    [ClientRpc]
    private void VisualizeExecutingHookshotClientRpc() {
        _lineRenShot.SetPosition(0, transform.position);
        // _lineRenShot.SetPosition(1, _hookshotTargetPos);
    }
}
