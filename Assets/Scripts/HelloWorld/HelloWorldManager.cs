using Unity.Netcode;
using UnityEngine;

namespace HelloWorld {
    public class HelloWorldManager : MonoBehaviour {
        void OnGUI() { // это callback вроде Start или Awake, но специально для древнего интерфейса
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));

            // Проверяем, что мы еще не клиент и не сервер, т.е. не запускали режим хоста, сервера или клиента
            // Проверяем мы это с помощью синглтона (паттерн, гарантирующий существование только одного экземпляра какого-то класса)
            // В нашем случае класс NetworkManager, через синглтон мы обращаемся к единственному существующему экземпляру
            // Т.е. нам не нужно создавать специальную переменную, в которую потом в интерфейсе движка надо закидывать объект со сцены
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer) {
                StartButtons(); // Вызываем метод, рисующий кнопки и дающий им функционал
            }
            else { // Если мы уже в каком-то режиме, то вызываем эти методы
                StatusLabels(); // Этот пишет нам, какой у нас режим, а еще какой Transport выбран (это в конспекте первого видео есть)

                SubmitNewPosition(); // Этот нужен для изменения позиции игрока (вся дичь из предыдущего скрипта)
            }

            GUILayout.EndArea();
        }

        static void StartButtons() { // Собственно методы для каждой кнопки, даем кнопкам методы тоже через синглтон, как выше
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }

        static void StatusLabels() { // Пишем текст режима, по сути условие два if и один else в конце, но в другом виде написано
            var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            // Тут пишем текст Transport-а, достаем его опять через синглтон хрен пойми в какой части класса NetworkManager
            GUILayout.Label("Transport: " +
            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
        }

        static void SubmitNewPosition() { // Метод изменения позиции
            // Если мы в режиме сервера, то пишем текст "Запросить изменение позиции"
            if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request Position Change")) {
                // Если мы сервер и не клиент, то перебираем все id подключенных клиентов (где-то там в менеджере есть список этих id)
                if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient) {
                    // ulong - тип данных, 64-разрядное целое число без знака, короче очень длинное число, без знака, т.к. ID без минуса
                    foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
                        // Получаем объект игрока для каждого ID, т.е. у каждого клиента, и через этот компонент \/ вызываем метод Move
                        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<HelloWorldPlayer>().Move();
                }
                // Если же мы в режиме хоста или клиента, то просто получаем локальный объект игрока и у него вызываем метод Move
                // Т.к. мы не сервер, то нам не надо проделывать приколы для каждого ID, что сверху
                // Просто вызываем метод и в предыдущем классе уже все само синхронизируется и т.д. и т.п.
                else {
                    var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
                    var player = playerObject.GetComponent<HelloWorldPlayer>();
                    player.Move();
                }
            }
        }
    }
}
