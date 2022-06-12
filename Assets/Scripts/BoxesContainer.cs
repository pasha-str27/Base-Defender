using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class BoxesContainer
{
    static BoxesContainer instance;

    List<Transform> boxesOnLevel;

    BoxesContainer()
    {
        boxesOnLevel = new List<Transform>();
    }

    public static BoxesContainer GetInstance()
    {
        if (instance == null)
            instance = new BoxesContainer();

        return instance;
    }

    public void AddBox(Transform box) => boxesOnLevel.Add(box);

    public int BoxesCount() => boxesOnLevel.Count;

    public Transform GetRandomBox() => boxesOnLevel[Random.Range(0, boxesOnLevel.Count)];

    public Transform GetNearestBox(Vector3 pos)
    {
        var nearestPos = boxesOnLevel[0];

        for (int i = 1; i < boxesOnLevel.Count; ++i)
            if (Vector3.Distance(pos, boxesOnLevel[i].position) < Vector3.Distance(pos, nearestPos.position))
                nearestPos = boxesOnLevel[i];

        return nearestPos;
    }

    public void RemoveBox(Transform box)
    {
        boxesOnLevel.Remove(box);

        if (boxesOnLevel.Count == 0)
            Debug.LogError("GAME OVER!!!");
    }
}