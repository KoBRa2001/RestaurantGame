using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyNewTable : MonoBehaviour
{

    public List<GameObject> allTables;
    public List<GameObject> allTables0;
    public List<GameObject> allTables1;
    public List<ShopItem> shopItemsListNotChange;
    public List<ShopItem> shopItemsList;
    public GameObject content;

    [SerializeField] Text coinsText;
    [SerializeField] Text curXPText;

    public List<int> shopItemPrice;
    public List<bool> shopItemPurchased;
    public List<int> shopItemLevel;
    
    Button buyBtn;    

    private void Awake()
    {        
        foreach (ShopItem item in shopItemsListNotChange)
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
        //itemTemplate = shopScrollView.GetChild(0).gameObject;

        int len = shopItemsList.Count;
        for (int i = 0; i < len; i++)
        {
            GameObject child = content.transform.GetChild(i).gameObject;
            child.transform.GetChild(0).GetComponent<Image>().sprite = shopItemsList[i].image;
            buyBtn = child.transform.GetChild(2).GetComponent<Button>();
            //buyBtn.interactable = !shopItemsList[i].isPurchased;            
            buyBtn.AddEventListener(i, OnShopItemBtnClicked);

            Game.Instance.maxXP += shopItemsList[i].xp * shopItemsList[i].maxLevel;

            if (shopItemPrice.Count == i)
            {
                child.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = shopItemsList[i].price.ToString();
                shopItemPrice.Add(shopItemsList[i].price);
                buyBtn.interactable = !shopItemsList[i].isPurchased;
                shopItemPurchased.Add(shopItemsList[i].isPurchased);
                //shopItemLevel[i] = shopItemsList[i].currentLevel;
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

            switch (i)
            {
                case 0:
                    for (int j = 0; j < shopItemsList[i].currentLevel; j++)
                    {
                        allTables0[j].SetActive(true);
                        if (allTables0[j].GetComponent<TableLogic>())
                            Game.Instance.avaragePrice += allTables0[j].GetComponent<TableLogic>().price;
                    }
                    break;
                case 1:
                    //allTables1[shopItemsList[i].currentLevel].SetActive(true);
                    for (int j = 0; j < shopItemsList[i].currentLevel; j++)
                    {
                        allTables1[j].SetActive(true);
                        if (allTables1[j].GetComponent<TableLogic>())
                            Game.Instance.avaragePrice += allTables1[j].GetComponent<TableLogic>().price;
                    }
                    break;

            }
        }

        //for(int i=0;i<Game.Instance.tabl)

        PlayerPrefsExtra.SetList<int>(gameObject.name + "Price", shopItemPrice);
        PlayerPrefsExtra.SetList<bool>(gameObject.name + "Purchased", shopItemPurchased);
        PlayerPrefsExtra.SetList<int>(gameObject.name + "Level", shopItemLevel);
        PlayerPrefs.SetFloat("AvPrice", Game.Instance.avaragePrice);

        //Destroy(itemTemplate); 
        SetCoinsUI();
        CloseShop();
        //Debug.Log("XP: " + xp);
    }

    private void OnEnable()
    {
        SetCoinsUI();
    }

    void OnShopItemBtnClicked(int itemIndex)
    {
        //Debug.Log("Click");
        ZoomPan.zoomEnable = false;
        //___________________________________________________________
        if (Game.Instance.HasEnoughCoins(shopItemsList[itemIndex].price))
        {
            if (shopItemsList[itemIndex].currentLevel < shopItemsList[itemIndex].maxLevel)
            {


                Game.Instance.UseCoins(shopItemsList[itemIndex].price);
                Game.Instance.curXP += shopItemsList[itemIndex].xp;
                GameObject parent = null;
                switch (itemIndex)
                {
                    case 0:
                        parent = allTables0[shopItemsList[itemIndex].currentLevel];
                        break;
                    case 1:
                        parent = allTables1[shopItemsList[itemIndex].currentLevel];
                        break;
                }

                //if (itemIndex == 0)
                //{
                //    GameObject parent = allTables[shopItemsList[itemIndex].currentLevel];
                //}
                //if (itemIndex == 1)
                //{
                //    GameObject parent = allTables[shopItemsList[itemIndex].currentLevel];
                //}                
                parent.SetActive(true);                                                
                switch (GetPersonCount(parent))
                {
                    case 1:
                        //Debug.Log("Added");
                        Game.Instance.tables.Add(parent);
                        Game.Instance.tables1.Add(parent.transform.position);                        
                        PlayerPrefsExtra.SetList("listTables1", Game.Instance.tables1);
                        break;
                    case 2:
                        //Debug.Log("Case 2");
                        break;
                    case 4:
                        //Debug.Log("Case 4");
                        break;
                    default:
                        if (parent.gameObject.layer == LayerMask.NameToLayer("Toilet"))
                        {
                            //Debug.Log("Added toilet");
                            Game.Instance.toilets.Add(parent);
                            PlayerPrefsExtra.SetList("listToilets", Game.Instance.toilets);
                        }
                        break;

                }
                PlayerPrefsExtra.SetList("listTables", Game.Instance.tables);
                //_________
                shopItemsList[itemIndex].currentLevel++;

                if (shopItemsList[itemIndex].currentLevel == shopItemsList[itemIndex].maxLevel)
                {                    
                    shopItemsList[itemIndex].isPurchased = true;
                    content.transform.GetChild(itemIndex).GetChild(2).GetChild(0).GetComponent<Text>().text = "PURCHASED";
                    shopItemsList[itemIndex].price.ToString();
                    content.transform.GetChild(itemIndex).GetChild(2).GetComponent<Button>().interactable = false;                   
                }
                else
                {
                    shopItemsList[itemIndex].price += 100;
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
            //Debug.Log("No enough money!");
        }
        //___________________________________________________________       
    }

    public int GetPersonCount(GameObject obj)
    {
        if (obj.tag == "1Person")
        {
            //Game.Instance.tables1.Add(obj.transform.position);
            return 1;
        }
        else if (obj.tag == "2Person")
        {
            Game.Instance.tables2.Add(obj.transform.position);
            return 2;
        }
        else if (obj.tag == "4Person")
        {
            Game.Instance.tables4.Add(obj.transform.position);
            return 4;
        }
        return 0;
    }

    void SetCoinsUI()
    {
        coinsText.text = PlayerPrefs.GetInt("Coins", 1500).ToString();
        curXPText.text = PlayerPrefs.GetInt("currentXP", 0).ToString();
        //Game.Instance.Coins.ToString();
    }

    public void OpenShop()
    {
        ZoomPan.zoomEnable = false;
        gameObject.SetActive(true);
        //gameObject.GetComponent<TweenAnimation>().Open();
    }

    public void CloseShop()
    {
        ZoomPan.zoomEnable = true;
        gameObject.SetActive(false);
        //gameObject.GetComponent<TweenAnimation>().Close();
    }
}
