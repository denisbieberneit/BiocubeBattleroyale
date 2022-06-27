/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using UnityEngine;
using FishNet.Object;
using FishNet;
using FishNet.Object.Synchronizing;

public class DamageCircle : NetworkBehaviour {

    private static DamageCircle instance;

    [SerializeField] private Transform targetCircleTransform;

    [SerializeField] private Transform circleTransform;
    [SerializeField] private Transform topTransform;
    [SerializeField] private Transform bottomTransform;
    [SerializeField] private Transform leftTransform;
    [SerializeField] private Transform rightTransform;

    [SyncVar] public float circleShrinkSpeed;
    [SyncVar] public float waitTime;
    [SyncVar] public float shrinkAmount;

    [SyncVar] private float shrinkTimer;

    [SyncVar] private Vector3 circleSize;
    [SyncVar] private Vector3 circlePosition;

    [SyncVar] private Vector3 targetCircleSize;
    [SyncVar] private Vector3 targetCirclePosition;


    private void Start() {
        instance = this;
        InstanceFinder.TimeManager.OnTick += TimeManager_OnTick;
        SetCircleSize(new Vector3(18.5f, 15f), new Vector3(100, 100));
        SetTargetCircle(new Vector3(18.5f, 15f), new Vector3(100, 100), waitTime);

    }

    private void TimeManager_OnTick() {
        if (!base.IsServer)
        {
            return;
        }
        shrinkTimer -= ((float)(base.TimeManager.TickDelta));

        if (shrinkTimer < 0) {
            Vector3 sizeChangeVector = (targetCircleSize - circleSize).normalized;
            Vector3 newCircleSize = circleSize + sizeChangeVector * ((float)(base.TimeManager.TickDelta)) * circleShrinkSpeed;

            Vector3 circleMoveDir = (targetCirclePosition - circlePosition).normalized;
            Vector3 newCirclePosition = circlePosition + circleMoveDir * ((float)(base.TimeManager.TickDelta)) * circleShrinkSpeed;

            SetCircleSize(newCirclePosition, newCircleSize);

            float distanceTestAmount = .1f;
            if (Vector3.Distance(newCircleSize, targetCircleSize) < distanceTestAmount && Vector3.Distance(newCirclePosition, targetCirclePosition) < distanceTestAmount) {
                GenerateTargetCircle();
            }
        }
    }

    private void GenerateTargetCircle() {
        if (!base.IsServer)
        {
            return;
        }
        float shrinkSizeAmount = shrinkAmount;
        Vector3 generatedTargetCircleSize = circleSize - new Vector3(shrinkSizeAmount, shrinkSizeAmount) * 2f;

        // Set a minimum size
        if (generatedTargetCircleSize.x < 20f) generatedTargetCircleSize = Vector3.one * 20f;

        Vector3 generatedTargetCirclePosition = circlePosition + 
            new Vector3(Random.Range(-shrinkSizeAmount, shrinkSizeAmount), Random.Range(-shrinkSizeAmount, shrinkSizeAmount));

        float shrinkTime = waitTime;

        SetTargetCircle(generatedTargetCirclePosition, generatedTargetCircleSize, shrinkTime);
    }

    private void SetCircleSize(Vector3 position, Vector3 size) {
        if (!base.IsServer)
        {
            return;
        }
        circlePosition = position;
        circleSize = size;

        transform.position = position;

        circleTransform.localScale = size;

        topTransform.localScale = new Vector3(1000, 1000);
        topTransform.localPosition = new Vector3(0, topTransform.localScale.y * .5f + size.y * .5f);
        
        bottomTransform.localScale = new Vector3(1000, 1000);
        bottomTransform.localPosition = new Vector3(0, -topTransform.localScale.y * .5f - size.y * .5f);

        leftTransform.localScale = new Vector3(1000, size.y);
        leftTransform.localPosition = new Vector3(-leftTransform.localScale.x * .5f - size.x * .5f, 0f);

        rightTransform.localScale = new Vector3(1000, size.y);
        rightTransform.localPosition = new Vector3(+leftTransform.localScale.x * .5f + size.x * .5f, 0f);
    }

    private void SetTargetCircle(Vector3 position, Vector3 size, float shrinkTimer) {
        if (!base.IsServer)
        {
            return;
        }
        this.shrinkTimer = shrinkTimer;

        targetCircleTransform.position = position;
        targetCircleTransform.localScale = size;
        
        targetCirclePosition = position;
        targetCircleSize = size;
    }

    private bool IsOutsideCircle(Vector3 position) { 
        return Vector3.Distance(position, circlePosition) > circleSize.x * .5f;
    }

    public static bool IsOutsideCircle_Static(Vector3 position) {
        return instance.IsOutsideCircle(position);
    }

    private void OnDestroy()
    {
        if (InstanceFinder.TimeManager != null)
        {
            InstanceFinder.TimeManager.OnTick -= TimeManager_OnTick;
        }
    }
}
