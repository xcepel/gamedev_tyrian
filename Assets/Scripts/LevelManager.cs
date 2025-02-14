using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private int level;
    // TODO CHANGE VALUES IN LEVELS
    [SerializeField] private int meteorsGoal;
    [SerializeField] private int enemiesGoal;
    
    private int killedMeteors = 0;
    private int killedEnemies = 0;
    
    bool bossBattle = false;
    
    public event Action OnLevelFinished;
    public event Action OnGameFinished;
    
    void Start()
    {
        GameManager.Instance.LoadLevelManager();
        killedMeteors = 0;
        killedEnemies = 0;
    }

    void Update()
    {
        if (GoalsCheck())
        {
            if (level == 3)
            {
                if (bossBattle == false)
                {
                    SetUpForBossBattle();
                    
                    BossConroller boss = FindFirstObjectByType<BossConroller>();
                    if (boss)
                    {
                        boss.ActivateBoss();
                    }
                
                    bossBattle = true;
                }
            }
            else
            {
                OnLevelFinished?.Invoke();
                Destroy(gameObject);
            }
        }
    }
    private bool GoalsCheck()
    {
        return (killedMeteors >= meteorsGoal && killedEnemies >= enemiesGoal);
    }

    private void SetUpForBossBattle()
    {
        MeteorFactory meteorFactory = FindFirstObjectByType<MeteorFactory>();
        if (meteorFactory)
        {
            meteorFactory.TurnOff();
        }
        EnemyFactory enemyFactory = FindFirstObjectByType<EnemyFactory>();
        if (enemyFactory)
        {
            enemyFactory.TurnOff();
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        GameObject[] meteors = GameObject.FindGameObjectsWithTag("Meteor");
        foreach (GameObject meteor in meteors)
        {
            Destroy(meteor);
        }
    }

    public void KilledMeteor()
    {
        killedMeteors++;
    }
    
    public void KilledEnemy()
    {
        killedEnemies++;
    }
    
    public void KilledBoss()
    {
        OnGameFinished?.Invoke();
        Destroy(gameObject);
    }
    
    public int GetLevel()
    {
        return level;
    }
}
