using UnityEngine;

public class BossConroller : Actor
{
    enum State
    {
        ENTER_GAME_ZONE,
        LONG_ATTACK,
        CLOSE_ATTACK,
        RETREAT,
        OFF
    }
    
    private AudioSource audioSource;
    private LevelManager levelManager;
    
    private bool isActive = false;
    
    [SerializeField] private GameObject projectilePrefab;
    
    public float maxSpeed = 8;
    public float maxAccel = 25;
    public Vector3 velocity = Vector3.zero;
    private BoxCollider boxCollider;
    public Transform targetTransform;
    
    private State activeState = State.OFF;
    public float minReactionDelay = 0.1f;
    public float maxReactionDelay = 0.2f;
    private float reactionDelay = 0.0f;
    private bool gameZoneEntered = false;
    public int NumShotsToCooldown = 7;
    private int numShots = 0;
    
    public float firePointShiftZ = 5.0f;
    private Vector3 enemyPosition = Vector3.zero;
    
    public float ReloadSeconds = 0.3f;
    private float reload = 0.0f;
    public float CooldownSeconds = 5.0f;
    private float cooldown;
    private Transform gun;
    
    private const float powerShootCooldown = 10.0f;
    private float powerShootTimer = 0.0f;
    private const float retreatCooldown = 2.0f;
    private float retreatTimer;
    
    void Awake()
    {
        boxCollider = GetComponentInChildren<BoxCollider>();
    }

    private void Start()
    {
        SetUpMaxHealth();
        
        levelManager = FindFirstObjectByType<LevelManager>();
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on Enemy GameObject.");
        }
        
