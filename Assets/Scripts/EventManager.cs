using UnityEngine.Events;

class EventManager
{
    static EventManager instance;

    UnityEvent levelComplete;
    UnityEvent gameOver;
    UnityEvent nuclearExplosion;

    EventManager()
    {
        levelComplete = new UnityEvent();
        gameOver = new UnityEvent();
        nuclearExplosion = new UnityEvent();
    }

    public static EventManager GetInstance()
    {
        if (instance == null)
            instance = new EventManager();

        return instance;
    }

    public void SubscribeOnLevelComplete(UnityAction action) => levelComplete.AddListener(action);

    public void SubscribeOnGameOver(UnityAction action) => gameOver.AddListener(action);

    public void GameOver() => gameOver.Invoke();

    public void LevelComplete() => levelComplete.Invoke();

    public void SubscribeOnNuclearExplosion(UnityAction action) => nuclearExplosion.AddListener(action);

    public void NuclearExplosion() => nuclearExplosion.Invoke();

    public void Reset()
    {
        gameOver.RemoveAllListeners();
        levelComplete.RemoveAllListeners();
        nuclearExplosion.RemoveAllListeners();
    }
}
