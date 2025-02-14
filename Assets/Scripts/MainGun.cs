using UnityEngine;

public class MainGun : MonoBehaviour
{

    [SerializeField] public GameObject ship;

    // reference to prefab
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float _projectileSpeed;

    private AudioSource audioSource;

    private float MAX_DELAY = 0.25f;

    // delay from last spawn
    private float _delay;
    private bool initialDelay = true; 
    private float animationDelay = 1.0f;

    void Start()
    {
        if (ShopManager.Instance.BoughtFiringRate())
        {
            MAX_DELAY -= 0.10f;
        }
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component is missing on MainGun GameObject.");
        }

        _delay = 0;
    }

    void Update()
    {
        // time elapsed from previous frame
        _delay -= Time.deltaTime;
        if (_delay > 0.0f)
            return;

        if (!ship) return;

        // Delay for animation before starting shooting
        if (initialDelay)
        {
            animationDelay -= Time.deltaTime;
            if (animationDelay > 0.0f) return;
            initialDelay = false; 
        }
        
        var shipPosition = ship.transform.position;
        ShootProjectile(shipPosition);
    }

    void ShootProjectile(Vector3 shipPosition)
    {
        var projectile = Instantiate(projectilePrefab, shipPosition, Quaternion.identity);
        var projectileContr = projectile.GetComponent<PlayerProjectileController>();
        if (projectileContr)
        {
            projectileContr.SetSpeed(_projectileSpeed);
        }
        else
        {
            Debug.LogError("Missing ProjectileController component");
        }

        PlaySound();

        _delay = MAX_DELAY;
    }

    void PlaySound()
    {
        if (audioSource)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.Play();
        }
    }
}