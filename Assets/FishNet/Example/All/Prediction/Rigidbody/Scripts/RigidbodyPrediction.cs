﻿using FishNet;
using FishNet.Object;
using FishNet.Object.Prediction;
using UnityEngine;

/*
* 
* See TransformPrediction.cs for more detailed notes.
* 
*/

namespace FishNet.Example.Prediction.Rigidbodies
{

    public class RigidbodyPrediction : NetworkBehaviour
    {
        #region Types.
        public struct MoveData
        {
            public bool Jump;
            public float Horizontal;
            public float Vertical;
            public MoveData(bool jump, float horizontal, float vertical)
            {
                Jump = jump;
                Horizontal = horizontal;
                Vertical = vertical;
            }
        }
        public struct ReconcileData
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Velocity;
            public Vector3 AngularVelocity;
            public ReconcileData(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
            {
                Position = position;
                Rotation = rotation;
                Velocity = velocity;
                AngularVelocity = angularVelocity;
            }
        }
        #endregion

        #region Serialized.
        [SerializeField]
        private float _jumpForce = 15f;
        [SerializeField]
        private float _moveRate = 15f;
        #endregion

        #region Private.
        /// <summary>
        /// Rigidbody on this object.
        /// </summary>
        private Rigidbody2D _rigidbody;
        /// <summary>
        /// Next time a jump is allowed.
        /// </summary>
        private float _nextJumpTime;
        /// <summary>
        /// True to jump next frame.
        /// </summary>
        private bool _jump;
        #endregion



        private void Awake()
        {

            _rigidbody = GetComponent<Rigidbody2D>();
            InstanceFinder.TimeManager.OnTick += TimeManager_OnTick;
            InstanceFinder.TimeManager.OnPostTick += TimeManager_OnPostTick;
        }

        private void OnDestroy()
        {
            if (InstanceFinder.TimeManager != null)
            {
                InstanceFinder.TimeManager.OnTick -= TimeManager_OnTick;
                InstanceFinder.TimeManager.OnPostTick -= TimeManager_OnPostTick;
            }
        }

        private void Update()
        {
           /* if (base.IsOwner)
            {
                if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextJumpTime)
                {
                    _nextJumpTime = Time.time + 1f;
                    _jump = true;
                }
            }
           */
        }

        private void TimeManager_OnTick()
        {
            if (base.IsOwner)
            {
                Reconciliation(default, false);
                CheckInput(out MoveData md);
                Move(md, false);
            }
            if (base.IsServer)
            {
                Move(default, true);
            }
        }


        private void TimeManager_OnPostTick()
        {
            if (base.IsServer)
            {
                ReconcileData rd = new ReconcileData(transform.position, transform.rotation, _rigidbody.velocity, Vector3.one);
                Reconciliation(rd, true);
            }
        }

        private void CheckInput(out MoveData md)
        {
            md = default;

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            if (horizontal == 0f && vertical == 0f && !_jump)
                return;

            md = new MoveData(_jump, horizontal, vertical);
            _jump = false;
        }

        [Replicate]
        private void Move(MoveData md, bool asServer, bool replaying = false)
        {
            //Add extra gravity for faster falls.
            Vector3 forces = new Vector3(md.Horizontal, Physics.gravity.y, md.Vertical) * _moveRate;
            _rigidbody.AddForce(forces);


            // Multiply the player's x local scale by -1.
            if (md.Horizontal >= 1)
            {
                Vector3 theScale = transform.localScale;
                theScale.x *= -1;
                transform.localScale = theScale;

            }

            if (md.Jump)
                _rigidbody.AddForce(new Vector3(0f, _jumpForce, 0f));
        }

        [Reconcile]
        private void Reconciliation(ReconcileData rd, bool asServer)
        {
            transform.position = rd.Position;
            transform.rotation = rd.Rotation;
            _rigidbody.velocity = rd.Velocity;
            _rigidbody.angularVelocity = 1f;
        }


    }


}