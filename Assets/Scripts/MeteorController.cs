using UnityEngine;

public class MeteorController : Actor
{
    private float radius = 1.0f;

    private AudioSource audioSource;
    private LevelManager levelManager;

    private void Start()
    {
        SetUpMaxHealth();
        
        levelManager = FindFirstObjectByType<LevelManager>();
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on Enemy GameObject.");
        }
        
        //set collider radius same as the meteor
        SphereCollider sphereCollider = GetComponent<SphereCollider>();

        if (sphereCollider != null)
        {
            sphereCollider.radius = (transform.localScale.x / 2);
        }
    }

    void Update()
    {
        MoveMeteor();
    }

    private void MoveMeteor()
    {
        // move meteor down
        transform.position += new Vector3(0, 0, -speed * Time.deltaTime);
        // destroy it on border
        if (EnvironmentProps.Instance.EscapedDown(transform.position, radius))
        {
            Destroy(this.gameObject);
        }
    }

    public void SetMeteor(float newSpeed, float newRadius)
    {
        speed = newSpeed;
        radius = newRadius;
        transform.localScale = new Vector3(radius, radius, radius);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        Actor collidedActor = collision.gameObject.GetComponent<Actor>();
        if (collidedActor == null) return;

        // Deal damage based on the collision damage from the other object
        Debug.Log("Hit by: " + collidedActor.gameObject.name + " Damage: " + collidedActor.GetCollisionDamage());
        float actorCollisionDamage = collidedActor.GetCollisionDamage();

        if (collidedActor.gameObject.CompareTag("PlayerProjectile"))
        {
            Currencies.Instance.OnCollisionCurrenciesMeteor();
        }

        ReceiveDamage(actorCollisionDamage);
    }

    protected override void HandleDeath()
    {
        if (currentHealth > 0)
            return;
    
        if (levelManager)
        {
            levelManager.KilledMeteor();
        }
        
        if (audioSource && audioSource.clip)
        {
            audioSource.Play();

            // hide the object
            Renderer renderer = GetComponentInChildren<Renderer>();
            if (renderer)
            {
                renderer.enabled = false;
            }

            // disable all colliders on the object
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }

            Destroy(gameObject, audioSource.clip.length);
        }
    }
}
