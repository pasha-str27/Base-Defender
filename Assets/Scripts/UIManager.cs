using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] TextMeshProUGUI boxesCount;
    [SerializeField] LevelSpawner levelSpawner;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] GameObject explosionZonePrefab;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance);

        instance = this;
    }

    public void UpdateBoxesCount(int count) => boxesCount.text = count.ToString();

    public void ActivateExtratime(int value)
    {
        levelSpawner.ExtraTime(value);
    }

    public void ActivateSuperBoom()
    {
        var enemies = EnemyContainer.GetInstance().GetEnemies();

        foreach(var enemy in enemies)
        {
            var enemyPos = enemy.gameObject.transform.position;

            Instantiate(explosionPrefab, new Vector3(enemyPos.x, enemyPos.z, 0), Quaternion.identity);
            Destroy(Instantiate(explosionZonePrefab, enemyPos, Quaternion.identity), 0.1f);
        }
    }

    public void ActivateNuclearWepon()
    {

    }
}
