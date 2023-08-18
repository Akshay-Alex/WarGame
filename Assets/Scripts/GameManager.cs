using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public readonly static HashSet<Soldier> RedTeam = new HashSet<Soldier>();
    public readonly static HashSet<Soldier> BlueTeam = new HashSet<Soldier>();
    private void Start()
    {
        gameManager = this;
    }
}
