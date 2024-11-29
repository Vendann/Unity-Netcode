using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkTransformTest : NetworkBehaviour {
    void Update() {
        if (IsServer) { // ѕровер€ем, что мы сервер, если да, то мы можем вертеть игрока как хотим, и это синхронизируетс€ у всех
            float theta = Time.frameCount / 10.0f;
            transform.position = new Vector3((float)Math.Cos(theta), 0.0f, (float)Math.Sin(theta));
        }
    }
}