using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromotionController : MonoBehaviour
{
    [SerializeField]
    private ClientSpawner clSp;    
    public List<GameObject> promotionVariants;
    private List<int> priceList = new List<int>();
    [SerializeField]
    private GameObject content;

    [SerializeField] Text coinsText;
    Button buyBtn;

    private void Start()
    {        
        Game.Instance.promote = PlayerPrefs.GetInt("Promote", 0);
        int len = promotionVariants.Count;
        Debug.Log(promotionVariants.Count);
        for (int i = 0; i < len; i++)
        {
            Debug.Log("Loop");
            GameObject child = content.transform.GetChild(i).gameObject;
            int price = Convert.ToInt32(child.transform.GetChild(3).GetComponent<Text>().text);
            priceList.Add(price);            
            buyBtn = child.transform.GetChild(4).GetComponent<Button>();
            Debug.Log("Loop");
            buyBtn.AddEventListener(i, OnShopItemBtnClicked);

            if (Game.Instance.promote != 0 && Game.Instance.promote == price)
            {
                buyBtn.transform.GetChild(0).GetComponent<Text>().text = "End";
            }
        }        
     
        CloseShop();
    }

    public void OnShopItemBtnClicked(int itemIndex)
    {        
        ZoomPan.zoomEnable = false;        
        GameObject child = content.transform.GetChild(itemIndex).gameObject;
        buyBtn = child.transform.GetChild(4).GetComponent<Button>();
        Debug.Log("Click");
        Debug.Log(Convert.ToInt32(child.transform.GetChild(1).GetComponent<Text>().text));


        if (buyBtn.transform.GetChild(0).GetComponent<Text>().text == "Start")
        {
            int price = Convert.ToInt32(child.transform.GetChild(3).GetComponent<Text>().text);
            if (Game.Instance.HasEnoughCoins(price))
            {
                Game.Instance.promote = price;
                Game.Instance.Coins -= price;

                int len = promotionVariants.Count;
        
                for (int i = 0; i < len; i++)
                {
                    GameObject child1 = content.transform.GetChild(i).gameObject;
                    buyBtn = child1.transform.GetChild(4).GetComponent<Button>();
                    buyBtn.transform.GetChild(0).GetComponent<Text>().text = "Start";
                }
                buyBtn = child.transform.GetChild(4).GetComponent<Button>();
                buyBtn.transform.GetChild(0).GetComponent<Text>().text = "End";

                //Calculate delay
                float percent = float.Parse(child.transform.GetChild(1).GetComponent<Text>().text);
                clSp.spawnDelay = 10 / ((percent / 100f) + 1);
                Game.Instance.promotePercent = Convert.ToInt32(percent);                
            }
            else
            {
                Debug.Log("Not enough money");
            }
        }
        else if (buyBtn.transform.GetChild(0).GetComponent<Text>().text == "End")
        {
            Game.Instance.promote = 0;
            buyBtn.transform.GetChild(0).GetComponent<Text>().text = "Start";
            clSp.spawnDelay = 10;
            Game.Instance.promotePercent = 0;
        }

        PlayerPrefs.SetInt("Coins", Game.Instance.Coins);
        PlayerPrefs.SetInt("Promote", Game.Instance.promote);
        PlayerPrefs.SetInt("Percent", Game.Instance.promotePercent);
    }

    void SetCoinsUI()
    {
        coinsText.text = PlayerPrefs.GetInt("Coins", 1500).ToString();        
    }

    public void OpenShop()
    {
        ZoomPan.zoomEnable = false;
        gameObject.SetActive(true);        
    }

    public void CloseShop()
    {
        ZoomPan.zoomEnable = true;
        gameObject.SetActive(false);       
    }

}
