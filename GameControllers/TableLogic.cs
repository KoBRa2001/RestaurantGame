using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableLogic : MonoBehaviour
{    
    private bool waiting = false;
    private ClientLogic client;
    private WaiterLogic waiter; 
    private bool toClean = false;
    private bool waiterEnter = false;
    private bool clientEnter = false;
    
    public int price;

    private void Awake()
    {
        price = PlayerPrefs.GetInt(gameObject.name + "Price", price);
    }

    private void Update()
    {
        if (waiting == true)
        {
            if (client != null)
                client.waiting = true;
        }
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("Delete");
            PlayerPrefs.DeleteAll();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Crowd1" || other.gameObject.tag == "Crowd2" || other.gameObject.tag == "Crowd4")
        {
            if (Game.Instance.waitForWaiter.Contains(gameObject.transform.position))
            {
                Debug.Log("Contain waitForWaiter");
                Game.Instance.waitForWaiter.Remove(gameObject.transform.position);
            }
            waiting = false;
            client = null;

            int dirty = other.GetComponent<ClientLogic>().dirt;
            if (dirty == 0)
            {
                toClean = false;
                if (Game.Instance.falseTables.Contains(gameObject.transform.position))
                {
                    Debug.Log("Contain");         
                    Game.Instance.falseTables.Remove(gameObject.transform.position);                    
                }
            }
            else if (dirty == 1)
            {
                toClean = true;
                Debug.Log("Need clean");
                Game.Instance.toClean.Add(gameObject);
            }
            else
            {
                Debug.Log("LOX");
            }
            clientEnter = false;            
        }

        if (other.gameObject.tag == "Waiter")
        {
            waiterEnter = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Crowd1" || other.gameObject.tag == "Crowd2" || other.gameObject.tag == "Crowd4")
        {
            if (clientEnter == false)
            {
                clientEnter = true;
                if (other.GetComponent<ClientLogic>() != null)
                {
                    client = other.GetComponent<ClientLogic>();                   
                }                                
            }
        }

        if (other.gameObject.tag == "Waiter")
        {
            if (waiterEnter == false)
            {
                Debug.Log("Waiter here");
                waiter = other.GetComponent<WaiterLogic>();
                if (waiter.withOrder == false)
                {
                    waiter.StartCoroutine(waiter.MakeOrder());
                    waiting = true;
                    waiterEnter = true;
                }
                else if (waiter.withOrder == true)
                {
                    waiterEnter = true;
                    if (client != null)
                    {
                        client.takeMeal = true;
                    }
                    price += waiter.tempPrice;                    
                    Debug.Log("GoToMyPosition");
                    waiter.GoToMyPosition();
                    waiting = false;
                    client = null;
                    waiter = null;
                }
            }
        }
    }       
}
