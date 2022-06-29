using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Managing;
using FishNet;
using FishNet.Object;

public class PhysicsResolver : MonoBehaviour
{
    /// <summary>
    /// Accumulated physics step.
    /// </summary>
    private float _stepTime = 0f;
    /// <summary>
    /// PhysicsScene this object is in. Required for scene stacking.
    /// </summary>
    private PhysicsScene2D _physicsScene;

    private void Awake()
    {
        /* //Note In 2020+ I believe this is
         * Physics.SimulationMode = script; */

        Physics2D.simulationMode = SimulationMode2D.Script;
        _physicsScene = gameObject.scene.GetPhysicsScene2D();
        Debug.Log(_physicsScene);
    }

    private void Update()
    {
        float fixedDelta = Time.fixedDeltaTime;
        _stepTime += Time.deltaTime;
        while (_stepTime >= fixedDelta)
        {
            _stepTime -= fixedDelta;
            _physicsScene.Simulate(fixedDelta);
        }
    }
}
