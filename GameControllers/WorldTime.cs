using UnityEngine;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class WorldTime : MonoBehaviour
{
	#region Singleton class: WorldTime

	public static WorldTime Instance;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else
		{
			Destroy(this.gameObject);
		}
	}

	#endregion

	[SerializeField]
	private GameObject welcomePanel;
	[SerializeField]
	private Text timeText;
	[SerializeField]
	private Text incomingText;
	[SerializeField]
	private Text uncomingText;
	private bool resetTime = false;
	[SerializeField]
	private Button closeBtn;

	private DateTime _currentDateTime = DateTime.Now;
	private DateTime quitTime;
	public float avaragePrice;
	public List<Staff> staff;

	private TimeSpan timeSpan;

	void Start()
	{
		closeBtn.interactable = false;
		timeText.text = "Loading...";
		string quitTimeString = PlayerPrefs.GetString(gameObject.name + "QuitTime", null);
		if (quitTimeString == "")
		{						
			welcomePanel.gameObject.SetActive(false);			
		}
		else
		{							
			quitTime = DateTime.Parse(quitTimeString);			
			StartCoroutine(CalculateTime());		
		}		
	}

	public IEnumerator CalculateTime()
	{		
		yield return new WaitForSeconds(1f);
		avaragePrice = PlayerPrefs.GetFloat("AvPrice", 0);		
		timeSpan = DateTime.Now - quitTime;
		timeText.text = timeSpan.Hours + ":" + timeSpan.Minutes + ":" + timeSpan.Seconds;
		if (timeSpan.Hours >= 1)
		{
			avaragePrice *= 60f;		
			float r = Random.Range((Game.Instance.promotePercent / 100f + 1f) - 0.5f, (Game.Instance.promotePercent / 100f + 1f) + 0.5f);
			Debug.Log("Random " + r);
			Game.Instance.Coins += Mathf.RoundToInt(avaragePrice * r);
			incomingText.text = "+" + Mathf.RoundToInt(avaragePrice * r);

			if (Game.Instance.HasEnoughCoins(Game.Instance.salary))
			{
				Game.Instance.Coins -= Game.Instance.salary;				
			}
			else
			{
				StartCoroutine(Fire(1)); //(1) more than hour		
			}
			
			uncomingText.text = "-" + Game.Instance.salary;
			PlayerPrefs.SetInt("Coins", Game.Instance.Coins);
		}
		else
		{
			avaragePrice *= (float)timeSpan.Minutes;
			float r = Random.Range((Game.Instance.promotePercent / 100f + 1f) - 0.5f, (Game.Instance.promotePercent / 100f + 1f) + 0.5f);
			Debug.Log("Random " + r);
			Game.Instance.Coins += Mathf.RoundToInt(avaragePrice * r);
			incomingText.text = "+" + Mathf.RoundToInt(avaragePrice * r);

			if (Game.Instance.HasEnoughCoins((Game.Instance.salary / 60) * timeSpan.Minutes))
			{
				Game.Instance.Coins -= (Game.Instance.salary / 60) * timeSpan.Minutes;				
			}
			else
			{
				StartCoroutine(Fire(0)); //(0) less than hour				
			}
			
			uncomingText.text = "-" + (Game.Instance.salary / 60) * timeSpan.Minutes;
			PlayerPrefs.SetInt("Coins", Game.Instance.Coins);
		}
		closeBtn.interactable = true;
	}

	private void OnApplicationQuit()
	{
		if (quitTime < DateTime.Now)
		{
			//Debug.Log("No cheat");
			if (!resetTime)
				PlayerPrefs.SetString(gameObject.name + "QuitTime", DateTime.Now.ToString());
			//Debug.Log("SetTime");
		}
		if (quitTime > DateTime.Now)
		{
			//Debug.Log("Cheat");
		}
		if (!resetTime)
		{
			PlayerPrefs.SetFloat("GameTime", Game.Instance.currentGameTime);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.K))
		{			
			PlayerPrefs.DeleteAll();
			resetTime = true;
		}
		if (welcomePanel.active == true)
		{
			ZoomPan.zoomEnable = false;
		}		
	}

	public void Close()
	{
		ZoomPan.zoomEnable = true;
		welcomePanel.gameObject.SetActive(false);
	}

	public IEnumerator Fire(int hour) //i=1 more than hour, i=0 less than hour
	{
		Debug.Log("Salary work");
		for (int index = 0; index < staff.Count; index++)
		{
			for (int i = 0; i < staff[index].staffInfoList.Count; i++)
			{
				if (staff[index].isHiredList[i])
				{
					switch (hour) {
						case 0:
							if (Game.Instance.HasEnoughCoins((staff[index].staffInfoList[i].price / 60) 
								* timeSpan.Minutes))
							{		
								Game.Instance.Coins -= (staff[index].staffInfoList[i].price / 60)
								* timeSpan.Minutes;
								PlayerPrefs.SetInt("Coins", Game.Instance.Coins);
								//Debug.Log("Success payment");
							}
							else
							{							
								staff[index].OnStaffHireBtnClicked(i);
								yield return new WaitForSeconds(2f);							
							}
							break;
						case 1:
							if (Game.Instance.HasEnoughCoins(staff[index].staffInfoList[i].price))
							{
								Game.Instance.Coins -= staff[index].staffInfoList[i].price;
								PlayerPrefs.SetInt("Coins", Game.Instance.Coins);							
							}
							else
							{								
								staff[index].OnStaffHireBtnClicked(i);
								yield return new WaitForSeconds(2f);							
							}
							break;
					}
				}
			}
		}
	}
}