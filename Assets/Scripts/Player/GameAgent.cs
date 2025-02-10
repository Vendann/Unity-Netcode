using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Triwoinmag {
    public class GameAgent : NetworkBehaviour { // Делаем NetworkBehaviour, чтобы можно было получать ID клиента через GameAgent
        public enum Faction {
            Player,
            Allies,
            SeventhStar
        }

        public Faction ShipFaction;
    }
}