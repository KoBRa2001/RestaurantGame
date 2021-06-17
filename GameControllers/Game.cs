using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
	#region SIngleton:Game

	public static Game Instance;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	#endregion

	public int Coins;

	public GameObject table1P;

	public Vector3 currentPosition;
	public List<GameObject> tables = new List<GameObject>() { };
	public List<GameObject> toilets = new List<GameObject>() { };
	public List<GameObject> toiletKitchenObjects = new List<GameObject>() { };

	public List<Vector3> tables1 = new List<Vector3>() { };
	public List<Vector3> tables2 = new List<Vector3>() { };
	public List<Vector3> tables4 = new List<Vector3>() { };

	public List<Vector3> falseTables = new List<Vector3>() { };
	public List<Vector3> falseToilets = new List<Vector3>() { };


	public List<GameObject> toClean = new List<GameObject>() { };

	//Нехай ігровий день триває 15хв=900сек. 24год=86400сек.
	public float currentGameTime;
	public float openTime;
	public float closeTime;

	public List<Vector3> waitForWaiter = new List<Vector3>() { };
	
	public float avaragePrice;	
	public int salary;
	public int promote;
	public int promotePercent;


	public int maxXP;
	public int curXP;

	void Start()
	{
		Debug.Log("Start game");
		curXP = PlayerPrefs.GetInt("currentXP", 0);
		promotePercent = PlayerPrefs.GetInt("Percent", 0);
		tables = PlayerPrefsExtra.GetList("listTables", new List<GameObject>());
		tables1 = PlayerPrefsExtra.GetList("listTables1", new List<Vector3>());
		toilets = PlayerPrefsExtra.GetList("listToilets", new List<GameObject>());		
		Coins = PlayerPrefs.GetInt("Coins", 1500);
		avaragePrice = PlayerPrefs.GetFloat("AvPrice", 0);
		currentGameTime = PlayerPrefs.GetFloat("GameTime", 0);
	}


	public void UseCoins(int amount)
	{
		Coins -= amount;
		PlayerPrefs.SetInt("Coins", Coins);
	}

	public bool HasEnoughCoins(int amount)
	{
		return (Coins >= amount);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.K))
		{
			Debug.Log("Delete");
			PlayerPrefs.DeleteAll();			
		}
	}
}
