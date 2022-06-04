using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinSelectionTouchController : MonoBehaviour
{

    private float width;
    private float height;
    Touch touch;
    private Vector2 pos;

    public SkinSelectionController skinSelectionController;

    void Awake()
    {
        width = (float)Screen.width / 2.0f;
        height = (float)Screen.height / 2.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        skinSelectionController = GetComponent<SkinSelectionController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            pos = touch.position;
            pos.x = (pos.x - width) / width;
            pos.y = (pos.y - height) / height;

            if (touch.phase == TouchPhase.Began)
            {
                if (pos.x < 0.2f)
                {
                    skinSelectionController.NextHero();
                }
                if (pos.x > -0.2f)
                {
                    skinSelectionController.LastHero();
                }

                if (pos.x < 0.2f && pos.x > -0.2f)
                {
                    //skinSelectionController.StartGame();
                }
            }
        }
    }
}
