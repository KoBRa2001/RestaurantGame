using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStaffUpdatePanel : MonoBehaviour
{
    [Header("Update settings")]    
    public Staff list;    
    public List<StaffInfo> allStaff;
    
    private bool isCreated = false;
    public List<bool> isHirededList;

    private void Start()
    {        
        Create();
    }

    public void Create()
    {
        isHirededList = PlayerPrefsExtra.GetList<bool>(list.gameObject.name + "isHired", new List<bool>());
        if (isCreated == false)
        {
            list.staffInfoList.Clear();
            for (int i = 0; i < allStaff.Count; i++)
            {
                StaffInfo currentItem = new StaffInfo();
                currentItem.image = allStaff[i].image;
                currentItem.price = allStaff[i].price;
                //currentItem.isHired = allStaff[i].isHired;
                currentItem.name = allStaff[i].name;
                currentItem.hireColor = allStaff[i].hireColor;
                currentItem.fireColor = allStaff[i].fireColor;
                currentItem.skill = allStaff[i].skill;
                currentItem.speed = allStaff[i].speed;                   

                if (isHirededList.Count == i)
                {
                    currentItem.isHired = allStaff[i].isHired;
                    isHirededList.Add(currentItem.isHired);
                }
                else
                {
                    currentItem.isHired = isHirededList[i];
                }

                list.staffInfoList.Add(currentItem);
            }
            isCreated = true;       
        }
        if (allStaff.Count > list.staffInfoList.Count)
        {
            //Debug.Log("Bigger");
            for(int i = list.staffInfoList.Count; i < allStaff.Count; i++)
            {
                StaffInfo currentItem = new StaffInfo();
                currentItem.image = allStaff[i].image;
                currentItem.price = allStaff[i].price;
                currentItem.isHired = allStaff[i].isHired;
                currentItem.name = allStaff[i].name;
                currentItem.hireColor = allStaff[i].hireColor;
                currentItem.fireColor = allStaff[i].fireColor;
                currentItem.skill = allStaff[i].skill;
                currentItem.speed = allStaff[i].speed;                

                if (isHirededList.Count == i)
                {
                    currentItem.isHired = allStaff[i].isHired;
                    isHirededList.Add(currentItem.isHired);
                }
                else
                {
                    currentItem.isHired = isHirededList[i];
                }

                list.staffInfoList.Add(currentItem);
            }
        }
        PlayerPrefsExtra.SetList<bool>(list.gameObject.name + "isHired", isHirededList);     
        list.SetCoinsUI();
        list.OpenShop();
        ZoomPan.zoomEnable = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            //Debug.Log("Delete");
            PlayerPrefs.DeleteAll();
        }
    }
}
