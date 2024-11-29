using Unity.Netcode;
using UnityEngine;

namespace HelloWorld {
    public class HelloWorldManager : MonoBehaviour {
        void OnGUI() { // ��� callback ����� Start ��� Awake, �� ���������� ��� �������� ����������
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));

            // ���������, ��� �� ��� �� ������ � �� ������, �.�. �� ��������� ����� �����, ������� ��� �������
            // ��������� �� ��� � ������� ��������� (�������, ������������� ������������� ������ ������ ���������� ������-�� ������)
            // � ����� ������ ����� NetworkManager, ����� �������� �� ���������� � ������������� ������������� ����������
            // �.�. ��� �� ����� ��������� ����������� ����������, � ������� ����� � ���������� ������ ���� ���������� ������ �� �����
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer) {
                StartButtons(); // �������� �����, �������� ������ � ������ �� ����������
            }
            else { // ���� �� ��� � �����-�� ������, �� �������� ��� ������
                StatusLabels(); // ���� ����� ���, ����� � ��� �����, � ��� ����� Transport ������ (��� � ��������� ������� ����� ����)

                SubmitNewPosition(); // ���� ����� ��� ��������� ������� ������ (��� ���� �� ����������� �������)
            }

            GUILayout.EndArea();
        }

        static void StartButtons() { // ���������� ������ ��� ������ ������, ���� ������� ������ ���� ����� ��������, ��� ����
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }

        static void StatusLabels() { // ����� ����� ������, �� ���� ������� ��� if � ���� else � �����, �� � ������ ���� ��������
            var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            // ��� ����� ����� Transport-�, ������� ��� ����� ����� �������� ���� ����� � ����� ����� ������ NetworkManager
            GUILayout.Label("Transport: " +
            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
        }

        static void SubmitNewPosition() { // ����� ��������� �������
            // ���� �� � ������ �������, �� ����� ����� "��������� ��������� �������"
            if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request Position Change")) {
                // ���� �� ������ � �� ������, �� ���������� ��� id ������������ �������� (���-�� ��� � ��������� ���� ������ ���� id)
                if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient) {
                    // ulong - ��� ������, 64-��������� ����� ����� ��� �����, ������ ����� ������� �����, ��� �����, �.�. ID ��� ������
                    foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
                        // �������� ������ ������ ��� ������� ID, �.�. � ������� �������, � ����� ���� ��������� \/ �������� ����� Move
                        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<HelloWorldPlayer>().Move();
                }
                // ���� �� �� � ������ ����� ��� �������, �� ������ �������� ��������� ������ ������ � � ���� �������� ����� Move
                // �.�. �� �� ������, �� ��� �� ���� ����������� ������� ��� ������� ID, ��� ������
                // ������ �������� ����� � � ���������� ������ ��� ��� ���� ���������������� � �.�. � �.�.
                else {
                    var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
                    var player = playerObject.GetComponent<HelloWorldPlayer>();
                    player.Move();
                }
            }
        }
    }
}
