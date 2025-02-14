using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }
    private bool durability;
    private bool movementSpeed;
    private bool firingRate;
    
    private void Awake()
    {
        // Implement singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep this object alive across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        durability = false;
        movementSpeed = false;
        firingRate = false;
    }
    
    public void BuyDurability(int price)
    {
        if (Currencies.GetCredits() >= price && !durability)
        {
            durability = true;
            Currencies.ChangeCredits(-price);
        }
    }

    public bool BoughtDurability()
    {
        return durability;
    }
    
    public void BuyMovementSpeed(int price)
    {
        if (Currencies.GetCredits() >= price && !movementSpeed)
        {
            movementSpeed = true;
            Currencies.ChangeCredits(-price);
        }
    }

    public bool BoughtMovementSpeed()
    {
        return movementSpeed;
    }
    
    public void BuyFiringRate(int price)
    {
        if (Currencies.GetCredits() >= price && !firingRate)
        {
            firingRate = true;
            Currencies.ChangeCredits(-price);
        }
    }

    public bool BoughtFiringRate()
    {
        return firingRate;
    }
    
    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}
