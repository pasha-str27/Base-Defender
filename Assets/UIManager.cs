using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] TextMeshProUGUI boxesCount;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance);

        instance = this;
    }

    public void UpdateBoxesCount(int count) => boxesCount.text = count.ToString();
}
