using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Managing;
using FishNet;
using FishNet.Object;
using FishNet.Managing.Timing;


public class PhysicsResolver : MonoBehaviour
{

    /// <summary>
    /// PhysicsScene this object is in. Required for scene stacking.
    /// </summary>
    private PhysicsScene2D _physicsScene;
    /// <summary>
    /// TimeManager subscribed to.
    /// </summary>
    private TimeManager _tm;

    private void Awake()
    {
        _tm = InstanceFinder.TimeManager;
        _tm.OnPhysicsSimulation += TimeManager_OnPhysicsSimulation;
        _physicsScene = gameObject.scene.GetPhysicsScene2D();

        //Let this script simulate physics.
        Physics.autoSimulation = false;
#if !UNITY_2020_2_OR_NEWER
            Physics2D.autoSimulation = false;
#else
        Physics2D.simulationMode = SimulationMode2D.Script;
#endif
    }

    private void OnDestroy()
    {
        if (_tm != null)
            _tm.OnPhysicsSimulation -= TimeManager_OnPhysicsSimulation;
    }

    private void TimeManager_OnPhysicsSimulation(float delta)
    {
        _physicsScene.Simulate(delta);
    }

}
