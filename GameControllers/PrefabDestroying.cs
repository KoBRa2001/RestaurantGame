using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabDestroying : MonoBehaviour
{
    public void DestroyPrefab()
    {
        Destroy(gameObject);
        Debug.Log("Destroyyyy");
    }
}
