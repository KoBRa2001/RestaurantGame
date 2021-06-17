using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Staff : MonoBehaviour
{
    public List<StaffInfo> staffInfoList;
    [SerializeField] Text coinsText;
    [SerializeField] List<Image> avatarList;
    [SerializeField] Sprite defaultAvatar;
    private int staffCounter;

    [SerializeField] GameObject itemTemplate;
    GameObject g;
    [SerializeField]
    Transform staffScrollView;    
    Button buyBtn;
    private int listCounter;

    [SerializeField] GameObject staffPrefab;
    [SerializeField] Transform staffPrefabParent;

    private bool canHire = false; 

    public List<bool> isHiredList;

    private bool firstOpen = true;

    
    public void OnStaffHireBtnClicked(int itemIndex)
    {
        if (staffInfoList[itemIndex].isHired == false)
        {
            for (int i = 0; i < avatarList.Count; i++)
            {
                canHire = false;
                if(avatarList[i].transform.GetChild(0).GetComponent<Image>().sprite == defaultAvatar)
                {
                    canHire = true;
                    break;
                }
            }

            if (canHire)
            {
                if (Game.Instance.HasEnoughCoins(staffInfoList[itemIndex].price))
                {
                    Game.Instance.UseCoins(staffInfoList[itemIndex].price);

                    buyBtn = staffScrollView.GetChild(itemIndex).GetChild(5).GetComponent<Button>();
                    staffInfoList[itemIndex].isHired = true;
                    isHiredList[itemIndex] = staffInfoList[itemIndex].isHired;
                    PlayerPrefsExtra.SetList<bool>(gameObject.name + "isHired", isHiredList);
                    buyBtn.GetComponent<Image>().color = staffInfoList[itemIndex].fireColor;
                    buyBtn.transform.GetChild(0).GetComponent<Text>().text = "FIRE";
                    for (int i = 0; i < avatarList.Count; i++)
                    {
                        if (avatarList[i].transform.GetChild(0).GetComponent<Image>().sprite == defaultAvatar)
                        {
                            avatarList[i].transform.GetChild(0).GetComponent<Image>().sprite = staffInfoList[itemIndex].image;
                            staffPrefab.GetComponent<SpriteRenderer>().sprite = staffInfoList[itemIndex].image;
                            if (staffPrefab.GetComponent<WaiterLogic>())
                            {
                                staffPrefab.GetComponent<WaiterLogic>().skill = staffInfoList[itemIndex].skill;
                                staffPrefab.GetComponent<WaiterLogic>().speed = staffInfoList[itemIndex].speed;
                            }
                            if (staffPrefab.GetComponent<ShefLogic>())
                            {
                                staffPrefab.GetComponent<ShefLogic>().skill = staffInfoList[itemIndex].skill;
                                staffPrefab.GetComponent<ShefLogic>().speed = staffInfoList[itemIndex].speed;
                            }
                            Game.Instance.salary += staffInfoList[itemIndex].price;
                            break;
                        }
                    }                    
                    Instantiate(staffPrefab, staffPrefabParent);
                    staffCounter++;
                    SetCoinsUI();

                }
                else
                {
                    Toast.Instance.Show("No enough money!", 1f, Toast.ToastColor.Custom);
                    //Debug.Log("No enough money!");
                }
            }
            else
            {
                Toast.Instance.Show("Fire someone at first", 1f, Toast.ToastColor.Custom);
                //Debug.Log("Fire someone at first");
            }
        }
        else
        {
            buyBtn = staffScrollView.GetChild(itemIndex).GetChild(5).GetComponent<Button>();
            staffInfoList[itemIndex].isHired = false;
            Game.Instance.salary -= staffInfoList[itemIndex].price;
            isHiredList[itemIndex] = staffInfoList[itemIndex].isHired;
            PlayerPrefsExtra.SetList<bool>(gameObject.name + "isHired", isHiredList);
            buyBtn.GetComponent<Image>().color = staffInfoList[itemIndex].hireColor;
            buyBtn.transform.GetChild(0).GetComponent<Text>().text = "HIRE";
            for(int i = 0; i < staffCounter; i++)
            {
                if(avatarList[i].transform.GetChild(0).GetComponent<Image>().sprite == staffInfoList[itemIndex].image)
                {
                    if (staffPrefabParent.transform.GetChild(i).GetComponent<PrefabDestroying>())
                    {
                        staffPrefabParent.transform.GetChild(i).GetComponent<PrefabDestroying>().DestroyPrefab();
                    }
                    while (i < staffCounter)
                    {
                        if (i == staffCounter - 1)
                        {
                            avatarList[i].transform.GetChild(0).GetComponent<Image>().sprite = defaultAvatar;
                        }
                        else
                            avatarList[i].transform.GetChild(0).GetComponent<Image>().sprite = avatarList[i + 1].transform.GetChild(0).GetComponent<Image>().sprite;
                        i++;
                    }
                    break;
                }
            }
            staffCounter--;
        }
        SetCoinsUI();
    }

    void UpdateList()
    {
        isHiredList = PlayerPrefsExtra.GetList<bool>(gameObject.name + "isHired", new List<bool>());
        if (staffInfoList.Count > listCounter)
        {
            int len = staffInfoList.Count;
            for (int i = listCounter; i < len; i++)
            {
                g = Instantiate(itemTemplate, staffScrollView);
                g.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = staffInfoList[i].image;
                g.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = staffInfoList[i].price.ToString();
                g.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = staffInfoList[i].name;
                g.transform.GetChild(3).GetComponent<Image>().fillAmount = Mathf.InverseLerp(0f, 5f, staffInfoList[i].skill);               
                g.transform.GetChild(4).GetComponent<Image>().fillAmount = Mathf.InverseLerp(0f, 5f, staffInfoList[i].speed);                
                buyBtn = g.transform.GetChild(5).GetComponent<Button>();
                //buyBtn.interactable = !isHiredList[i];                
                if (staffInfoList[i].isHired == true)
                {
                    buyBtn.transform.GetChild(0).GetComponent<Text>().text = "FIRE";
                    buyBtn.GetComponent<Image>().color = staffInfoList[i].fireColor;
                    Game.Instance.salary += staffInfoList[i].price;
                }
                else
                {
                    buyBtn.transform.GetChild(0).GetComponent<Text>().text = "HIRE";
                    buyBtn.GetComponent<Image>().color = staffInfoList[i].hireColor;
                }
                buyBtn.AddEventListener(i, OnStaffHireBtnClicked);
            }
            listCounter = staffInfoList.Count;
        }        
    }


    public void UpdateAvatar()
    {
        int avID = 0;
        for (int i = 0; i < staffInfoList.Count; i++)
        {
            if (staffInfoList[i].isHired == true)
            {
                avatarList[avID].transform.GetChild(0).GetComponent<Image>().sprite = staffInfoList[i].image;
                staffPrefab.GetComponent<SpriteRenderer>().sprite = staffInfoList[i].image;
                if (staffPrefab.GetComponent<WaiterLogic>())
                {
                    staffPrefab.GetComponent<WaiterLogic>().skill = staffInfoList[i].skill;
                    staffPrefab.GetComponent<WaiterLogic>().speed = staffInfoList[i].speed;
                }
                if (staffPrefab.GetComponent<ShefLogic>())
                {
                    staffPrefab.GetComponent<ShefLogic>().skill = staffInfoList[i].skill;
                    staffPrefab.GetComponent<ShefLogic>().speed = staffInfoList[i].speed;
                }
                Instantiate(staffPrefab, staffPrefabParent);
                staffCounter++;
                avID++;
            }                
        }
        if (firstOpen)
        {
            firstOpen = false;         
            CloseShop();
        }
    }

    public void SetCoinsUI()
    {
        coinsText.text = Game.Instance.Coins.ToString();
    }

    public void OpenShop()
    {
        UpdateList();        
        gameObject.GetComponent<TweenAnimation>().Open();
        if (firstOpen)
        {            
            UpdateAvatar();            
        }
    }

    public void CloseShop()
    {
        ZoomPan.zoomEnable = true;
        gameObject.GetComponent<TweenAnimation>().Close();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {            
            PlayerPrefs.DeleteAll();
        }
    }
}
