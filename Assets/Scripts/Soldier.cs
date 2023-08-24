using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Threading.Tasks;

public class Soldier : MonoBehaviour
{
    #region Public properties
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
    public int _soldierID;
    public State _currentState;
    public SoldierStat soldierStat;
    public HealthBar healthBar;
    public int _health;
    public int _damage;
    public float _attackLandDelay;
    public Team _team;
    public Soldier _currentEnemy;
    public bool _isEnemyNear;
    public bool _canAttack;
    #endregion

    #region Private properties
    float _maxHealth;
    string IdleAnimation = "Idle";
    string RunningAnimation = "Running";
    string AttackingAnimation = "Attack";
    string ReceivingDamageAnimation = "ReceivingDamage";
    Animator _animator;
    AIDestinationSetter _destinationSetter;
    #endregion

    #region Private functions
    bool IsEnemyNear()
    {
        if (_currentEnemy)
        {
            var DistanceVector = this.transform.position - _currentEnemy.transform.position;
            //Debug.Log("Distance square")
            if (Vector3.SqrMagnitude(DistanceVector) < 400)
            {
                _isEnemyNear = true;
                return true;
            }       
        }
        return false;
    }
    void InitializeStats()
    {
        _health = soldierStat._health;
        _maxHealth = _health;
        _damage = soldierStat._damage;
    }
    void InitializeAnimator()
    {
        ChangeState(State.Idle);
    }
    void GetReferences()
    {
        _animator = GetComponent<Animator>();
        _destinationSetter = GetComponent<AIDestinationSetter>();
    }
    void DamageEnemyHelperFunction()    //this function is used to call ReceiveDamage after a delay
    {
        if (_currentState == State.Attacking) //this check is used to avoid both soldiers getting damaged
        {
            _currentEnemy.ReceiveDamage(_damage,this);
        }
    }

    void Die()
    {
        FxManager.fxManager.PlaySFXAudio(FxManager.fxManager._sfxPoof);
        FxManager.fxManager.PlayVFX(FxManager.fxManager._vfxPoof, transform.position);
        Destroy(this.gameObject);
    }


    void UpdateSoldierState()
    {
        if (_currentEnemy != null)
        {
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
                _destinationSetter.target = this.transform; //this is to stop soldiers moving when target has been destroyed
                ChangeState(State.Idle);
            }
        }

    }
    #endregion

    #region Public functions
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
    public void ReceiveDamage(int damage,Soldier attacker)
    {
        ChangeState(State.ReceivingDamage);
        _health -= damage;
        healthBar.UpdateHealthBar(_health / _maxHealth);
        if (_health <= 0)
        {
            Die();
            GameManager.gameManager.SoldierKilledEvent.Invoke(this, attacker);
            GameManager.gameManager.ScoreUpdatedEvent.Invoke(attacker._team);
        }
        else
        {
            FxManager.fxManager.PlaySFXAudio(FxManager.fxManager._sfxSwordHit);
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
    #endregion

    #region Unity methods
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

    private void FixedUpdate()
    {
        UpdateSoldierState();
    }
    #endregion
}
