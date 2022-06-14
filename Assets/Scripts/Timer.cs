using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    TextMeshProUGUI timerText;

    void OnEnable() => timerText = GetComponent<TextMeshProUGUI>();

    public void UpdateTime(int time)
    {
        string newText = "";

        if (time / 60 < 10)
            newText += "0";

        newText += (time / 60).ToString() + ":";

        if (time % 60 < 10)
            newText += "0";

        newText += (time % 60).ToString();

        timerText.text = newText;
    }
}
