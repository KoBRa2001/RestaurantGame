using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
    [SerializeField]
    private Light2D enviromentLight;

    //Нехай ігровий день триває 15хв=900сек. 24год=86400сек.
    //Значить кожну секунду потрібно оновлювати час на 86400/900=96сек.
    public float updateTime;
    public float updateDelay;
    [SerializeField]
    private Text timeField;

    private bool salaryTime = true;
    private int hours;

    [SerializeField]
    private List<Staff> staff;
    [SerializeField]
    private PromotionController promoController;

    private void Start()
    {
        hours = PlayerPrefs.GetInt("Hours", 0);
        LightsControl();
        InvokeRepeating("UpdateTime", updateTime, updateDelay);
    }   

    public void UpdateTime()
    {
        timeField.text = string.Format("{0}:{1} ", Math.Truncate(Game.Instance.currentGameTime / 3600).ToString("00"),
            Math.Truncate((Game.Instance.currentGameTime - 3600 * 
            Math.Truncate(Game.Instance.currentGameTime / 3600)) / 60).ToString("00"));        

        if (hours < Convert.ToInt32(Math.Truncate(Game.Instance.currentGameTime / 3600)))
        {
            Debug.Log("Hour spend");
            hours++;
            PlayerPrefs.SetInt("Hours", hours);

            if (Game.Instance.HasEnoughCoins(Game.Instance.promote))
            {
                Game.Instance.Coins -= Game.Instance.promote;
                PlayerPrefs.SetInt("Coins", Game.Instance.Coins);
            }
            else {
                for (int i = 0; i < promoController.promotionVariants.Count; i++)
                {
                    GameObject child = promoController.transform.GetChild(0).transform.GetChild(i).gameObject;
                    int price = Convert.ToInt32(child.transform.GetChild(3).GetComponent<Text>().text);
                    Button buyBtn = child.transform.GetChild(4).GetComponent<Button>();
                    if (buyBtn.transform.GetChild(0).GetComponent<Text>().text == "End")
                    {
                        promoController.OnShopItemBtnClicked(i);
                        break;
                    }
                }
            }
        }

        LightsControl();        
        Game.Instance.currentGameTime += 96f;
        if (Game.Instance.currentGameTime >= 86400f)
        {
            Game.Instance.currentGameTime = 0f;
            salaryTime = true;
        }
        if (Game.Instance.currentGameTime >= Game.Instance.closeTime && salaryTime)
        {
            salaryTime = false;
            if (Game.Instance.HasEnoughCoins(Game.Instance.salary))
            {
                Game.Instance.Coins -= Game.Instance.salary;
                Debug.Log(Game.Instance.salary);
            }
            else
            {
                StartCoroutine(Fire());              
                //Debug.Log("TODO! Fire staff, not enough money for pay sallary");
            }            
        }
    }   

    public IEnumerator Fire()
    {
        Debug.Log("Salary work");
        for (int index = 0; index < staff.Count; index++)
        {
            for (int i = 0; i < staff[index].staffInfoList.Count; i++)
            {
                if (staff[index].isHiredList[i])
                {
                    if (Game.Instance.HasEnoughCoins(staff[index].staffInfoList[i].price))
                    {
                        Game.Instance.Coins -= staff[index].staffInfoList[i].price;
                        PlayerPrefs.SetInt("Coins", Game.Instance.Coins);
                        //Debug.Log("Success payment");
                    }
                    else
                    {                        
                        staff[index].OnStaffHireBtnClicked(i);
                        yield return new WaitForSeconds(2f);                     
                    }
                }
            }
        }
    }

    public void SkipNight()
    {
        Game.Instance.currentGameTime = 23300f;        
    }

    public void LightsControl()
    {
        if (5f <= hours && hours < 20f)
        {
            enviromentLight.intensity = 1f;
        }
        else 
        {
            enviromentLight.intensity = 0.37f;
        }
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
