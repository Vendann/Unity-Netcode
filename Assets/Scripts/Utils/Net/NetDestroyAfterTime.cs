using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetDestroyAfterTime : NetworkBehaviour {
	[SerializeField] private float _seconds = 1f;

	private void Start() {
        if (IsServer)
            Destroy(gameObject, _seconds);
    }


}