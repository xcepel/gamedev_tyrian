using UnityEngine;

public class EnvironmentProps : MonoBehaviour
{
    public static EnvironmentProps Instance { get; private set; }
    
    private float sizeX = 45;
    private float sizeZ = 25;

    public float MinX() { return -sizeX / 2.0f; }
    public float MaxX() { return sizeX / 2.0f; }
    public float MinZ() { return -sizeZ / 2.0f; }
    public float MaxZ() { return sizeZ / 2.0f; }


    // Start is called before the first frame update
    
    public void Awake()
    {
        // Check, if we do not have any instance yet.
        if (Instance == null)
        {
            // 'this' is the first instance created => save it.
            Instance = this;
        }
        else if (Instance != this)
        {
            // Destroy 'this' object as there exist another instance
            Destroy(this.gameObject);
        }
    }

    public Vector3 IntoAreaSphere(Vector3 pos, float dx)
    {
        Vector3 result = pos;
        
        // Check X axis (left and right)
        result.x = result.x - dx < MinX() ? MinX() + dx : result.x;
        result.x = result.x + dx > MaxX() ? MaxX() - dx : result.x;

        // Check Z axis (up and down)
        result.z = result.z - dx < MinZ() ? MinZ() + dx : result.z;
        result.z = result.z + dx > MaxZ() ? MaxZ() - dx : result.z;
        
        return result;
    }
    
    public Vector3 IntoAreaEnemy(Vector3 pos, Vector3 size)
    {
        Vector3 result = pos;

        // Check X axis (left and right)
        result.x = result.x - size.x/2 < MinX() ? MinX() + size.x/2 : result.x;
        result.x = result.x + size.x/2 > MaxX() ? MaxX() - size.x/2 : result.x;
        
        return result;
    }
    
    public Vector3 IntoArea(Vector3 pos, float dx, float dz)
    {
        Vector3 result = pos;
        result.x = result.x - dx < MinX() ? MinX() + dx : result.x;
        result.x = result.x + dx > MaxX() ? MaxX() - dx : result.x;
        
        result.z = result.z - dz < MinZ() ? MinZ() + dz : result.z;
        result.z = result.z + dz > MaxZ() ? MaxZ() - dz : result.z;

        return result;
    }
    
    public bool EscapedDown(Vector3 pos, float dz = 0)
    {
        return pos.z + dz < MinZ();
    }
    
    public bool EscapedUp(Vector3 pos, float dz = 0)
    {
        return pos.z - dz > MaxZ();
    }
    
    public bool IsOutsideArea(Vector3 pos)
    {
        return pos.x < MinX() || pos.x > MaxX() || pos.z < MinZ() || pos.z > MaxX();
    }
    
}
