using System;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyFactory : MonoBehaviour
{
    [SerializeField] private bool factoryActive = true;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnRate;

    [SerializeField] private float maxNumberOfEnemies;
    
    // delay from last spawn
    private float _delay;
    private float _enemyRadius = 1f;

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

        // Check for number of spawned Enemies
        GameObject[] activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (activeEnemies.Length >= maxNumberOfEnemies)
            return;
        
        GenerateEnemy();
        
        _delay = spawnRate;
    }

    private void GenerateEnemy()
    {
        // Choose position for new spawn
        //horizontal
        float x = Random.Range(
            EnvironmentProps.Instance.MinX() + _enemyRadius,
            EnvironmentProps.Instance.MaxX() - _enemyRadius
        );
        //vertical
        float z = EnvironmentProps.Instance.MaxZ();

        var enemyGO = Instantiate(enemyPrefab, new Vector3(x, 0, z), Quaternion.Euler(0, 135, 0));
        EnemyController enemyController = enemyGO.GetComponent<EnemyController>();
        SetUpType(GetRandomEnemyType(), enemyController);
    }

    private EnemyType GetRandomEnemyType()
    {
        EnemyType[] enemyTypes = (EnemyType[]) Enum.GetValues(typeof(EnemyType));
    
        int randomIndex = Random.Range(0, enemyTypes.Length);
    
        return enemyTypes[randomIndex];
    }
    
    private void SetUpType(EnemyType enemyType, EnemyController enemyController)
    {
        (int health, int attack, int level) = enemyType switch
        {
            EnemyType.WeakEnemy => (6, 5, 1),
            EnemyType.MediumEnemy => (8, 8, 2),
            EnemyType.StrongEnemy => (10, 15, 3),
            _ => throw new ArgumentOutOfRangeException(nameof(enemyType), "Unknown enemy type")
        };
        enemyController.Initialize(enemyType, health, attack, level);
    }
    
    
    public void TurnOff()
    {
        Destroy(this.gameObject);
    }
}