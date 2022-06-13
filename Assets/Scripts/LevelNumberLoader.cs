using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelNumberLoader : MonoBehaviour
{
    void Start()
    {
        if (!PlayerPrefs.HasKey("LevelNumber"))
            PlayerPrefs.SetInt("LevelNumber", 1);

        GetComponent<TextMeshProUGUI>().text = "LEVEL " + PlayerPrefs.GetInt("LevelNumber");
    }
}