        gun = transform.Find("Gun");
        cooldown = CooldownSeconds;
    }

    void Update()
    {
        if (activeState == State.OFF)
        {
            if (isActive)
            {
                activeState = State.ENTER_GAME_ZONE;
            }
            else
            {
                return;
            }
        }
        
        reactionDelay -= Time.deltaTime;
        if (reactionDelay <= 0.0f)
        {
            reactionDelay = Random.Range(minReactionDelay, maxReactionDelay);
            Perception();
            SelectState();
        }
        ProcessGunTimers();
        if (powerShootTimer <= 0.0f && activeState != State.ENTER_GAME_ZONE)
        {
            PowerShoot();
        }
        
        switch (activeState)
        {
            case State.ENTER_GAME_ZONE:
                Process_ENTER_GAME_ZONE();
                break;
            case State.LONG_ATTACK:
                Process_LONG_ATTACK();
                break;
            case State.CLOSE_ATTACK:
                Process_CLOSE_ATTACK();
                break;
            case State.RETREAT:
                Process_RETREAT();
                break;
            case State.OFF:
                Process_OFF();
                break;
            default:
                Debug.Assert(false);
                break;
        }
    }
    
    private void Perception()
    {
        enemyPosition = targetTransform.position;
    }
    
    private void SelectState()
    {
        switch (activeState)
        {
            case State.ENTER_GAME_ZONE:
                if (gameZoneEntered)
                    activeState = State.LONG_ATTACK;
                break;
            case State.LONG_ATTACK:
                if (numShots >= NumShotsToCooldown)
                    activeState = State.CLOSE_ATTACK;
                break;
            case State.CLOSE_ATTACK:
                if (numShots < NumShotsToCooldown)
                {
                    activeState = State.RETREAT;
                    retreatTimer = retreatCooldown;
                }
                break;
            case State.RETREAT:
                if (retreatTimer <= 0)
                    activeState = State.LONG_ATTACK;
                break;
            default: Debug.Assert(false); break;
        }
    }
    
    private void Process_ENTER_GAME_ZONE()
    {
        EnvironmentProps env = EnvironmentProps.Instance;
        Vector3 target = new Vector3(
            0.5f * (env.MinX() + env.MaxX()),
            0.0f,
            env.MinZ() + 0.75f * (env.MaxZ() - env.MinZ())
        );
        velocity = GameUtils.Instance.ComputeSeekVelocity(
            transform.position, velocity,
            maxSpeed, maxAccel,
            target, Time.deltaTime);
        transform.position = GameUtils.Instance.ComputeEulerStep(
            transform.position, velocity, Time.deltaTime);
        if ((target - transform.position).magnitude < 1.0f)
            gameZoneEntered = true;
    }
    
    private void Process_LONG_ATTACK()
    {
        EnvironmentProps env = EnvironmentProps.Instance;
        Vector3 target = new Vector3(
            env.MinX() +
            (enemyPosition.x < transform.position.x ? 0f : 1f) *
            (env.MaxX() - env.MinX()),
            0,
            env.MinZ() + 0.8f * (env.MaxZ() - env.MinZ())
        );
        velocity = GameUtils.Instance.ComputeSeekVelocity(
            transform.position, velocity,
            maxSpeed, maxAccel,
            target, Time.deltaTime);
        Vector3 pos = GameUtils.Instance.ComputeEulerStep(
            transform.position, velocity, Time.deltaTime);
        transform.position = EnvironmentProps.Instance.IntoArea(
            pos, 0.5f * boxCollider.size.x, 0.5f * boxCollider.size.z);
        Shoot();
    }
    
    private void Process_RETREAT()
    {
        retreatTimer -= Time.deltaTime;
        
        EnvironmentProps env = EnvironmentProps.Instance;
        Vector3 target = new Vector3(
            env.MinX() +
            (enemyPosition.x < transform.position.x ? 0f : 1f) *
            (env.MaxX() - env.MinX()),
            0,
            env.MinZ() + 0.8f * (env.MaxZ() - env.MinZ())
        );
        velocity = GameUtils.Instance.ComputeSeekVelocity(
            transform.position, velocity,
            maxSpeed, maxAccel,
            target, Time.deltaTime);
        Vector3 pos = GameUtils.Instance.ComputeEulerStep(
            transform.position, velocity, Time.deltaTime);
        transform.position = EnvironmentProps.Instance.IntoArea(
            pos, 0.5f * boxCollider.size.x, 0.5f * boxCollider.size.z);
    }
    
    private void Process_CLOSE_ATTACK()
    {
        velocity = GameUtils.Instance.ComputeSeekVelocity(
            transform.position, velocity,
            maxSpeed, maxAccel,
            enemyPosition + new Vector3(0, 0, firePointShiftZ),
            Time.deltaTime
        );
        Vector3 pos = GameUtils.Instance.ComputeEulerStep(
            transform.position, velocity, Time.deltaTime);
        transform.position = EnvironmentProps.Instance.IntoArea(
            pos, 0.5f * boxCollider.size.x, 0.5f * boxCollider.size.z);
    }
    
    private void Process_OFF()
    {
        // The boss does nothing in the OFF state
    }
    
    private void ProcessGunTimers()
    {
        if (numShots == NumShotsToCooldown)
        {
            cooldown -= Time.deltaTime;
            if (cooldown <= 0.0f)
            {
                cooldown = CooldownSeconds;
                reload = 0.0f;
                numShots = 0;
            }
        }
        else if (reload > 0.0f)
        {
            reload -= Time.deltaTime;
        }
        
        powerShootTimer -= Time.deltaTime;
    }
    
    private void Shooter(Vector3 target)
    {
        Vector3 horizontalVelocity = Vector3.Dot(velocity, Vector3.right) * Vector3.right;
    
        GameObject projectileObject = Instantiate(projectilePrefab, gun.position, gun.rotation);
    
        BossProjectile projectile = projectileObject.GetComponent<BossProjectile>();
    
        if (projectile)
        {
            projectile.SetVelocity(horizontalVelocity, target);
        }
    }
    
    private void Shoot()
    {
        if (reload <= 0.0f)
        {
            Shooter(new Vector3(0, 0, -1));

            ++numShots;
            reload = ReloadSeconds;
        }
    }
    
    private void PowerShoot()
    {
        Shooter(new Vector3(-1, 0, -1));
        Shooter(new Vector3(-1, 0, 0));
        Shooter(new Vector3(-1, 0, 1));
        Shooter(new Vector3(0, 0, -1));
        Shooter(new Vector3(0, 0, 1));
        Shooter(new Vector3(1, 0, -1));
        Shooter(new Vector3(1, 0, 0));
        Shooter(new Vector3(1, 0, 1));
        
        powerShootTimer = powerShootCooldown;
    }
    
    protected override void OnCollisionEnter(Collision collision)
    {
        Actor collidedActor = collision.gameObject.GetComponent<Actor>();
        
        // Cant be killed by crash - bites
        if (collidedActor == null || collidedActor.gameObject.CompareTag("Player")) return;
        
        float actorCollisionDamage = collidedActor.GetCollisionDamage();
        if (collidedActor.gameObject.CompareTag("PlayerProjectile"))
        {
            Currencies.Instance.OnCollisionCurrenciesBoss(false, actorCollisionDamage);
        }
        
        // Deal damage based on the collision damage from the other object
        Debug.Log("Hit by: " + collidedActor.gameObject.name + " Damage: " + collidedActor.GetCollisionDamage());
        ReceiveDamage(collidedActor.GetCollisionDamage());
    }
    
    protected override void HandleDeath()
    {
        if (currentHealth > 0) 
            return;
        Currencies.Instance.OnCollisionCurrenciesBoss(true);
        
        if (levelManager)
        {
            levelManager.KilledBoss();
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

    public void ActivateBoss()
    {
        isActive = true;
    }
}
