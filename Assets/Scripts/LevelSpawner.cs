using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    [SerializeField] Vector2 screenSize;
    [SerializeField] int timePerBox = 15;

    [SerializeField] float minEnemySpawnDelay = 1;
    [SerializeField] float maxEnemySpawnDelay = 5;

    [SerializeField] GameObject[] targetBoxes;
    [SerializeField] GameObject[] levelEnvirement;
    [SerializeField] Material[] planeMaterials;

    [SerializeField] GameObject plane;
    [SerializeField] GameObject soldier;

    int time;

    void Start()
    {
        plane.GetComponent<MeshRenderer>().material = planeMaterials[Random.Range(0, planeMaterials.Length)];

        SpawnLevelEnvirement();
        SpawnBoxes();
        StartCoroutine(SpawnEnemies());
        StartCoroutine(Timer());
    }

    void SpawnLevelEnvirement()
    {
        int count = Random.Range(1, 5);

        for (int i = 0; i < count; ++i) 
        {
            var pos = new Vector3(Random.Range(-screenSize.x, screenSize.x), 1, Random.Range(-screenSize.y, screenSize.y));
            var rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            Instantiate(levelEnvirement[Random.Range(0, levelEnvirement.Length)], pos, rotation);
        }
    }

    void SpawnBoxes()
    {
        int count = Random.Range(5, 21);

        time = count * timePerBox + 60 - (count * timePerBox) % 60;

        var spawnZone = screenSize - new Vector2(6, 1);

        for (int i = 0; i < count; ++i)
        {
            Vector3 pos = Vector3.zero;

            while(true)
            {
                pos = new Vector3(Random.Range(-spawnZone.x, spawnZone.x), 2, Random.Range(-spawnZone.y, spawnZone.y));

                var collider = Physics.OverlapSphere(pos, 0.5f, 7);

                if (collider.Length == 0)
                    break;
            }

            var rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            Instantiate(targetBoxes[Random.Range(0, targetBoxes.Length)], pos, rotation);
        }
    }

    Vector3 GetEnemySpawnPos()
    {
        var spawnPosition = screenSize + new Vector2(1.5f, 1.5f);

        switch (Random.Range(0, 4))
        {
            case 0:
                spawnPosition.y = Random.Range(-spawnPosition.y, spawnPosition.y);
                break;
            case 1:
                spawnPosition.x *= -1;
                spawnPosition.y = Random.Range(-spawnPosition.y, spawnPosition.y);
                break;
            case 2:
                spawnPosition.x = Random.Range(-spawnPosition.x, spawnPosition.x);
                break;
            case 3:
                spawnPosition.y *= -1;
                spawnPosition.x = Random.Range(-spawnPosition.x, spawnPosition.x);
                break;
        }

        return new Vector3(spawnPosition.x, 1, spawnPosition.y);
    }

    IEnumerator SpawnEnemies()
    {
        while(time > 0)
        {
            int enemyCount = Random.Range(2, 5);

            for (int i = 0; i < enemyCount; ++i)
                Instantiate(soldier, GetEnemySpawnPos(), Quaternion.Euler(0, 90, 0));

            yield return new WaitForSeconds(Random.Range(minEnemySpawnDelay, maxEnemySpawnDelay));
        }
    }

    IEnumerator Timer()
    {
        yield break;
    }
}
