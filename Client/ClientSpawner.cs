using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ClientSpawner : MonoBehaviour
{
    [SerializeField]
    private List<Transform> spawnPoint;
    [SerializeField]
    private ClientLogic client1;
    [SerializeField]
    private ClientLogic client2;
    [SerializeField]
    private ClientLogic client4;
    public bool stopSpawn = false;
    [SerializeField]
    private float spawnTime;    
    public float spawnDelay;

    private bool saveDelay = true;

    private void Awake()
    {
        spawnDelay = PlayerPrefs.GetFloat("spawnDelay", 10);
    }

    private void Start()
    {
        InvokeRepeating("SpawnObject", spawnTime, spawnDelay);
    }   

    public void SpawnObject()
    {
        if (Game.Instance.openTime < Game.Instance.currentGameTime & 
            Game.Instance.currentGameTime < Game.Instance.closeTime)
            Instantiate(client1, spawnPoint[Random.Range(0, spawnPoint.Count)].position, Quaternion.identity);

        if (stopSpawn == true)
        {
            CancelInvoke("SpawnObject");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {            
            PlayerPrefs.DeleteAll();
            saveDelay = false;
        }
    }

    private void OnApplicationQuit()
    {
        if (saveDelay)
            PlayerPrefs.SetFloat("spawnDelay", spawnDelay);
    }
}
