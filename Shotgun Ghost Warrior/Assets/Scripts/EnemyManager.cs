using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    GameObject WaveScreen;
    GameObject EnemyPrefab;
    Transform EnemyHolder;

    Text KillCounter;

    Transform Player;

    int WaveCount = 1;
    int StarterEnemyCount;

    int KillCount = 0;

    readonly int MaxX = 1500;//float MinX = 8.65f;
    readonly int MaxY = 850;//float MinY = 15.25f;

    readonly int MinX = 865;//float MinX = 8.65f;
    readonly int MinY = 1525;//float MinY = 15.25f;

    void Start()
    {
        EnemyPrefab = Resources.Load("Enemy") as GameObject;
        EnemyHolder = GameObject.Find("EnemyHolder").transform;
        Player = GameObject.Find("Player").transform;

        WaveScreen = GameObject.Find("Canvas").transform.Find("WaveScreen").gameObject;
        KillCounter = GameObject.Find("Canvas").transform.Find("KillCounter").GetComponent<Text>();
        KillCounter.text = KillCount + "";

        StarterEnemyCount = EnemyHolder.childCount;
    }
    
    //Checks if a new wave is required, is called when any enemy dies
    public void EnemyDied()
    {
        KillCount++;
        KillCounter.text = KillCount + "";

        StartCoroutine(CheckForNextWave());
    }

    IEnumerator CheckForNextWave()
    {
        yield return 0;

        if (EnemyHolder.childCount <= (WaveCount + 2))
        {
            WaveScreen.SetActive(true);
            WaveScreen.GetComponent<Text>().text = "Wave " + (WaveCount + 1);
            SpawnNextWave();
        }
    }

    //Spawns next wave
    void SpawnNextWave()
    {
        for (int i = 0; i < (StarterEnemyCount + WaveCount); i++)
        {
            SpawnEnemy();
        }

        WaveCount++;
    }

    //Spawns enemy
    void SpawnEnemy()
    {
        float CurrentX, CurrentY;

        if (Random.Range(1, 3) == 1)
        {
            CurrentX = Random.Range(MinX * 1.3f, MinX * 1.85f) / 100f;
            CurrentY = Random.Range(0, MaxY) / 100f;
        }
        else
        {
            CurrentX = Random.Range(0, MaxX) / 100f;
            CurrentY = Random.Range(MinY * 1.3f, MinY * 1.85f) / 100f;
        }

        if(Random.Range(1, 3) == 1)
            CurrentX *= -1;
        if(Random.Range(1, 3) == 1)
            CurrentY *= -1;

        Vector3 posToSpawnAt = new Vector3(CurrentX, CurrentY, 0) + Player.position;
        Instantiate(EnemyPrefab, posToSpawnAt, Quaternion.identity, EnemyHolder);
    }
}
