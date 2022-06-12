using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AgentTarget : MonoBehaviour
{
    List<SoldierAgent> subscribedEnemies;

    private void Awake()
    {
        subscribedEnemies = new List<SoldierAgent>();
        BoxesContainer.GetInstance().AddBox(transform);
    }

    public void SubscribeOnTargetTaken(SoldierAgent enemy) => subscribedEnemies.Add(enemy);

    public void UnSubscribeOnTargetTaken(SoldierAgent enemy) => subscribedEnemies.Remove(enemy);

    public void TargetTaken()
    {
        for (int i = 0; i < subscribedEnemies.Count; ++i) 
            subscribedEnemies[i]?.FindNewTarget();

        subscribedEnemies.Clear();
    }
}
