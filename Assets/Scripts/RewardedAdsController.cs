using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Advertisements;
using UnityEngine.Events;

public class RewardedAdsController : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    string _adUnitId = "Rewarded_Android";

    UnityEvent onAdComplete;

    public void Start()
    {
        onAdComplete = new UnityEvent();
        Advertisement.Load(_adUnitId, this);
    }

    public void Show()
    {
        OnUnityAdsShowClick(_adUnitId);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        print("rewarded ads loaded");
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        print("rewarded ads failed to load");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        print("rewarded OnUnityAdsShowFailure");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        print("rewarded OnUnityAdsShowStart");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Advertisement.Show(_adUnitId, this);
        print("rewarded OnUnityAdsShowClick");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            print("finished");

            onAdComplete.Invoke();
            onAdComplete.RemoveAllListeners();

            Advertisement.Load(_adUnitId, this);
        }
    }

    public void SubscribeOnAdsComplete(UnityAction action) => onAdComplete.AddListener(action);
}
