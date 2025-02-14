using UnityEngine;

public class EnemyProjectileController : Actor
{
    private float radius = 0.25f;
    private Vector3 direction;
    
    private void Start()
    {
        SetUpMaxHealth();
        direction = new Vector3(0, 0, -1);
    }

    void Update()
    {
        transform.position += direction * (speed * Time.deltaTime);
        
        // Destroy on boarder
        if (EnvironmentProps.Instance.EscapedDown(transform.position, radius))
        {
            Destroy(this.gameObject);
        }
    }
    
    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
    }
}