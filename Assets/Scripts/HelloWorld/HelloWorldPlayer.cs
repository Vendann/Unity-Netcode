using Unity.Netcode;
using UnityEngine;


namespace HelloWorld {
    public class HelloWorldPlayer : NetworkBehaviour {
        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

        public override void OnNetworkSpawn() {
            if (IsOwner) { // Если клиент - владелец объекта, т.е. префаба игрока, то вызываем Move
                Move();
            }
        }

        public void Move() { // В Move делаем запрос на сервер дл выполнения логики
            SubmitPositionRequestRpc();
        }

        [Rpc(SendTo.Server)]
        void SubmitPositionRequestRpc(RpcParams rpcParams = default) {
            var randomPosition = GetRandomPositionOnPlane(); // создаем переменную, которой присваивается результат метода справа
                                                                // Он душил тебя на знаке равно, это оператор присванивания
                                                                // Присваиваем значение переменной, не наоборот
            transform.position = randomPosition; // Обновляем позицию для своего префаба игрока, на котором висит этот скрипт
            Position.Value = randomPosition; // Обновляем Value для синхронизации между клиентами
        }

        static Vector3 GetRandomPositionOnPlane() { // Метод для создания вектора с 3 рандомными значениями, идем дальше
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }

        void Update() {
            // Обновляем позицию префаба игрока у всех клиентов
            //if (!IsOwner) // Здесь он показывал вариант обновления позиции в том случае, если это не твой префаб
            // Т.е. у владельца префаба не перезаписывался position, и он мог менять его другими методами
            // А синхронизация все равно работала
                transform.position = Position.Value;
        }
    }
}