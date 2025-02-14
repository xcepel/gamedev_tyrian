using UnityEngine;

public class Actor : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;

    [SerializeField] protected float maxHealth;
    [SerializeField] protected float collisionDamage;
    [SerializeField] protected float speed;
    protected float currentHealth;

    protected void SetUpMaxHealth()
    {
        CurrentHealth = maxHealth;
    }

    protected float CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = value;
            
            // Only update the health bar if it exists
            if (healthBar != null)
                healthBar.SetFillLevel(currentHealth / maxHealth);

            HandleDeath();
        }
    }

    protected virtual void HandleDeath()
    {
        if (currentHealth > 0) 
            return;
        Destroy(gameObject);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void SetCollisionDamage(float newDamage)
    {
        collisionDamage = newDamage;
    }
    
    protected virtual void OnCollisionEnter(Collision collision)
    {
        Actor collidedActor = collision.gameObject.GetComponent<Actor>();
        if (collidedActor == null) return;
        
        // Deal damage based on the collision damage from the other object
        Debug.Log("Hit by: " + collidedActor.gameObject.name + " Damage: " + collidedActor.GetCollisionDamage());
        ReceiveDamage(collidedActor.GetCollisionDamage());
    }
    
    public float GetCollisionDamage()
    {
        return this.collisionDamage;
    }
    
    protected virtual void ReceiveDamage(float damage)
    {
        CurrentHealth -= damage;
    }
    
    public void Heal(float heal)
    {
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + heal);
    }
    
    public void Kill()
    {
        CurrentHealth = 0;
        Debug.Log("You died!");
    }
}
