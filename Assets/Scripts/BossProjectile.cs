using UnityEngine;

public class BossProjectile : Actor
{
    private float radius = 0.25f;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        SetUpMaxHealth();
    }

    public void SetVelocity(Vector3 gunVelocity, Vector3 gunUnitAimingDir)
    {
        //gunUnitAimingDir.Normalize();
        
        velocity = gunVelocity + speed * gunUnitAimingDir;
    }

    private void Update()
    {
        //transform.position += new Vector3(0, 0, - speed * Time.deltaTime);
        transform.position += velocity.normalized * (speed * Time.deltaTime);
        
        if (EnvironmentProps.Instance.EscapedDown(transform.position, radius))
        {
            Destroy(gameObject);
        }
    }
}