using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInit : MonoBehaviour, IUnityAdsInitializationListener
{
    public void OnInitializationComplete()
    {
        print("OnInitializationComplete");
        //throw new System.NotImplementedException();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        print("ads OnInitializationFailed" + error.ToString());
        print("message: " + message);
        //throw new System.NotImplementedException();
    }

    private void Start()
    {
        print("start to init ads");
        //Advertisement.Initialize("4748196", true, this);
        Advertisement.Initialize("4797750", true, this);
        //DontDestroyOnLoad(this);
    }
}
