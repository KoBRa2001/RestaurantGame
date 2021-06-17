using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShefController : MonoBehaviour
{
    public List<ShefLogic> shefs;
    public List<ShefLogic> busyShefs;
    public float speed;    

    public void MakeFood()
    {
        if (shefs.Count > 0)
        {
            for (int i = 0; i < shefs.Count; i++)
            {
                if (shefs[i] == null)
                    shefs.RemoveAt(i);
            }
        }

        if (shefs.Count > 0)
        {
            for (int i = 0; i < busyShefs.Count; i++)
            {
                if (busyShefs[i] == null)
                    busyShefs.RemoveAt(i);
            }
        }

        if (shefs.Count > 0)
        {
            shefs[shefs.Count - 1].StartCoroutine(shefs[shefs.Count - 1].MakeFood());
            speed = shefs[shefs.Count - 1].speed;
            busyShefs.Add(shefs[shefs.Count - 1]);
            shefs.RemoveAt(shefs.Count - 1);
        }        
    }
}
