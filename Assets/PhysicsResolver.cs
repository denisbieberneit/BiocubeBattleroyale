using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Managing;
using FishNet;
using FishNet.Object;

public class PhysicsResolver : MonoBehaviour
{
  
    private float _stepTime = 0f;
    private PhysicsScene2D _physicsScene;

    private void Awake()
    {
        Physics2D.simulationMode = SimulationMode2D.Script;
        _physicsScene = gameObject.scene.GetPhysicsScene2D();
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
