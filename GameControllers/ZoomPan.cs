using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomPan : MonoBehaviour
{
    Vector3 touch;

    [SerializeField]
    private float zoomMin = 4;
    [SerializeField]
    private float zoomMax = 35;

    [SerializeField]
    private float xLock;
    [SerializeField]
    private float yLock;

    public static bool zoomEnable = true;

    private void Update()
    {
        if (zoomEnable)
        {
            if (Input.GetMouseButtonDown(0))
            {
                touch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroLastPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOneLastPos = touchOne.position - touchOne.deltaPosition;

                float distTouch = (touchZeroLastPos - touchOneLastPos).magnitude;
                float currentDistTouch = (touchZero.position - touchOne.position).magnitude;

                float difference = currentDistTouch - distTouch;

                Zoom(difference * 0.01f);
            }
            else if (Input.GetMouseButton(0))
            {
                Vector3 direction = touch - Camera.main.ScreenToWorldPoint(Input.mousePosition);                
                Camera.main.transform.position = new Vector3(
                    Mathf.Clamp(Camera.main.transform.position.x + direction.x, -xLock, xLock),
                    Mathf.Clamp(Camera.main.transform.position.y + direction.y, -yLock, yLock),
                    Camera.main.transform.position.z);
            }

            Zoom(Input.GetAxis("Mouse ScrollWheel"));
        }
    }

    public void Zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomMin, zoomMax);
    }
}
