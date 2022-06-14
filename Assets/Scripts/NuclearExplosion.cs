using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NuclearExplosion : MonoBehaviour
{
    [SerializeField] GameObject craterPrefab;
    [SerializeField] GameObject nuclearPanel;

    void Start()
    {
        Invoke("Explosion", 0.1f);
    }

    void Explosion()
    {
        PlayerPrefs.DeleteKey("LevelNumber");

        EventManager.GetInstance().NuclearExplosion();

        var crater = Instantiate(craterPrefab, Vector2.zero, Quaternion.Euler(90, 0, 0));
        crater.GetComponent<Transform>().localScale = Vector3.one * 25;

        Invoke("ShowNuclearPanel", 3);
    }

    void ShowNuclearPanel()
    {
        //nuclearPanel.SetActive(true);
    }
}
