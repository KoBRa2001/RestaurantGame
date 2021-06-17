using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeTable : MonoBehaviour
{
    [SerializeField]
    private List<ShopItem> shopItemsListNotChange;
    private List<ShopItem> shopItemsList = new List<ShopItem>();
    [SerializeField]
    private GameObject content;

    [SerializeField] Text coinsText;
    [SerializeField] Text curXPText;    

    private List<int> shopItemPrice;
    private List<bool> shopItemPurchased;
    private List<int> shopItemLevel;    
    
    Button buyBtn;

    private bool createTable;
    [SerializeField]
    private GameObject fullTable;

    private void OnEnable()
    {
        SetCoinsUI();
    }

    private void Awake()
    {
        foreach(ShopItem item in shopItemsListNotChange)
        {
            ShopItem g = new ShopItem();
            g.image = item.image;
            g.isPurchased = item.isPurchased;
            g.currentLevel = item.currentLevel;
            g.maxLevel = item.maxLevel;
            g.obj = item.obj;
            g.price = item.price;
            g.xp = item.xp;
            shopItemsList.Add(g);
        }

        shopItemPrice = PlayerPrefsExtra.GetList<int>(gameObject.name + "Price", new List<int>());
        shopItemPurchased = PlayerPrefsExtra.GetList<bool>(gameObject.name + "Purchased", new List<bool>());
        shopItemLevel = PlayerPrefsExtra.GetList<int>(gameObject.name + "Level", new List<int>());
    }

    private void Start()
    {
        int len = shopItemsList.Count;
        for (int i = 0; i < len; i++)
        {
            GameObject child = content.transform.GetChild(i).gameObject;
            child.transform.GetChild(0).GetComponent<Image>().sprite = shopItemsList[i].image;
            buyBtn = child.transform.GetChild(2).GetComponent<Button>();            
            buyBtn.AddEventListener(i, OnShopItemBtnClicked);

            Game.Instance.maxXP += shopItemsList[i].xp * shopItemsList[i].maxLevel;

            if (shopItemPrice.Count == i)
            {
                child.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = shopItemsList[i].price.ToString();
                shopItemPrice.Add(shopItemsList[i].price);
                buyBtn.interactable = !shopItemsList[i].isPurchased;
                shopItemPurchased.Add(shopItemsList[i].isPurchased);                
                shopItemLevel.Add(shopItemsList[i].currentLevel);
            }
            else
            {
                child.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = shopItemPrice[i].ToString();
                buyBtn.interactable = !shopItemPurchased[i];
                shopItemsList[i].currentLevel = shopItemLevel[i];
            }

            if (buyBtn.interactable == false)
            {
                buyBtn.transform.GetChild(0).GetComponent<Text>().text = "PURCHASED";
            }

            if (fullTable.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite)
            {
                fullTable.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = shopItemsList[i].obj[shopItemsList[i].currentLevel].GetComponent<SpriteRenderer>().sprite;                 
            }
        }

        PlayerPrefsExtra.SetList<int>(gameObject.name + "Price", shopItemPrice);
        PlayerPrefsExtra.SetList<bool>(gameObject.name + "Purchased", shopItemPurchased);
        PlayerPrefsExtra.SetList<int>(gameObject.name + "Level", shopItemLevel);                

        CloseShop();
    }

    void OnShopItemBtnClicked(int itemIndex)
    {
        if (Game.Instance.HasEnoughCoins(shopItemsList[itemIndex].price))
        {
            if (shopItemsList[itemIndex].currentLevel < shopItemsList[itemIndex].maxLevel)
            {
                Game.Instance.UseCoins(shopItemsList[itemIndex].price);
                Game.Instance.curXP += shopItemsList[itemIndex].xp;

                shopItemsList[itemIndex].currentLevel++;

                fullTable.transform.GetChild(itemIndex).GetComponent<SpriteRenderer>().sprite = shopItemsList[itemIndex].obj[shopItemsList[itemIndex].currentLevel].GetComponent<SpriteRenderer>().sprite;
                if (fullTable.GetComponent<TableLogic>())
                {
                    int tableIncome = fullTable.GetComponent<TableLogic>().price += Mathf.RoundToInt((fullTable.GetComponent<TableLogic>().price * 0.2f));
                    PlayerPrefs.SetInt(fullTable.name + "Price", tableIncome);
                }

                if (shopItemsList[itemIndex].currentLevel == shopItemsList[itemIndex].maxLevel)
                {
                    shopItemsList[itemIndex].isPurchased = true;
                    content.transform.GetChild(itemIndex).GetChild(2).GetChild(0).GetComponent<Text>().text = "PURCHASED";
                    shopItemsList[itemIndex].price.ToString();
                    content.transform.GetChild(itemIndex).GetChild(2).GetComponent<Button>().interactable = false;
                }
                else
                {
                    shopItemsList[itemIndex].price += 10;
                    content.transform.GetChild(itemIndex).GetChild(1).GetChild(0).GetComponent<Text>().text = shopItemsList[itemIndex].price.ToString();
                }

                shopItemPrice[itemIndex] = shopItemsList[itemIndex].price;
                shopItemPurchased[itemIndex] = shopItemsList[itemIndex].isPurchased;
                shopItemLevel[itemIndex] = shopItemsList[itemIndex].currentLevel;

                PlayerPrefsExtra.SetList<int>(gameObject.name + "Price", shopItemPrice);
                PlayerPrefsExtra.SetList<bool>(gameObject.name + "Purchased", shopItemPurchased);
                PlayerPrefsExtra.SetList<int>(gameObject.name + "Level", shopItemLevel);

                PlayerPrefs.SetInt("currentXP", Game.Instance.curXP);

                SetCoinsUI();
            }
        }
        else
        {
            Toast.Instance.Show("No enough money!", 1f, Toast.ToastColor.Custom);
        }
    }

    private void Update()
    {        
        if (Input.GetKeyDown(KeyCode.K))
        {         
            PlayerPrefs.DeleteAll();
        }
    }

    void SetCoinsUI()
    {
        coinsText.text = PlayerPrefs.GetInt("Coins", 1500).ToString();
        curXPText.text = PlayerPrefs.GetInt("currentXP", 0).ToString();                        
    }

    public void OpenShop()
    {
        gameObject.SetActive(true);        
    }

    public void CloseShop()
    {
        ZoomPan.zoomEnable = true;
        gameObject.SetActive(false);        
    }    
}
