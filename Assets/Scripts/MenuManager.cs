using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    private void Start()
    {
        FadeAnimationController.instance.SubscribeOnFadeOutFinish(delegate { SceneManager.LoadScene("Game"); });
    }
}
