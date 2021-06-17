using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanerController : MonoBehaviour
{
    public List<CleanerLogic> cleaners;
    public List<CleanerLogic> busyCleaners;    

    private void Update()
    {
        if (Game.Instance.toClean.Count > 0)
        {
            if (cleaners.Count > 0)
            {
                cleaners[cleaners.Count - 1].GoClean();
                //speed = cleaners[cleaners.Count - 1].speed;
                busyCleaners.Add(cleaners[cleaners.Count - 1]);
                cleaners.RemoveAt(cleaners.Count - 1);
            }
        }        
    }
}
