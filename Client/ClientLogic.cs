using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ClientLogic : MonoBehaviour
{
    [SerializeField]
    private Transform startPoint;
    private Vector3 target;
    [SerializeField]
    private Transform endPoint;
    public bool waiting = false;
    public bool takeMeal = false;
    private bool toilet = false;
    public int dirt = 0;
    private bool clientEnterTable = false;

    private bool eatAndGo = false;
    private int price;    

    private SAP2D.SAP2DAgent agent;    

    private void Awake()
    {
        agent = gameObject.GetComponent<SAP2D.SAP2DAgent>();
        agent.Target = startPoint;        
    }

    
    private void OnTriggerExit2D(Collider2D other)
    {        
        if (other.gameObject.layer == LayerMask.NameToLayer("Table"))
        {
            gameObject.transform.Find("Text").GetComponent<TextMesh>().text = "";
     
            price += other.gameObject.GetComponent<TableLogic>().price;
            Game.Instance.Coins += price;
            Debug.Log("+" + price);
            other.gameObject.GetComponent<TableLogic>().price = PlayerPrefs.GetInt(other.gameObject.name + "Price");
            PlayerPrefs.SetInt("Coins", Game.Instance.Coins);
        }

        if (other.tag == "Toilet")
        {
            clientEnterTable = false;
            
            if (Game.Instance.falseTables.Contains(other.transform.position))
            {
                Game.Instance.falseTables.Remove(other.transform.position);                
            }
            int dirty = dirt;
            if (dirty == 0)
            {
                if (Game.Instance.falseToilets.Contains(other.gameObject.transform.position))
                {
                    Game.Instance.falseToilets.Remove(other.gameObject.transform.position);
                }
            }
            else if (dirty == 1)
            {
                Debug.Log("Need clean");
                Game.Instance.toClean.Add(other.gameObject);
            }         
        }
    }


    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.tag == "StartPoint")
        {
            if (clientEnterTable == false)
            {                                              
                clientEnterTable = true;    
                FirstTryFindTarget();
            }
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Table"))
        {
            clientEnterTable = true;
            Debug.Log("Trigger table");

            if (!Game.Instance.falseTables.Contains(other.transform.position))
            {
                Game.Instance.falseTables.Add(other.transform.position);
            }
            Game.Instance.waitForWaiter.Add(other.transform.position);
            StartCoroutine(WaitForWaiter());            
        }

        //if (other.tag == "EndPoint")
        //{
        //    ///Debug.Log("EndPoint");
        //    gameObject.GetComponent<PrefabDestroying>().DestroyPrefab();
        //    //Destroy(gameObject);
        //}

        if (other.tag == "Toilet")
        {
            StartCoroutine(ToiletProcedure());            
        }
    }

    public void FindTarget()
    {
        if (gameObject.tag == "Crowd1")
        {
            if (Game.Instance.tables1.Count != 0)
            {                
                int i = 0;
                foreach (Vector3 tables in Game.Instance.tables1)
                {
                    if (!Game.Instance.falseTables.Contains(tables))
                    {
                        Game.Instance.falseTables.Add(Game.Instance.tables1[i]);
                        foreach (GameObject allTable in Game.Instance.tables)
                        {
                            if (allTable.transform.position == tables)
                            {
                                target = tables;
                                agent.Target = allTable.transform;             
                                Debug.Log("Break");
                                break;
                            }
                        }
                        break;                        
                    }
                    else
                        i++;
                }                
            }
            if (agent.Target == startPoint & Game.Instance.tables2.Count != 0)
            {                
                int i = 0;
                foreach (Vector3 tables in Game.Instance.tables2)
                {
                    if (Game.Instance.falseTables.Contains(Game.Instance.tables2[i]))
                    {
                        i++;
                    }
                    else
                    {
                        agent.Target.position = Game.Instance.tables2[i];
                        target = Game.Instance.tables2[i];
                        Game.Instance.falseTables.Add(Game.Instance.tables2[i]);
                        break;
                    }

                }
            }
            if (agent.Target == startPoint & Game.Instance.tables4.Count != 0)
            {               
                int i = 0;
                foreach (Vector3 tables in Game.Instance.tables4)
                {
                    if (!Game.Instance.falseTables.Contains(Game.Instance.tables4[i]))
                    {
                        agent.Target.position = Game.Instance.tables4[i];
                        target = Game.Instance.tables4[i];
                        Game.Instance.falseTables.Add(Game.Instance.tables4[i]);
                        break;
                    }
                    else
                        i++;
                }
            }
            
        }
        else if (gameObject.tag == "Crowd2")
        {
            if (Game.Instance.tables2.Count != 0)
            {
                int i = 0;
                foreach (Vector3 tables in Game.Instance.tables2)
                {
                    if (!Game.Instance.falseTables.Contains(Game.Instance.tables2[i]))
                    {
                        agent.Target.position = Game.Instance.tables2[i];
                        target = Game.Instance.tables2[i];
                        Game.Instance.falseTables.Add(Game.Instance.tables2[i]);
                        break;
                    }
                    else
                        i++;
                }
            }
            if (agent.Target == startPoint & Game.Instance.tables4.Count != 0)
            {             
                int i = 0;
                foreach (Vector3 tables in Game.Instance.tables4)
                {
                    if (!Game.Instance.falseTables.Contains(Game.Instance.tables4[i]))
                    {
                        agent.Target.position = Game.Instance.tables4[i];
                        target = Game.Instance.tables4[i];
                        Game.Instance.falseTables.Add(Game.Instance.tables4[i]);
                        break;
                    }
                    else
                        i++;
                }
            }
        }
        else if (gameObject.tag == "Crowd4")
        {
            if (Game.Instance.tables4.Count != 0)
            {
                int i = 0;
                foreach (Vector3 tables in Game.Instance.tables4)
                {
                    if (!Game.Instance.falseTables.Contains(Game.Instance.tables4[i]))
                    {
                        agent.Target.position = Game.Instance.tables4[i];
                        target = Game.Instance.tables4[i];
                        Game.Instance.falseTables.Add(Game.Instance.tables4[i]);
                        break;                        
                    }
                    else
                        i++;
                }
            }            
        }
    }

    public void FirstTryFindTarget()
    {     
        FindTarget();
        if (target == Vector3.zero)
        {
            Debug.Log("No target");            
            StartCoroutine(SecondTryFindTarget());            
        }
        else
        {
            Debug.Log("Target: " + agent.Target.position);
        }
    }

    IEnumerator SecondTryFindTarget()
    {        
        yield return new WaitForSeconds(5f);
        FindTarget();
        if (target == Vector3.zero)
        {
            agent.Target = endPoint;            
        }
        else
        {
            Debug.Log("Target: " + agent.Target.position);
        }        
    }

    //Чекаємо на офіціанта, якщо не підходить за якийсь час, клієнт йде
    IEnumerator WaitForWaiter()
    {       
        float timeWait = 10f;        
        while (timeWait >= 0f)
        {
            gameObject.transform.Find("Text").GetComponent<TextMesh>().text = "Wait for waiter " + timeWait;
            yield return new WaitForSeconds(1);
            if (waiting == true)
            {
                //Виклик корутини чекати на їжу
                StartCoroutine(WaitForMeal());
                //timeWait = 10f;
                yield return null;
                break;
            }
            timeWait--;     
        }
        if (timeWait < 0f)
        {
            Debug.Log("Time Out");
            agent.Target = endPoint;
            
            yield return null;
        }
    }

    IEnumerator WaitForMeal()
    {
        Debug.Log("WaitForMeal");
        float timeWait = 30f;        
        while (timeWait >= 0f)
        {
            gameObject.transform.Find("Text").GetComponent<TextMesh>().text = "Wait for meal " + timeWait;
            yield return new WaitForSeconds(1);
            if (takeMeal == true)
            {
                StartCoroutine(EatAndGo());
                yield return null;
                break;
            }
            timeWait--;            
        }
        if (timeWait < 0f)
        {
            agent.Target = endPoint;
            Debug.Log("Time Out, i left cafe hungry");
                        
            yield return null;
        }
    }

    IEnumerator EatAndGo()
    {
        gameObject.transform.Find("Text").GetComponent<TextMesh>().text = "Eating";
        bool isFreeToilet = false;        
        yield return new WaitForSeconds(5f);
        Debug.Log("Maybe toilet?");
        int toil = Random.Range(0, 2);        
        if (toil == 0)
        {
            Debug.Log("No");
            agent.Target = endPoint;                                   
        }
        else if (toil == 1)
        {
            if (Game.Instance.toilets.Count > 0)
            {
                Debug.Log("There are some toilets");
                foreach (GameObject freeToilet in Game.Instance.toilets)
                {
                    if (!Game.Instance.falseToilets.Contains(freeToilet.transform.position))
                    {
                        Debug.Log("There is free toilet");
                        Game.Instance.falseToilets.Add(freeToilet.transform.position);
                        agent.Target = freeToilet.transform;
                        isFreeToilet = true;
                        break;
                    }
                }
            }
            if (isFreeToilet == false)
            {
                Debug.Log("There is no free toilets(");
                agent.Target = endPoint;
            }            
        }

        dirt = Random.Range(0, 2);


        eatAndGo = true;
    }

    IEnumerator ToiletProcedure()
    {
        Debug.Log("Toilet Procedure");
        yield return new WaitForSeconds(5f);
        Debug.Log("Nice");
        dirt = Random.Range(0, 2);       
        agent.Target = endPoint;        
    }

}
