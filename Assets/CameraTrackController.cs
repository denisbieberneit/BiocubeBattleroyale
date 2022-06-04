using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrackController : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    private Transform targetTransform;
    public float smoothSpeed = 0.125f;
    private int currentTarget = 2;

    private void Start()
    {
        targetTransform = gameObject.transform.GetChild(currentTarget);
    }

    void FixedUpdate()
    {
        if (Mathf.Round(cam.transform.position.x) == Mathf.Round(targetTransform.position.x) && Mathf.Round(cam.transform.position.y) == Mathf.Round(targetTransform.position.y))
        {
            currentTarget++;
            if (currentTarget == 39){
                currentTarget = 1;
            }
            targetTransform = gameObject.transform.GetChild(currentTarget);
        }
        Vector2 desiredPosition = new Vector2(targetTransform.position.x, targetTransform.position.y);
        Vector2 smoothedPosition = Vector2.MoveTowards(cam.transform.position, desiredPosition, smoothSpeed);
        cam.transform.position = smoothedPosition;
    }
}
