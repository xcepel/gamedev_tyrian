using System;
using UnityEngine;

public class Player : Actor
{
    public event Action OnPlayerDeath;
    private CapsuleCollider _capsuleCollider;
    
    [SerializeField] private FixedJoystick joystick;
    
    void Awake()
    {
        _capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        if (_capsuleCollider == null)
        {
            Debug.LogError("CapsuleCollider not found in any child object.");
        }
    }
    
    private void Start()
    {
        if (ShopManager.Instance.BoughtDurability())
        {
            maxHealth += 20;
        }
        if (ShopManager.Instance.BoughtMovementSpeed())
        {
            speed += 10;
        }
        
        SetUpMaxHealth();

        Invoke(nameof(DestroyAnimator), 1f);
    }
    
    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 pos = transform.position;

        Vector3 input = GetInput();
        Vector3 velocity = input.normalized * speed;

        pos = GameUtils.Instance.ComputeEulerStep(pos, velocity, Time.deltaTime);
        pos = EnvironmentProps.Instance.IntoAreaSphere(pos, _capsuleCollider.radius);

        transform.position = pos;
    }
    
    private Vector3 GetInput()
    {
        Vector3 input = Vector3.zero;

        // PC input (keyboard)
        #if UNITY_STANDALONE || UNITY_EDITOR
            if (Input.GetKey(KeyCode.A))
                input.x -= 1;
            if (Input.GetKey(KeyCode.D))
                input.x += 1;
            if (Input.GetKey(KeyCode.S))
                input.z -= 1;
            if (Input.GetKey(KeyCode.W))
                input.z += 1;
        #endif

        // Mobile input (joystick)
        #if UNITY_ANDROID || UNITY_IOS
            input.x = joystick.Horizontal;
            input.z = joystick.Vertical;
        #endif

        return input;
    }
    
    protected override void HandleDeath()
    {
        if (currentHealth > 0) 
            return;
        OnPlayerDeath?.Invoke();
        Destroy(gameObject);
    }
    
    protected override void ReceiveDamage(float damage)
    {
        // Check for immortality
        if (CheatsManager.Instance != null && CheatsManager.Instance.IsImmortal())
        {
            Debug.Log("player AVOIDED damage");
            return;
        }
        Debug.Log("player received damage");
        CurrentHealth -= damage;
    }
    
    private void DestroyAnimator()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            Destroy(animator);
            Debug.Log("Animator destroyed after 1 second.");
        }
    }
}
