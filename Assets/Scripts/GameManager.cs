using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public readonly static HashSet<Soldier> RedTeam = new HashSet<Soldier>();
    public readonly static HashSet<Soldier> BlueTeam = new HashSet<Soldier>();
    public GameObject RedSoldierPrefab;
    public GameObject BlueSoldierPrefab;
    public GameObject MenuCanvas;
    public GameObject InGameMenuCanvas;
    public Collider SpawnableAreaCollider;
    public GameState gameState;
    public enum GameState
    {
        InMenu,
        InPlayMode
    }
    Camera camera;
    //coordinate references
    float minimumXCoordinate;
    float maximumXCoordinate;
    float minimumZCoordinate;
    float maximumZCoordinate;
    void CheckLeftMouseClick()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if(hit.collider == SpawnableAreaCollider)
                {
                    Instantiate(BlueSoldierPrefab, hit.point, Quaternion.identity);
                }           
            }
        }
    }
    public void Play()
    {
        ToggleMainMenu(false);
        ToggleInGameMenu(true);
        gameState = GameState.InPlayMode;
    }
    public void ExitGame()
    {
        Application.Quit();
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
    private void Start()
    {
        gameManager = this;
        camera = Camera.main;
        gameState = GameState.InMenu;
        FindSpawnableAreaCorners();

        //Mouse.current.leftButton.
    }
    void FindSpawnableAreaCorners()
    {
        Bounds spawnableBounds = SpawnableAreaCollider.bounds;
         minimumXCoordinate = spawnableBounds.center.x - spawnableBounds.extents.x;
         maximumXCoordinate = spawnableBounds.center.x + spawnableBounds.extents.x;
         minimumZCoordinate = spawnableBounds.center.z - spawnableBounds.extents.z;
         maximumZCoordinate = spawnableBounds.center.z + spawnableBounds.extents.z;
        Debug.Log("X is from " + minimumXCoordinate + " to " + maximumXCoordinate + " And Z is from " + minimumZCoordinate + " to " + maximumZCoordinate);
    }
    private void Update()
    {
        if(gameState == GameState.InPlayMode)
        {
            CheckLeftMouseClick();
            CheckRightMouseClick();
        } 
    }
    
}
