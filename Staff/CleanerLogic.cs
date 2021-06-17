using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanerLogic : MonoBehaviour
{
    private SAP2D.SAP2DAgent cleanerAgent;
    private CleanerController controller;
    private GameObject target = null;
    [SerializeField]
    private Transform myPosition;

    private void Awake()
    {        
        cleanerAgent = GetComponent<SAP2D.SAP2DAgent>();
        cleanerAgent.Target = myPosition;
        controller = FindObjectOfType<CleanerController>();
        controller.cleaners.Add(this);
    }    

    public void GoClean()
    {        
        if (Game.Instance.toClean.Count > 0)
        {
            target = Game.Instance.toClean[Game.Instance.toClean.Count - 1];
            cleanerAgent.Target = target.transform;
            Game.Instance.toClean.Remove(target);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {        
        if (other.gameObject == target.gameObject)
        {
            StartCoroutine(CleaningProcess());           
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Table"))
        {
            if (Game.Instance.falseTables.Contains(other.gameObject.transform.position))
            {
                Game.Instance.falseTables.Remove(other.gameObject.transform.position);                
            }
        }        

        if (other.tag == "Toilet")
        {
            if (Game.Instance.falseToilets.Contains(other.gameObject.transform.position))
            {
                Game.Instance.falseToilets.Remove(other.gameObject.transform.position);
            }
        }
    }

    public IEnumerator CleaningProcess()
    {
        //Debug.Log("Clean 5 sec");
        yield return new WaitForSeconds(5f);
        GoToMyPosition();
    }

    public void GoToMyPosition()
    {        
        cleanerAgent.Target = myPosition;        
        if (controller.busyCleaners.Contains(gameObject.GetComponent<CleanerLogic>()))
        {
            controller.busyCleaners.Remove(gameObject.GetComponent<CleanerLogic>());
            controller.cleaners.Add(gameObject.GetComponent<CleanerLogic>());
        }
    }

}
