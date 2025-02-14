using DefaultNamespace;
using UnityEngine;

public class EnemyController : Actor
{
    [SerializeField] private float downSpeed = 1.0f;
    private bool _movingRight = true; // Whether the enemy is moving right

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileDamage;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileSpawnDelay;

    [SerializeField] private Material weakEnemyMaterial;
    [SerializeField] private Material mediumEnemyMaterial;
    [SerializeField] private Material strongEnemyMaterial;

    private AudioSource audioSource;
    private LevelManager levelManager;
    private GameObject player;

    private EnemyType type;
    private bool isAlive = true;
    private BoxCollider _boxCollider;
    private float _delay;
    
    
    void Awake()
    {
        _boxCollider = GetComponentInChildren<BoxCollider>();
        if (_boxCollider == null)
        {
            Debug.LogError("CapsuleCollider not found in any child object.");
        }
        
        _delay = 0;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on Enemy GameObject.");
        }
        
        levelManager = FindFirstObjectByType<LevelManager>();
        
        player = GameObject.FindWithTag("Player");
        
        SetUpMaxHealth();
    }
    
    void Update()
    {
        Move();
        HandleShooting();
    }
    
    public void Initialize(EnemyType enemyType, float newMaxHealth, float newCollisionDamage, float newProjectileDamage)
    {
        maxHealth = newMaxHealth;
        collisionDamage = newCollisionDamage;
        projectileDamage = newProjectileDamage;
        
        ChangeEnemyMaterial(enemyType);
        
        Debug.Log("CREATED " + enemyType);
    }
    
    void Move()
    {
        Vector3 pos = transform.position;

        switch (type)
        {
            case EnemyType.WeakEnemy:
                // WeakEnemy: Move left/right until hitting the edge, then reverse direction
                MoveWeakEnemy(ref pos);
                break;
            case EnemyType.MediumEnemy:
                // MediumEnemy: Move left/right, and move down when hitting an edge
                MoveMediumEnemy(ref pos);
                break;
            case EnemyType.StrongEnemy:
                // StrongEnemy: Constant movement in the -Z direction + rotation to aim at player
                MoveStrongEnemy(ref pos);
                break;
        }
        
        transform.position = pos;
        
        if (EnvironmentProps.Instance.EscapedDown(transform.position))
        {
            Destroy(this.gameObject);
        }
    }
    
    void MoveWeakEnemy(ref Vector3 pos)
    {
        Vector3 velocity = _movingRight ? Vector3.right * speed : Vector3.left * speed;
        pos = GameUtils.Instance.ComputeEulerStep(pos, velocity, Time.deltaTime);
        pos = EnvironmentProps.Instance.IntoAreaEnemy(pos, _boxCollider.size);

        if (pos == transform.position)
        {
            _movingRight = !_movingRight;
        }
    }
    
    void MoveMediumEnemy(ref Vector3 pos)
    {
        Vector3 velocity = _movingRight ? Vector3.right * speed : Vector3.left * speed;
        pos = GameUtils.Instance.ComputeEulerStep(pos, velocity, Time.deltaTime);
        pos = EnvironmentProps.Instance.IntoAreaEnemy(pos, _boxCollider.size);

        if (pos == transform.position)
        {
            _movingRight = !_movingRight;
            pos.z -= downSpeed;
        }
    }

    void MoveStrongEnemy(ref Vector3 pos)
    {
        var rotationSpeed = 30f;

        pos.z -= (speed / 2) * Time.deltaTime; // slower

        if (player)
        {
            Vector3 directionToPlayer = player.transform.position - transform.position;
            directionToPlayer.y = 0;

            if (directionToPlayer.magnitude > 0.1f)
            {
                directionToPlayer.Normalize();
            
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
            transform.position += transform.forward * (speed * Time.deltaTime);
        }

        pos.y = transform.position.y;
        
        transform.position = pos;
    }

    
    void HandleShooting()
    {
        if (!isAlive)
            return;
        
        // time elapsed from previous frame
        _delay -= Time.deltaTime;
        if (_delay > 0.0f)
            return;

        var enemyPosition = transform.position;
        ShootProjectile(enemyPosition);
    }

    void ShootProjectile(Vector3 enemyPosition)
    {
        var projectile = Instantiate(projectilePrefab, enemyPosition, Quaternion.identity);
        var projectileContr =  projectile.GetComponent<EnemyProjectileController>();
        if (projectileContr)
        {
            if (projectileSpeed != 0)
            {
                projectileContr.SetSpeed(projectileSpeed);
                projectileContr.SetCollisionDamage(projectileDamage);
                
                Vector3 directionToPlayer = player.transform.position - enemyPosition;
                projectileContr.SetDirection(directionToPlayer);
            }
        }
        else
        {
            Debug.LogError("Missing ProjectileController component");
        }
        
        _delay = projectileSpawnDelay;
    }
    
    protected override void OnCollisionEnter(Collision collision)
    {
        Actor collidedActor = collision.gameObject.GetComponent<Actor>();
        if (collidedActor == null) return;
        
        // Deal damage based on the collision damage from the other object
        Debug.Log("Hit by: " + collidedActor.gameObject.name + " Damage: " + collidedActor.GetCollisionDamage());
        float actorCollisionDamage = collidedActor.GetCollisionDamage();
        
        // Calculate Credit + Score bonus
        if (collidedActor.gameObject.CompareTag("Player")) // Crash
        {
            Currencies.Instance.OnCollisionCurrenciesEnemy(actorCollisionDamage, true, false);
        }
        else if (collidedActor.gameObject.CompareTag("PlayerProjectile"))
        {
            Currencies.Instance.OnCollisionCurrenciesEnemy(actorCollisionDamage, false,
                currentHealth - actorCollisionDamage <= 0);
        }
        
        ReceiveDamage(actorCollisionDamage);
    }
    
    private void ChangeEnemyMaterial(EnemyType enemyType)
    {
        this.type = enemyType;
        MeshRenderer meshRenderer = transform.Find("Visuals/stingray")?.GetComponent<MeshRenderer>();
        if (!meshRenderer)
        {
            Debug.LogWarning("MeshRenderer component on 'stingray' not found!");
            return;
        }

        meshRenderer.material = enemyType switch
        {
            EnemyType.WeakEnemy => weakEnemyMaterial,
            EnemyType.MediumEnemy => mediumEnemyMaterial,
            EnemyType.StrongEnemy => strongEnemyMaterial,
            _ => throw new System.ArgumentOutOfRangeException(nameof(enemyType), "Unknown enemy type")
        };
    }
    
    protected override void HandleDeath()
    {
        if (currentHealth > 0) 
            return;
        isAlive = false;
        if (levelManager)
        {
            levelManager.KilledEnemy();
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