using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaiterLogic : MonoBehaviour
{
    [SerializeField]
    private WaiterController controller;    
    public bool withOrder = false;
    private Vector3 target = Vector3.zero;
    [SerializeField]
    private Transform point;    
    private Transform myPosition;
    private Vector3 defaulPosMyPos;
    private SAP2D.SAP2DAgent waiterAgent;    
    private ShefController shefController;


    public float speed;
    public float skill;

    public int tempPrice;    

    private void Awake()
    {        
        shefController = FindObjectOfType<ShefController>();        
        controller = FindObjectOfType<WaiterController>();
        controller.waiters.Add(this);
        waiterAgent = GetComponent<SAP2D.SAP2DAgent>();
        waiterAgent.MovementSpeed = speed;
        defaulPosMyPos = myPosition.position;     
    }

    
    private void Update()
    {
        if (target != Vector3.zero)
        {
            if (!Game.Instance.falseTables.Contains(target))
            {        
                GoToMyPosition();
            }
        }       
    }

    public void LetOrder()
    {   target = Game.Instance.waitForWaiter[Game.Instance.waitForWaiter.Count - 1];
        Game.Instance.waitForWaiter.RemoveAt(Game.Instance.waitForWaiter.Count - 1);     

        foreach (GameObject allTable in Game.Instance.tables)
        {
            if (allTable.transform.position == target)
            {                
                waiterAgent.Target = allTable.transform;                
                break;
            }
        }
    }

    public IEnumerator MakeOrder()
    {
        yield return new WaitForSeconds(7f - skill);
        waiterAgent.Target = point;
    }

    public IEnumerator TakeOrder()
    {
        yield return new WaitUntil(() => shefController.shefs.Count > 0);
        
        if (shefController.shefs.Count > 0)
        {
            shefController.MakeFood();
            tempPrice = Mathf.RoundToInt(shefController.busyShefs[shefController.busyShefs.Count - 1].speed + shefController.busyShefs[shefController.busyShefs.Count - 1].skill);
            tempPrice += Mathf.RoundToInt(skill + speed);
        }

        yield return new WaitForSeconds(7f - shefController.speed + 0.5f);
        foreach (GameObject allTable in Game.Instance.tables)
        {
            if (allTable.transform.position == target)
            {
                waiterAgent.Target = allTable.transform;        
                break;
            }
        }
        withOrder = true; 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "TakeOrder")
        {
            StartCoroutine(TakeOrder());
        }
    }

    public void GoToMyPosition()
    {
        tempPrice = 0;        
        myPosition.position = defaulPosMyPos;
        target = Vector3.zero;
        waiterAgent.Target = myPosition;
        withOrder = false;
        
        if (controller.busyWaiters.Contains(gameObject.GetComponent<WaiterLogic>()))
        {
            controller.busyWaiters.Remove(gameObject.GetComponent<WaiterLogic>());
            controller.waiters.Add(gameObject.GetComponent<WaiterLogic>());
        }
    }
}
