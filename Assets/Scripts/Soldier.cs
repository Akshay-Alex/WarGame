using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Threading.Tasks;

public class Soldier : MonoBehaviour
{
    [HideInInspector]
    public enum Team
    {
        Red,
        Blue
    }
    [HideInInspector]
    public enum State
    {
        Idle,
        Running,
        Attacking,
        ReceivingDamage
    }
    public State _currentState; 
    
    public SoldierStat soldierStat;
    public int _health;
    public int _damage;
    public int _damageTime;
    public int _attackTime;
    public int _delayTime;
    public Team _team;
    public Soldier _currentEnemy;


    string IdleAnimation = "Idle";
    string RunningAnimation = "Running";
    string AttackingAnimation = "Attack";
    string ReceivingDamageAnimation = "ReceivingDamage";
    Animator _animator;
    AIDestinationSetter _destinationSetter;
    AIPath _aIPath;

    void InitializeStats()
    {
        _health = soldierStat._health;
        _damage = soldierStat._damage;
        _damageTime = soldierStat._damageTime;
    }
    void InitializeAnimator()
    {
        _animator.Play(IdleAnimation);
        _currentState = State.Idle;
    }
    void GetReferences()
    {
        _animator = GetComponent<Animator>();
        _destinationSetter = GetComponent<AIDestinationSetter>();
        _aIPath = GetComponent<AIPath>();
    }
    void ChangeState(State state)
    {
        if(_currentState != state)
        {
            switch (state)
            {
                case State.Idle:
                    _animator.Play(IdleAnimation);
                    _currentState = State.Idle;
                    break;
                case State.Running:
                    _animator.Play(RunningAnimation);
                    _currentState = State.Running;
                    break;
                case State.Attacking:
                    _animator.Play(AttackingAnimation);
                    _currentState = State.Attacking;
                    break;
                case State.ReceivingDamage:
                    _animator.Play(ReceivingDamageAnimation);
                    _currentState = State.ReceivingDamage;
                    break;
                default:
                    _animator.Play(IdleAnimation);
                    _currentState = State.Idle;
                    break;
            }
        }  
    }
    public void ReceiveDamage(int damage)
    {
        ChangeState(State.ReceivingDamage);
        Wait(_damageTime);
        _health = _health - damage;
        //ChangeState(State.Idle);
    }
    void Attack(Soldier enemy)
    {
        Debug.Log("Attack!");
        ChangeState(State.Attacking);
        enemy.ReceiveDamage(_damage);
        //Wait(_attackTime);
        //ChangeState(State.Idle);
        //Wait(_delayTime);
    }
    async void Wait(int delay)
    {
        await Task.Delay(delay);
    }
    void Start()
    {
        GetReferences();
        InitializeStats();
        InitializeAnimator();
        //_team = soldierStat._team;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if(_destinationSetter.target != null)
        {
            _currentEnemy = _destinationSetter.target.transform.GetComponent<Soldier>();
            if (!_aIPath.reachedDestination)
            {
                ChangeState(State.Running);
            }
            else
            {
                //Debug.Log("Reached destination");
                if(_currentState!= State.ReceivingDamage)
                {
                    Attack(_currentEnemy);
                }
            }
        }
    }
}
