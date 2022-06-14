using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BonusButton : MonoBehaviour
{
    bool isActivated = false;

    [SerializeField] RewardedAdsController adsController;
    [SerializeField] UnityEvent onClick;

    void OnEnable() => UnactivateButton();

    public void Click()
    {
        if (!isActivated)
        {
            adsController.SubscribeOnAdsComplete(ActivateButton);

            adsController.Show();

            return;
        }

        onClick.Invoke();

        UnactivateButton();
    }

    public void ActivateButton()
    {
        GetComponent<Image>().color = Color.white;
        transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.white;
        isActivated = true;
    }

    void UnactivateButton()
    {
        GetComponent<Image>().color = Color.gray;
        transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.gray;
        isActivated = false;
    }
}
