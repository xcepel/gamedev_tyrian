using UnityEngine;

public class GameUtils : MonoBehaviour
{
    public static GameUtils Instance { get; private set; }
    
    public void Awake()
    {
        // Check, if we do not have any instance yet.
        if (Instance == null)
        {
            // 'this' is the first instance created => save it.
            Instance = this;
        }
        else if (Instance != this)
        {
            // Destroy 'this' object as there exist another instance
            Destroy(this.gameObject);
        }
    }
    
    public Vector3 ComputeEulerStep(Vector3 f0, Vector3 df0_dt, float delta_t)
    {
        return f0 + delta_t * df0_dt;
    }
    
    public Vector3 ComputeSeekAcceleration(Vector3 pos, float maxAccel, Vector3 targetPos)
    {
        Vector3 dir = targetPos - pos;
        if (dir.magnitude < 1e-3f)
            return Vector3.zero;
        // Seek only if not too close.
        return maxAccel * (dir / dir.magnitude);
    }
    
    public Vector3 ComputeSeekVelocity(
        Vector3 pos, Vector3 velocity,
        float maxSpeed, float maxAccel,
        Vector3 targetPos, float dt
    )
    {
        Vector3 seekAccel = ComputeSeekAcceleration(pos, maxAccel, targetPos);
        velocity = ComputeEulerStep(velocity, seekAccel, dt);
        // And we must also apply the clipping.
        if (velocity.magnitude > maxSpeed)
            velocity *= (maxSpeed / velocity.magnitude);
        return velocity;
    }
}