using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    #region Public properties
    public GameObject _eventsPanel;
    public Transform _gameEventsParentTransform;
    public GameObject _currentEventObject;
    public GameObject _redTeamEvent;
    public GameObject _blueTeamEvent;
    public static EventManager eventManager;
    public Transform _scrollViewTransform;
    HashSet<GameEvent> gameEvents = new HashSet<GameEvent>();
    public float _timeUpdateDelayInSeconds;
    float _timeSinceLastTimeUpdate = 0f;
    #endregion

    #region Public functions
    public void ToggleEventsPanel()
    {
        _eventsPanel.SetActive(!_eventsPanel.activeSelf);
        ScrollToBottom();
    }
    public void CloseEventsPanel()
    {
        _eventsPanel.SetActive(false);
    }
    public void SubscribeToGameEvents()
    {
        GameManager.gameManager.SoldierSpawnedEvent.AddListener(GenerateSoldierSpawnedGameEvent);
        GameManager.gameManager.SoldierKilledEvent.AddListener(GenerateSoldierKilledGameEvent);
        GameManager.gameManager.ScoreUpdatedEvent.AddListener(GenerateScoreUpdatedEvent);
    }
    #endregion

    #region Private functions
    void ScrollToBottom()
    {
        //_contentRectTransform.anchoredPosition = new Vector2(_contentRectTransform.position.x, _contentRectTransform.rect.height - _scrollViewRectTransform.rect.height);
        Canvas.ForceUpdateCanvases();
        _scrollViewTransform.GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
    }
    void GenerateSoldierSpawnedGameEvent(int soldierID, Soldier.Team soldierTeam, Vector3 spawnPosition)
    {
        GenerateGameEvent(soldierTeam);
        string soldierName = soldierTeam.ToString() + soldierID;
        string description = "Soldier spawned at (" + (int) spawnPosition.x + "," + (int)spawnPosition.z + ")";
        _currentEventObject.GetComponent<GameEvent>().SetEventText(soldierName, description);
    }
    void GenerateSoldierKilledGameEvent(Soldier killedSoldier, Soldier Attacker)
    {
        GenerateGameEvent(Attacker._team);
        string soldierName = Attacker._team.ToString() + Attacker._soldierID;
        string description = killedSoldier._team.ToString() + killedSoldier._soldierID + " Killed by " + Attacker._team.ToString() + Attacker._soldierID;
        _currentEventObject.GetComponent<GameEvent>().SetEventText(soldierName, description);
    }
    void GenerateScoreUpdatedEvent(Soldier.Team team)
    {
        GenerateGameEvent(team);
        string soldierName = team.ToString();
        string description = soldierName + " Score updated by 10";
        GameManager.gameManager.IncreaseScore(team);
        _currentEventObject.GetComponent<GameEvent>().SetEventText(soldierName, description);
    }
    
    void GenerateGameEvent(Soldier.Team soldierTeam)
    {
        if(soldierTeam == Soldier.Team.Red)
        {
            _currentEventObject = Instantiate(_redTeamEvent, _gameEventsParentTransform);
        }
        else
        {
            _currentEventObject = Instantiate(_blueTeamEvent, _gameEventsParentTransform);
        }
        gameEvents.Add(_currentEventObject.GetComponent<GameEvent>());
        ScrollToBottom();
    }
    void UnSubscribeToGameEvents()
    {
        GameManager.gameManager.SoldierSpawnedEvent.RemoveAllListeners();
    }
    void RunTimer()
    {
        _timeSinceLastTimeUpdate += Time.deltaTime;
        if (_timeSinceLastTimeUpdate >= _timeUpdateDelayInSeconds)
        {
            _timeSinceLastTimeUpdate = 0f;
            UpdateTimeOnAllEvents();

        }
    }
    void UpdateTimeOnAllEvents()
    {
        HashSet<GameEvent>.Enumerator gameEventsEnumerator = gameEvents.GetEnumerator();
        while(gameEventsEnumerator.MoveNext())
        {
            gameEventsEnumerator.Current.UpdateTimeText();
        }
    }
    #endregion

    #region Unity functions
    private void OnEnable()
    {
        eventManager = this;
    }
    private void OnDisable()
    {
        UnSubscribeToGameEvents();
    }
    private void FixedUpdate()
    {
        RunTimer();
    }
    #endregion
}
