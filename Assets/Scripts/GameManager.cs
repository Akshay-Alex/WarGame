using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    #region Public events
    public class CustomSoldierSpawnedEvent : UnityEvent<int, Soldier.Team, Vector3>
    {
    }
    /// <summary>
    /// first argument is the soldier that is killed and second argument is the attacker.
    /// </summary>
    public class CustomSoldierKilledEvent : UnityEvent<Soldier,Soldier>
    {
    }
    public class CustomScoreUpdatedEvent : UnityEvent<Soldier.Team>
    {
    }
    public CustomSoldierSpawnedEvent SoldierSpawnedEvent = new CustomSoldierSpawnedEvent();
    public CustomSoldierKilledEvent SoldierKilledEvent = new CustomSoldierKilledEvent();
    public CustomScoreUpdatedEvent ScoreUpdatedEvent = new CustomScoreUpdatedEvent();
    #endregion
    #region Public properties
    public static GameManager gameManager;
    public readonly static HashSet<Soldier> RedTeam = new HashSet<Soldier>();
    public readonly static HashSet<Soldier> BlueTeam = new HashSet<Soldier>();
    public GameObject RedSoldierPrefab;
    public GameObject BlueSoldierPrefab;
    public GameObject Obstacle;
    public GameObject Astar;
    public GameObject MenuCanvas;
    public GameObject InGameMenuCanvas;
    public GameObject HelpTextCanvas;
    public Collider SpawnableAreaCollider;
    public int _redTeamScore = 0, _blueTeamScore = 0;
    public GameState gameState;
    [Range(0f, .5f)]
    public float ObstacleDensity;
    public enum GameState
    {
        InMenu,
        InPlayMode
    }
    #endregion

    #region Private properties
    Camera camera;
    //coordinate references
    float minimumXCoordinate;
    float maximumXCoordinate;
    float minimumZCoordinate;
    float maximumZCoordinate;
    float obstacleXLength, obstacleZLength;
    int redSoldierCount = 0, blueSoldierCount = 0;
    #endregion

    #region Private functions
    void CheckLeftMouseClick()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider == SpawnableAreaCollider)
                {
                    SpawnBlueSoldier(hit);             
                }
            }
        }
    }
    void CheckRightMouseClick()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider == SpawnableAreaCollider)
                {
                    SpawnRedSoldier(hit);
                }
            }
        }
    }
    void SpawnBlueSoldier(RaycastHit hit)
    {
        var soldier = Instantiate(BlueSoldierPrefab, hit.point, Quaternion.identity);
        soldier.GetComponent<Soldier>()._soldierID = blueSoldierCount;
        SoldierSpawnedEvent.Invoke(blueSoldierCount, Soldier.Team.Blue, hit.point);
        blueSoldierCount++;
    }
    void SpawnRedSoldier(RaycastHit hit)
    {
        var soldier = Instantiate(RedSoldierPrefab, hit.point, Quaternion.identity);
        soldier.GetComponent<Soldier>()._soldierID = redSoldierCount;
        SoldierSpawnedEvent.Invoke(redSoldierCount, Soldier.Team.Red, hit.point);
        redSoldierCount++;
    }
    void ToggleMainMenu(bool toggle)
    {
        MenuCanvas.SetActive(toggle);
    }
    void ToggleInGameMenu(bool toggle)
    {
        InGameMenuCanvas.SetActive(toggle);
    }
    
    void CalculateBounds()
    {
        Bounds spawnableAreaBounds = SpawnableAreaCollider.bounds;
        minimumXCoordinate = spawnableAreaBounds.center.x - spawnableAreaBounds.extents.x;
        maximumXCoordinate = spawnableAreaBounds.center.x + spawnableAreaBounds.extents.x;
        minimumZCoordinate = spawnableAreaBounds.center.z - spawnableAreaBounds.extents.z;
        maximumZCoordinate = spawnableAreaBounds.center.z + spawnableAreaBounds.extents.z;
        Bounds obstacleBounds = Obstacle.GetComponentInChildren<Collider>().bounds;
        obstacleXLength = obstacleBounds.size.x;
        obstacleZLength = obstacleBounds.size.z;
        Debug.Log("Obstacle dimensions " + obstacleXLength + " " + obstacleZLength);
    }
    void GenerateObstacles()
    {
        for (float XCoordinate = minimumXCoordinate; XCoordinate <= maximumXCoordinate; XCoordinate += obstacleXLength)
            for (float ZCoordinate = minimumZCoordinate; ZCoordinate <= maximumZCoordinate; ZCoordinate += obstacleZLength)
            {
                if (ObstacleDensity > Random.Range(0f, 1f))
                {
                    Instantiate(Obstacle, new Vector3(XCoordinate, SpawnableAreaCollider.transform.position.y, ZCoordinate), Quaternion.identity);
                }

            }

    }
    #endregion

    #region Public functions
    public void ToggleHelpText()
    {
        FxManager.fxManager.PlaySFXAudio(FxManager.fxManager._sfxClick);
        HelpTextCanvas.SetActive(!HelpTextCanvas.activeSelf);
    }
    public void Play()
    {
        FxManager.fxManager.PlaySFXAudio(FxManager.fxManager._sfxPlay);
        EventManager.eventManager.SubscribeToGameEvents();
        ToggleMainMenu(false);
        ToggleInGameMenu(true);
        gameState = GameState.InPlayMode;
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void IncreaseScore(Soldier.Team team)
    {
        if (team == Soldier.Team.Red)
            _redTeamScore += 10;
        else
            _blueTeamScore += 10;
    }
    #endregion

    #region Unity functions
    private void Start()
    {
        camera = Camera.main;
        gameState = GameState.InMenu;
        CalculateBounds();
        GenerateObstacles();
        Astar.SetActive(true);

        //Mouse.current.leftButton.
    }
    private void OnEnable()
    {
        gameManager = this;
    }

    private void Update()
    {
        if (gameState == GameState.InPlayMode)
        {
            CheckLeftMouseClick();
            CheckRightMouseClick();
        }
    }
    #endregion
}

