using UnityEngine;

namespace FirstGearGames.LobbyAndWorld.Demos.KingOfTheHill
{


    /// <summary>
    /// Simulates phyics for the current scene.
    /// </summary>
    public class SimulatePhysics : MonoBehaviour
    {
        /// <summary>
        /// Accumulated physics step.
        /// </summary>
        private float _stepTime = 0f;
        /// <summary>
        /// PhysicsScene this object is in. Required for scene stacking.
        /// </summary>
        private PhysicsScene _physicsScene;

        private void Awake()
        {
            /* //Note In 2020+ I believe this is
             * Physics.SimulationMode = script; */
            Physics.autoSimulation = false;
            _physicsScene = gameObject.scene.GetPhysicsScene();
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


}