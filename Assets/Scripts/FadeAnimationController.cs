using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FadeAnimationController : MonoBehaviour
{
    public static FadeAnimationController instance;

    UnityEvent onFadeInAnimationFinish;
    UnityEvent onFadeOutAnimationFinish;

    private void Awake()
    {
        //if (instance != null)
        //    Destroy(instance);

        instance = this;

        onFadeInAnimationFinish = new UnityEvent();
        onFadeOutAnimationFinish = new UnityEvent();
    }

    public void FadeOut()
    {
        GetComponent<Animator>().Play("FadeOut");
    }

    public void OnFadeInFinish()
    {
        onFadeInAnimationFinish.Invoke();
        onFadeInAnimationFinish.RemoveAllListeners();
    }

    public void OnFadeOutFinish()
    {
        onFadeOutAnimationFinish.Invoke();
        onFadeOutAnimationFinish.RemoveAllListeners();
    }

    public void SubscribeOnFadeInFinish(UnityAction action) => onFadeInAnimationFinish.AddListener(action);

    public void SubscribeOnFadeOutFinish(UnityAction action) => onFadeOutAnimationFinish.AddListener(action);
}
