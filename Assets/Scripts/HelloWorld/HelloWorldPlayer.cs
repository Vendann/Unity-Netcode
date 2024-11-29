using Unity.Netcode;
using UnityEngine;


namespace HelloWorld {
    public class HelloWorldPlayer : NetworkBehaviour {
        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

        public override void OnNetworkSpawn() {
            if (IsOwner) { // ���� ������ - �������� �������, �.�. ������� ������, �� �������� Move
                Move();
            }
        }

        public void Move() { // � Move ������ ������ �� ������ �� ���������� ������
            SubmitPositionRequestRpc();
        }

        [Rpc(SendTo.Server)]
        void SubmitPositionRequestRpc(RpcParams rpcParams = default) {
            var randomPosition = GetRandomPositionOnPlane(); // ������� ����������, ������� ������������� ��������� ������ ������
                                                                // �� ����� ���� �� ����� �����, ��� �������� �������������
                                                                // ����������� �������� ����������, �� ��������
            transform.position = randomPosition; // ��������� ������� ��� ������ ������� ������, �� ������� ����� ���� ������
            Position.Value = randomPosition; // ��������� Value ��� ������������� ����� ���������
        }

        static Vector3 GetRandomPositionOnPlane() { // ����� ��� �������� ������� � 3 ���������� ����������, ���� ������
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }

        void Update() {
            // ��������� ������� ������� ������ � ���� ��������
            //if (!IsOwner) // ����� �� ��������� ������� ���������� ������� � ��� ������, ���� ��� �� ���� ������
            // �.�. � ��������� ������� �� ��������������� position, � �� ��� ������ ��� ������� ��������
            // � ������������� ��� ����� ��������
                transform.position = Position.Value;
        }
    }
}