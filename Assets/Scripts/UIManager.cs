using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] TextMeshProUGUI boxesCount;
    [SerializeField] LevelSpawner levelSpawner;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] GameObject explosionZonePrefab;
    [SerializeField] GameObject nuclearExplosion;

    [SerializeField] GameObject nuclearPanel;
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject losePanel;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance);

        instance = this;

        EventManager.GetInstance().SubscribeOnGameOver(delegate { losePanel.SetActive(true); Time.timeScale = 0; });
        EventManager.GetInstance().SubscribeOnLevelComplete(delegate { winPanel.SetActive(true); Time.timeScale = 0; });
    }

    public void UpdateBoxesCount(int count) => boxesCount.text = count.ToString();

    public void ActivateExtratime(int value)
    {
        levelSpawner.ExtraTime(value);
    }

    public void ActivateSuperBoom()
    {
        var enemies = EnemyContainer.GetInstance().GetEnemies();

        foreach(var enemy in enemies)
        {
            var enemyPos = enemy.gameObject.transform.position;

            Instantiate(explosionPrefab, new Vector3(enemyPos.x, enemyPos.z, 0), Quaternion.identity);
            Destroy(Instantiate(explosionZonePrefab, enemyPos, Quaternion.identity), 0.1f);
        }
    }

    public void ActivateNuclearWepon()
    {
        Instantiate(nuclearExplosion, Vector2.zero, Quaternion.identity);

        Invoke("ShowNuclearPanel", 3);
    }

    void ShowNuclearPanel()
    {
        nuclearPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGame() => Time.timeScale = 1;

    public void PauseGame() => Time.timeScale = 0;

    public void LoadScene(string scene)
    {
        Time.timeScale = 1;

        FadeAnimationController.instance.SubscribeOnFadeOutFinish(delegate { EventManager.GetInstance().Reset(); });
        FadeAnimationController.instance.SubscribeOnFadeOutFinish(delegate { BoxesContainer.GetInstance().Clear(); });
        FadeAnimationController.instance.SubscribeOnFadeOutFinish(delegate { EnemyContainer.GetInstance().Clear(); });
        FadeAnimationController.instance.SubscribeOnFadeOutFinish(delegate { SceneManager.LoadScene(scene); Time.timeScale = 1; });
        FadeAnimationController.instance.FadeOut();
    }
}
