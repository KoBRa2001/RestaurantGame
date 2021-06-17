using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OpenPanelBuyTable : MonoBehaviour
{
    public GameObject shopPanel;
    private Vector3 mousePos;    

    private void Awake()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;                     
    }


    private void Start()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
    }
    

    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            mousePos = Camera.main.transform.position;
            Debug.Log("pos");
        }
    }

    private void OnMouseUpAsButton()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("final");
            Vector3 mousePosFinal = Camera.main.transform.position;
            Vector3 delta = mousePos - mousePosFinal;            
            if (delta.x <= 3f && delta.x >= -3f &&
                delta.y <= 3f && delta.y >= -3f)
            {               
                shopPanel.SetActive(true);
                ZoomPan.zoomEnable = false;
            }

            if (gameObject.layer != LayerMask.NameToLayer("Table"))
            {
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                gameObject.GetComponent<BoxCollider2D>().enabled = true;
            }
            
            if (gameObject.layer != LayerMask.NameToLayer("Toilet"))
            {
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                gameObject.GetComponent<BoxCollider2D>().enabled = true;
            }            
        }
    }
}
