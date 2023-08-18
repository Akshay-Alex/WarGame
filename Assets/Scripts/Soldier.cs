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
    public float _attackLandDelay;
    public float _attackCooldownTime;
    public Team _team;
    public Soldier _currentEnemy;
    //bool _canAttack;

    //test variables
    public bool _isEnemyNear;
    public bool _canAttack;


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
        ChangeState(State.Idle);
    }
    void GetReferences()
    {
        _animator = GetComponent<Animator>();
        _destinationSetter = GetComponent<AIDestinationSetter>();
        _aIPath = GetComponent<AIPath>();
    }
    public void ChangeState(State state)
    {
        if (_currentState != state)
        {
            switch (state)
            {
                case State.Idle:
                    _animator.Play(IdleAnimation);
                    _currentState = State.Idle;
                    _canAttack = true;
                    break;
                case State.Running:
                    _animator.Play(RunningAnimation);
                    _currentState = State.Running;
                    _canAttack = true;
                    break;
                case State.Attacking:
                    _animator.Play(AttackingAnimation);
                    _currentState = State.Attacking;
                    _canAttack = false;
                    break;
                case State.ReceivingDamage:
                    _animator.Play(ReceivingDamageAnimation);
                    _currentState = State.ReceivingDamage;
                    _canAttack = false;
                    break;
                default:
                    _animator.Play(IdleAnimation);
                    _currentState = State.Idle;
                    break;
            }
        }
    }
    


 
    public void Attack()
    {
        ChangeState(State.Attacking);
        Invoke("DamageEnemyHelperFunction", _attackLandDelay);
        
        //ChangeState(State.Idle);
        //enemy.ReceiveDamage(_damage);
    }
    void DamageEnemyHelperFunction()    //this function is used to call ReceiveDamage after a delay
    {
        if(_currentState == State.Attacking) //this check is used to avoid both soldiers getting damaged
        {
            _currentEnemy.ReceiveDamage(_damage);
        }    
    }
    public void ReceiveDamage(int damage)
    {
        ChangeState(State.ReceivingDamage);
        //ReceiveDamageAsync().GetAwaiter().GetResult();
        _health -= damage;
        if(_health <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Destroy(this.gameObject);
    }

    async Task WaitAsync(int delay)
    {
         await Task.Delay(delay);
    }
    void Wait(int delay)
    {
        WaitAsync(delay).GetAwaiter().GetResult();
    }
    
    void UpdateSoldierState()
    {
        if (_currentEnemy != null)
        {
            //_isEnemyNear = IsEnemyNear(); // need to remove
            if (!IsEnemyNear())
            {
                ChangeState(State.Running);
            }
            else
            {
                if (_canAttack)
                {
                    Attack();
                }
                //ChangeState(State.Idle);
                //Debug.Log("Reached destination");
                //if (!(_currentState == State.Attacking || _currentState == State.ReceivingDamage))
                //{
                //   Attack(_currentEnemy);
                //}
            }
        }
        else
        {
            _currentEnemy = FindNearestEnemySoldier(transform.position, _team);
            if (_currentEnemy)
            {
                _destinationSetter.target = _currentEnemy.transform;
            }
            else
            {
                Debug.Log("No enemies left");
            }
        }

    }
    public static Soldier FindNearestEnemySoldier(Vector3 pos, Team team) //This public static will be read by your enemy script, it makes sense to name it this, to find the closest ally
    {
        Soldier result = null;
        HashSet<Soldier>.Enumerator enemySoldiers;
        float dist = float.PositiveInfinity;
        if (team == Team.Red)
        {
            enemySoldiers = GameManager.BlueTeam.GetEnumerator();
        }
        else
        {
            enemySoldiers = GameManager.RedTeam.GetEnumerator();
        }
        while (enemySoldiers.MoveNext())
        {
            float d = (enemySoldiers.Current.transform.position - pos).sqrMagnitude;
            if (d < dist)
            {
                result = enemySoldiers.Current;
                dist = d;
            }
        }
        return result;
    }
    bool IsEnemyNear()
    {
        if (_currentEnemy)
        {
            var DistanceVector = this.transform.position - _currentEnemy.transform.position;
            //Debug.Log("Distance square")
            if (Vector3.SqrMagnitude(DistanceVector) < 400)
                return true;
        }
        return false;
    }
    private void OnEnable()
    {
        if (_team == Team.Red)
        {
            GameManager.RedTeam.Add(this);
        }
        else
        {
            GameManager.BlueTeam.Add(this);
        }
    }
    private void OnDisable()
    {
        if (_team == Team.Red)
        {
            GameManager.RedTeam.Remove(this);
        }
        else
        {
            GameManager.BlueTeam.Remove(this);
        }
    }

    void Start()
    {
        GetReferences();
        InitializeStats();
        InitializeAnimator();
    }

    private void Update()
    {
        //Debug.Log("is Attacking "+AnimatorIsPlaying(AttackingAnimation));
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        UpdateSoldierState();
        //ShowCurrentAnimatorState();
    }
    void ShowCurrentAnimatorState()
    {
        if (_currentEnemy)
        {
            Debug.Log("Distance " + Vector3.Distance(this.transform.position, _currentEnemy.transform.position));
        }

        //var state = _animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
        //Debug.Log(state);
    }

}
