using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    public bool moveRight = false;
    public bool moveLeft = false;
    public bool jump = false;
    public bool touchUp = false;

    private Vector2 oldTouchPosition;

    private Vector2 touchStart; 

    private Vector2 pos;
    private Vector2 pos2;
    private float width;
    private float height;

    Touch touch;
    Touch touch2;

    private PlayerMovement movement;

    void Awake()
    {
        width = (float)Screen.width / 2.0f;
        height = (float)Screen.height / 2.0f;
    }

    private void Start()
    {
        movement = GetComponent<PlayerMovement>();
    }

    void OnGUI()
    {
        // Compute a fontSize based on the size of the screen width.
        //GUI.skin.label.fontSize = (int)(Screen.width / 25.0f);

        /*        GUI.Label(new Rect(20, 20, width, height * 0.25f),
                    "x = " + pos.x.ToString("f2") +
                    ", y = " + pos.y.ToString("f2"));
          */

        // GUI.Label(new Rect(20, 20, width, height * 0.25f), onLeftScreen.ToString());

    }

    void Update()
    {
        // Handle screen touches.
        if (Input.touchCount > 0)
        {
            TouchCode(1);
        }

        if (Input.touchCount == 2)
        {
            TouchCode(2);
        }
    }

    private void TouchCode(int touchCount){
        touch = Input.GetTouch(touchCount - 1);
        pos = touch.position;
        pos.x = (pos.x - width) / width;
        pos.y = (pos.y - height) / height;

        if (pos.x < .4f)
        {
            RunCode(touch);
        }


        if (touch.phase == TouchPhase.Ended)
        {
            moveRight = false;
            moveLeft = false;
        }
    }

   

    private void RunCode(Touch Touch)
    {
        if (Touch.phase == TouchPhase.Began)
        {
            touchStart = Touch.position;
        }

        if (Touch.phase == TouchPhase.Ended)
        {
            moveRight = false;
            moveLeft = false;
        }

        if (Touch.phase == TouchPhase.Moved)
        {
            if ((Touch.position.x < touchStart.x))
            {
                moveLeft = true;
                moveRight = false;
            }
            if ((Touch.position.x > touchStart.x))
            {
                moveRight = true;
                moveLeft = false;
            }
        }
    }
}
