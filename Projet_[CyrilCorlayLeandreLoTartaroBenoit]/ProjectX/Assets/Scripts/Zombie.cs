using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Zombie : MonoBehaviour
{
    // ============================================================
    // Variables
    // ============================================================

    public LevelManager levelManager;

    AudioSource audioSource;

    private Player player;

    private AIPath aiPath;
    private Rigidbody2D rb;
    private Animator animator;

    [SerializeField] private float viewDistanceMove;
    [SerializeField] private float viewDistanceLost;
    [SerializeField] private float viewDistanceAttack;
    [SerializeField] private int health;

    // ============================================================
    // Enum
    // ============================================================

    public enum Animation
    {
        Idle,
        Attack,
        Move,
    }

    private Animation _currentAnimation;

    public Animation CurrentAnimation
    {
        get { return _currentAnimation; }
        set
        {
            _currentAnimation = value;
            UpdateAnimation();
        }
    }

    public string AnimationName
    {
        get
        {
            var action = CurrentAnimation.ToString();

            return "Zombie_" + action;
        }
    }

    // ============================================================
    // Start, Update, etc.
    // ============================================================

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        aiPath = GetComponent<AIPath>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = FindObjectOfType<Player>();

        CurrentAnimation = Animation.Idle;
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < viewDistanceAttack)
        {
            AttackTarget();
        }
        else if (Vector3.Distance(transform.position, player.transform.position) < viewDistanceMove && 
                 Vector3.Distance(transform.position, player.transform.position) > viewDistanceAttack)
        {
            MoveToTarget();
        }
        else if (Vector3.Distance(transform.position, player.transform.position) > viewDistanceLost)
        {
            TargetLost();
        }
    }

    // ============================================================
    // Collisions
    // ============================================================

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("HandgunBullet"))
        {
            health--;
            MoveToTarget();
            GameManager.Instance.PrefabManager.Spawn(PrefabManager.Global.Blood, gameObject.transform.position, transform.rotation);
            if (health <= 0)
            {
                Destroy(gameObject);
                levelManager.TotalZombiesKilled++;
                
            }    
        }
    }

    // ============================================================
    // Functions
    // ============================================================

    private void RotateTowardsTarget()
    {
        Vector3 direction = player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle;
    }

    private void UpdateAnimation()
    {
        var animationName = AnimationName;
        animator.Play(animationName);
    }

    private void MoveToTarget()
    {
        CurrentAnimation = Animation.Move;
        RotateTowardsTarget();

        aiPath.canMove = true;
        aiPath.canSearch = true;

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private void TargetLost()
    {
        CurrentAnimation = Animation.Idle;

        aiPath.canMove = false;
        aiPath.canSearch = false;

        audioSource.Stop();
    }

    private void AttackTarget()
    {
        CurrentAnimation = Animation.Attack;

        RotateTowardsTarget();

        aiPath.canMove = true;
        aiPath.canSearch = true;
    }

    private void setAnimationToIdle()
    {
        CurrentAnimation = Animation.Idle;
    }
}