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

    public void RemoveBox(Transform box)
    {
        boxesOnLevel.Remove(box);

        if (boxesOnLevel.Count == 0)
            Debug.LogError("GAME OVER!!!");
    }
}