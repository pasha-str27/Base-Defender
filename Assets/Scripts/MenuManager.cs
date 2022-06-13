using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    AsyncOperation operation;

    [SerializeField] Slider loadingProgres;
    [SerializeField] GameObject loadingScreen;

    private void Start()
    {
        FadeAnimationController.instance.SubscribeOnFadeOutFinish(delegate { ShowLoadingScreen(); });
    }

    void ShowLoadingScreen()
    {
        loadingScreen.SetActive(true);
        StartCoroutine(SceneLoading());
    }

    IEnumerator SceneLoading()
    {
        operation = SceneManager.LoadSceneAsync("Game");

        while (!operation.isDone)
        {
            yield return new WaitForSeconds(0.1f);
            loadingProgres.value = operation.progress;

            print(loadingProgres.value);
        }

        operation.allowSceneActivation = true;

        yield return null;
    }
}
