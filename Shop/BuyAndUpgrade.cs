using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyAndUpgrade : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> allTables;
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

    public List<GameObject> fullTable;
    public List<bool> isObjectCreated;

    private void OnEnable()
    {
        SetCoinsUI();
    }

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
        isObjectCreated = PlayerPrefsExtra.GetList<bool>(gameObject.name + "Created", new List<bool>());
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
                isObjectCreated.Add(shopItemsList[i].isPurchased);
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

            if (isObjectCreated[i])
            {
                allTables[i].SetActive(true);
                allTables[i].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = shopItemsList[i].obj[shopItemsList[i].currentLevel].GetComponent<SpriteRenderer>().sprite;

                if (allTables[i].GetComponent<TableLogic>())
                    Game.Instance.avaragePrice += allTables[i].GetComponent<TableLogic>().price;
            }
        }

        PlayerPrefsExtra.SetList<int>(gameObject.name + "Price", shopItemPrice);
        PlayerPrefsExtra.SetList<bool>(gameObject.name + "Purchased", shopItemPurchased);
        PlayerPrefsExtra.SetList<bool>(gameObject.name + "Created", isObjectCreated);
        PlayerPrefsExtra.SetList<int>(gameObject.name + "Level", shopItemLevel);
        PlayerPrefs.SetFloat("AvPrice", Game.Instance.avaragePrice);

        SetCoinsUI();
        CloseShop();
    }

    void OnShopItemBtnClicked(int itemIndex)
    {
        //Upgrade

        if (isObjectCreated[itemIndex])
        {
            if (Game.Instance.HasEnoughCoins(shopItemsList[itemIndex].price))
            {
                if (shopItemsList[itemIndex].currentLevel < shopItemsList[itemIndex].maxLevel)
                {
                    Game.Instance.UseCoins(shopItemsList[itemIndex].price);
                    Game.Instance.curXP += shopItemsList[itemIndex].xp;
                    shopItemsList[itemIndex].currentLevel++;

                    allTables[itemIndex].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = shopItemsList[itemIndex].obj[shopItemsList[itemIndex].currentLevel].GetComponent<SpriteRenderer>().sprite;
                    if (allTables[itemIndex].GetComponent<TableLogic>())
                    {
                        int tableIncome = allTables[itemIndex].GetComponent<TableLogic>().price += Mathf.RoundToInt((allTables[itemIndex].GetComponent<TableLogic>().price * 0.2f));
                        PlayerPrefs.SetInt(allTables[itemIndex].name + "Price", tableIncome);
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


        //BuyNew

        if (!isObjectCreated[itemIndex])
        {
            //ZoomPan.zoomEnable = false;         
            if (Game.Instance.HasEnoughCoins(shopItemsList[itemIndex].price))
            {
                if (shopItemsList[itemIndex].currentLevel < shopItemsList[itemIndex].maxLevel)
                {


                    Game.Instance.UseCoins(shopItemsList[itemIndex].price);
                    Game.Instance.curXP += shopItemsList[itemIndex].xp;
                    GameObject parent = null;
                    parent = allTables[itemIndex];
                    parent.SetActive(true);
                    switch (GetPersonCount(parent))
                    {
                        case 1:
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
                                Game.Instance.toilets.Add(parent);
                                PlayerPrefsExtra.SetList("listToilets", Game.Instance.toilets);
                            }
                            break;

                    }
                    PlayerPrefsExtra.SetList("listTables", Game.Instance.tables);
                    
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
                    isObjectCreated[itemIndex] = true;
                    shopItemLevel[itemIndex] = shopItemsList[itemIndex].currentLevel;

                    PlayerPrefsExtra.SetList<int>(gameObject.name + "Price", shopItemPrice);
                    PlayerPrefsExtra.SetList<bool>(gameObject.name + "Purchased", shopItemPurchased);
                    PlayerPrefsExtra.SetList<bool>(gameObject.name + "Created", isObjectCreated);
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

    public int GetPersonCount(GameObject obj)
    {
        if (obj.tag == "1Person")
        {
            Game.Instance.tables1.Add(obj.transform.position);
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

}