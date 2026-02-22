using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SkeletonScript : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackEveryXSeconds;
    [SerializeField] private float _attackCounter;
    [SerializeField] private bool _canAttack = false;
    [SerializeField] private bool _isAttacking;
    [SerializeField] private float _detectPlayerRadius;
    [SerializeField] private bool _playerDetected;
    [SerializeField] private LayerMask _playerLayer;
    // store the original position
    private Vector3 initialPosition;

    [Header("Health Settings")]
    [SerializeField] private float skeletonTotalHealth;
    [SerializeField] private float skeletonCurrentHealth;
    [SerializeField] private Image healthBar;
    [SerializeField] private bool isDead;

    [Header("Settings")]
    [SerializeField] private string skeletonXFacing = "left";
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private SkeletonAnimationControl animControl;
    private Player player;
    private GameObject playerGO;

    private bool _xChecked;
    private float _turnCounter;
    [SerializeField] private float secsToTurnXAxis;

    public float SkeletonCurrentHealth { get => skeletonCurrentHealth; set => skeletonCurrentHealth = value; }
    public float SkeletonTotalHealth { get => skeletonTotalHealth; set => skeletonTotalHealth = value; }
    public Image HealthBar { get => healthBar; set => healthBar = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    public string SkeletonXFacing { get => skeletonXFacing; set => skeletonXFacing = value; }
    public float DetectPlayerRadius { get => _detectPlayerRadius; set => _detectPlayerRadius = value; }
    public bool PlayerDetected { get => _playerDetected; set => _playerDetected = value; }

    private void Awake()
    {
        skeletonCurrentHealth = skeletonTotalHealth;

        playerGO = GameObject.FindWithTag("Player");
        player = playerGO.GetComponent<Player>();

        //keep skeleton sprite stabilized for 2d and without rotation
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // Store the skeleton's initial position
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Make sure the skeleton is not dead before doing anything.
        if (isDead)
        {
            return; // Exit the Update loop if the skeleton is dead.
        }

        // This is the core logic: check if the player is in range.
        Collider2D hit = Physics2D.OverlapCircle(transform.position, _detectPlayerRadius, _playerLayer);
        _playerDetected = (hit != null); // Directly update the state.

        // If player is detected and not paused
        if (_playerDetected && !player.isPaused)
        {
            // Check if we are within stopping distance.
            if (Vector2.Distance(transform.position, player.transform.position) <= agent.stoppingDistance)
            {
                animControl.PlayIntegerAnimation(0); // Idle animation
            }
            else
            {
                animControl.PlayIntegerAnimation(1); // Set to walking animation
            }

            // Continue the NavMesh speed momentum when following the player.
            agent.isStopped = false;
            // Follow the player.
            agent.SetDestination(player.transform.position);

            // Face and attack logic.
            FaceTarget(player.transform.position);
            SkeletonAttack();
        }
        // If the player is NOT detected, go back to the starting point.
        else if (Vector2.Distance(transform.position, initialPosition) > agent.stoppingDistance)
        {
            // Move back to the initial position.
            agent.SetDestination(initialPosition);

            FaceTarget(initialPosition); // You can still face a direction while returning.
            animControl.PlayIntegerAnimation(1); // Set to walking animation.
        }
        else
        {
            // We're back at the initial position, so stop and go idle.
            animControl.PlayIntegerAnimation(0); // Idle animation
                                          // Stop the NavMesh speed momentum when idle.
            agent.isStopped = true;
        }
    }

    private void FixedUpdate()
    {
        DetectPlayer();
    }

    /// <summary>
    /// Face a given target on the X Axis.
    /// </summary>
    public void FaceTarget(Vector3 targetPosition)
    {
        // The same logic as before, but now using a generic targetPosition
        if (!_xChecked)
        {
            float targetVsEnemyPosX = targetPosition.x - transform.position.x;
            if (targetVsEnemyPosX > 0)
            {
                transform.eulerAngles = new Vector2(0, 0); // Skeleton turns right
                skeletonXFacing = "left";
            }
            else
            {
                transform.eulerAngles = new Vector2(0, 180); // Skeleton turns left
                skeletonXFacing = "right";
            }
            _xChecked = true;
        }

        _turnCounter += Time.deltaTime;
        if (_turnCounter >= secsToTurnXAxis)
        {
            _xChecked = false;
            _turnCounter = 0;
        }
    }

    private void SkeletonAttack()
    {
        _attackCounter += Time.deltaTime;

        //when skeleton gets close to player, attack.
        if (Vector2.Distance(transform.position, player.transform.position) <= agent.stoppingDistance)
        {
            //chegou no limite de distancia do player / skeleton para pra atacar
            animControl.PlayIntegerAnimation(0);//idle to start attack

            if (_canAttack)
            {
                //animControl.PlayAnimation(2);//attack
                animControl.AttackAnimation();
                _canAttack = false;
            }

            if (!_canAttack)
            {
                // If the cooldown is over, allow the next attack
                if (_attackCounter >= attackEveryXSeconds)
                {
                    _canAttack = true;
                    _attackCounter = 0f;
                }
            }
        }
        else
        {
            //skeleton esta seguindo o player
            animControl.PlayIntegerAnimation(1);//walk animation
        }
    }
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //    {
    //        //playerDetected = true;
    //    }
    //}
    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //    {
    //        //playerDetected = false;
    //    }
    //}

    public void DetectPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, _detectPlayerRadius, _playerLayer);

        if (hit)
        {
            //have seen player
            _playerDetected = true;
        }
        else
        {
            //haven't seen player
            _playerDetected = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _detectPlayerRadius);
    }
}
