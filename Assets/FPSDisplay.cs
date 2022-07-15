using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FishNet.Managing;
using FishNet;
using FishNet.Object;

public class FPSDisplay : NetworkBehaviour
{
    private int avgFrameRate;
    public TMP_Text display_fps;
    public TMP_Text display_ping;

    private void Start()
    {
        InvokeRepeating("GetFps", 1, 1);
        InvokeRepeating("GetPing", 1, 1);

    }

    public void GetFps()
    {
        avgFrameRate = (int)(1f / Time.unscaledDeltaTime);
        display_fps.text = "FPS: " + avgFrameRate.ToString();
    }

    private void GetPing()
    {
        NetworkManager nm = InstanceFinder.NetworkManager;
        if (nm != null && nm.IsClient)
            display_ping.text = $"Ping: {nm.TimeManager.RoundTripTime}";
    }
}
