using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShefLogic : MonoBehaviour
{
    private ShefController controller;
    public float speed = 5f;
    public float skill;

    private void Awake()
    {
        controller = FindObjectOfType<ShefController>();
        controller.shefs.Add(this);
    }  

    public IEnumerator MakeFood()
    {
        yield return new WaitForSeconds(7f - speed);
        Debug.Log("Done");
        if (controller.busyShefs.Contains(gameObject.GetComponent<ShefLogic>()))
        {
            controller.busyShefs.Remove(gameObject.GetComponent<ShefLogic>());
            controller.shefs.Add(gameObject.GetComponent<ShefLogic>());
        }
    }
}
