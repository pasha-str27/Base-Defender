using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class EnemyContainer
{
    static EnemyContainer instance;

    List<SoldierAgent> enemiesOnLevel;

    EnemyContainer()
    {
        enemiesOnLevel = new List<SoldierAgent>();
    }

    public static EnemyContainer GetInstance()
    {
        if (instance == null)
            instance = new EnemyContainer();

        return instance;
    }

    public void AddEnemy(SoldierAgent enemy) => enemiesOnLevel.Add(enemy);

    public void RemoveEnemy(SoldierAgent enemy) => enemiesOnLevel.Remove(enemy);

    public List<SoldierAgent> GetEnemies() => enemiesOnLevel;

    public void Clear() => enemiesOnLevel.Clear();
}