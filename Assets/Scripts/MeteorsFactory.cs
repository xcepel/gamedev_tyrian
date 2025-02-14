using UnityEngine;

public class MeteorFactory : MonoBehaviour
{
    [SerializeField] private bool factoryActive = true;

    [SerializeField] private GameObject meteorPrefab;
    [SerializeField] private float delayMin;
    [SerializeField] private float delayMax;
    [SerializeField] private float _meteorRadius;
    [SerializeField] private float _meteorSpeed;
    // delay from last spawn
    private float _delay;
        
    void Start() //reset delay
    {
        if (!factoryActive)
        {
            Destroy(this.gameObject);
        }
        
        _delay = 0;
    }
    
    void Update()
    {
        // time elapsed from previous frame
        _delay -= Time.deltaTime;
        if (_delay > 0.0f)
            return;
        
        //Choose position for new spawn
        //horizontal
        float x = Random.Range(
            EnvironmentProps.Instance.MinX() + _meteorRadius,
            EnvironmentProps.Instance.MaxX() - _meteorRadius
        );
        //vertical
        float z = EnvironmentProps.Instance.MaxZ() + _meteorRadius;
        
        var meteorGO = Instantiate(meteorPrefab, new Vector3(x, 0, z), Quaternion.identity);
        //Debug.Log("New meteor spawned at: " + meteorGO.transform.position);
        
        var meteorContr = meteorGO.GetComponent<MeteorController>();
        if (meteorContr)
        {
            meteorContr.SetMeteor(_meteorSpeed, _meteorRadius);
        }
        else
        {
            Debug.LogError("Missing MeteorController component");
        }
        
        _delay = Random.Range(delayMin, delayMax);
    }

    public void TurnOff()
    {
        Destroy(this.gameObject);
    }
}
