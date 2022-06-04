using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    public bool isSkill;
    public bool isAttack;


    private new Camera camera;

    private Vector2 skillPos;
    private Vector2 attackPos;

    private void Start()
    {
        camera = Camera.main;
        skillPos = new Vector2(Screen.width * 0.93f, Screen.height * 0.27f);
        attackPos = new Vector2(Screen.width * 0.87f, Screen.height * 0.12f);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (isSkill)
        {

            // This time simply stay at the current distance to camera
            gameObject.transform.position = camera.ScreenToWorldPoint(new Vector3(skillPos.x, skillPos.y, Vector3.Distance(camera.transform.position, transform.position)));
        }

        if (isAttack)
        {

            // This time simply stay at the current distance to camera
            gameObject.transform.position = camera.ScreenToWorldPoint(new Vector3(attackPos.x, attackPos.y, Vector3.Distance(camera.transform.position, transform.position)));
        }
    }

}
