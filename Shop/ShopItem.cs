using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="ShopItemData",menuName ="ShopItem")]
public class ShopItem : ScriptableObject
{
    public Sprite image;
    public int price;
    public bool isPurchased = false;
    public int currentLevel;
    public int maxLevel;
    public GameObject[] obj;

    public int xp;    

}
