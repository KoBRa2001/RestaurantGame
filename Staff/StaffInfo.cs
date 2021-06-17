using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StaffInfoData", menuName = "StaffInfo")]
public class StaffInfo : ScriptableObject
{
    public Sprite image;
    public string name;
    public int price;
    public bool isHired = false;
    public float skill;
    public float speed;
    public Color32 hireColor;
    public Color32 fireColor;

    private void Awake()
    {
        
    }
}
