using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaiterController : MonoBehaviour
{    
    public List<WaiterLogic> waiters;
    public List<WaiterLogic> busyWaiters;    

    private void Update()
    {
        if (Game.Instance.openTime < Game.Instance.currentGameTime & Game.Instance.currentGameTime < Game.Instance.closeTime)
        {
            if (Game.Instance.waitForWaiter.Count > 0)
            {
                if (waiters.Count > 0)
                {
                    if (waiters[waiters.Count - 1].GetComponent<WaiterLogic>() != null)
                    {     
                        waiters[waiters.Count - 1].GetComponent<WaiterLogic>().LetOrder();
                        busyWaiters.Add(waiters[waiters.Count - 1]);
                        waiters.RemoveAt(waiters.Count - 1);
                    }
                }
            }

            if (waiters.Count > 0)
            {
                for (int i = 0; i < waiters.Count; i++)
                {
                    if (waiters[i] == null)
                        waiters.RemoveAt(i);
                }
            }

            if (busyWaiters.Count > 0)
            {
                for (int i = 0; i < busyWaiters.Count; i++)
                {
                    if (busyWaiters[i] == null)
                        busyWaiters.RemoveAt(i);
                }
            }
        }
    }
}
