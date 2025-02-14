using System;
using UnityEngine;

public class PlayerProjectileController : Actor
{
    private float radius = 2f;

    private void Start()
    {
        SetUpMaxHealth();
    }

    void Update()
    {
        // move projectile up
        transform.position -= new Vector3(0, 0, - speed * Time.deltaTime);
        
        // destroy it on border
        if (EnvironmentProps.Instance.EscapedUp(transform.position, radius))
        {
            Destroy(this.gameObject);
        }
    }
}
