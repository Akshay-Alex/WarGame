using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
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
                    Instantiate(BlueSoldierPrefab, hit.point, Quaternion.identity);
                }
            }
        }
    }
    void ToggleMainMenu(bool toggle)
    {
        MenuCanvas.SetActive(toggle);
    }
    void ToggleInGameMenu(bool toggle)
    {
        InGameMenuCanvas.SetActive(toggle);
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
                    Instantiate(RedSoldierPrefab, hit.point, Quaternion.identity);
                }
            }
        }
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
        ToggleMainMenu(false);
        ToggleInGameMenu(true);
        gameState = GameState.InPlayMode;
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    #endregion

    #region Unity functions
    private void Start()
    {
        gameManager = this;
        camera = Camera.main;
        gameState = GameState.InMenu;
        CalculateBounds();
        GenerateObstacles();
        Astar.SetActive(true);

        //Mouse.current.leftButton.
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

