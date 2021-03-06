using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class BoxesContainer
{
    static BoxesContainer instance;

    List<Transform> boxesOnLevel;

    int boxesOnScreen;

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
            if (boxesOnLevel[i] && Vector3.Distance(pos, boxesOnLevel[i].position) < Vector3.Distance(pos, nearestPos.position))
                nearestPos = boxesOnLevel[i];

        return nearestPos;
    }

    public void RemoveBox(Transform box)
    {
        boxesOnLevel.Remove(box);
    }

    public void SetBoxesOnScreen(int count)
    {
        boxesOnScreen = count;

        UIManager.instance.UpdateBoxesCount(boxesOnScreen);
    }

    public void DeleteBox()
    {
        --boxesOnScreen;

        UIManager.instance.UpdateBoxesCount(boxesOnScreen);

        if (boxesOnScreen == 0)
            EventManager.GetInstance().GameOver();
    }

    public void Clear()
    {
        boxesOnScreen = 0;
        boxesOnLevel.Clear();
    }

    public void DestroyAll()
    {
        for (int i = 0; i < boxesOnLevel.Count; ++i)
            UnityEngine.Object.Destroy(boxesOnLevel[i].gameObject);

        Clear();
    }
}