using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    // Set all game settings here such as targetFramerate
    void Start()
    {
        Application.targetFrameRate = 60;
    }
}
