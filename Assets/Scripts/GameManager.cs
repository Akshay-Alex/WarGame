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
    Camera camera;
    void CheckLeftMouseClick()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Instantiate(BlueSoldierPrefab, hit.point, Quaternion.identity);
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
                Instantiate(RedSoldierPrefab, hit.point, Quaternion.identity);
            }
        }
    }
    private void Start()
    {
        gameManager = this;
        camera = Camera.main;
        //Mouse.current.leftButton.
    }
    private void Update()
    {
        CheckLeftMouseClick();
        CheckRightMouseClick();
    }
    
}
