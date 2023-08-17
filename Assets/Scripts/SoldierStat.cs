using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoldierStat", menuName = "SoldierStat")]
public class SoldierStat : ScriptableObject
{
    public int _health;
    public int _damage;
    public int _damageTime;
    public int _attackTime;
    public int _delayTime;
}
