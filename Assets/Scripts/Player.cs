using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private string playerXFacing = "right";

    [Header("Combat Settings")]
    [SerializeField] private float attackEveryXSeconds;
    [SerializeField] private float _attackCounter;
    [SerializeField] private bool _canAttack = true;
    [SerializeField] private bool _isSwordAttacking;
    [SerializeField] private bool _hasClickedToRoll;
    //[SerializeField] private bool _enableAttack;// Future update, enable attack every 3s without mouse, on ` key "enable attack" or "attack target"
    [SerializeField] private Image attackCooldown;
    [SerializeField] private bool hasAttacked;

    [Header("Health Settings")]
    [SerializeField] private int playerTotalHealth;
    [SerializeField] private int playerCurrentHealth;
    [SerializeField] private Image healthBar;
    [SerializeField] private bool isDead;


    [Header("Eating Settings")]
    [SerializeField] private float _eatCounter;
    [SerializeField] private float canEatEveryXSeconds;
    [SerializeField] private bool _canEat = true;
    [SerializeField] private bool _isEating;


    [Header("Settings")]
    /// <summary>
    /// to give time to the animation end while being a higher sorting order
    /// </summary>
    [SerializeField] private float SortingOrderDelayTime;
    [SerializeField] private int cuttingSortingOrder;

    [Header("Settings")]
    public bool isPaused;

    private float initialSpeed;
    private bool _isRunning;
    private bool _isCutting;
    private bool _isDigging;
    private bool _isWatering;
    private bool _isFillingUpWater;
    private bool _isSeeding;
    private bool _isFishing;
    private Vector2 _direction;

    private SpriteRenderer _playerSpriteRenderer;
    private int _originalSortingOrder;

    private int handlingObj;
    private Rigidbody2D rb;
    private PlayerAnim playerAnim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<PlayerAnim>();
        initialSpeed = walkingSpeed;
        // Get the SpriteRenderer component from this GameObject (the Player).
        _playerSpriteRenderer = GetComponent<SpriteRenderer>();
        playerCurrentHealth = playerTotalHealth;

        // Store the original sorting order number.
        if (_playerSpriteRenderer != null)
        {
            _originalSortingOrder = _playerSpriteRenderer.sortingOrder;
        }
        else
        {
            Debug.LogError("SpriteRenderer component not found on the Player GameObject!");
        }
    }

    /// <summary>
    /// input related method
    /// </summary>
    private void Update()
    {

        if (!isPaused)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                handlingObj = 0;
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                handlingObj = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                handlingObj = 2;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                handlingObj = 3;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                handlingObj = 4;
            }


            //PlayerMouseInput();
            OnWieldingSword();
            OnEating();
            OnRolling();
            OnInput();
            OnPlayerRun();
            OnCutting();
            OnDigging();
            OnWatering();
            OnFillingUpWater();
            OnSeeding();

            if (Input.GetKeyDown(KeyCode.L))
            {
                if (SceneManager.GetActiveScene().name.Equals("Scene_MapTwo"))
                {
                    SceneManager.LoadScene("MainLand");
                }
                if (SceneManager.GetActiveScene().name.Equals("MainLand"))
                {
                    SceneManager.LoadScene("Scene_MapTwo");
                }
            }
        }
        else
        {
            if (!isDead)
            {
                playerAnim.PlayIdleAnimation();
            }
        }
    }

    /// <summary>
    /// physics related method
    /// </summary>
    private void FixedUpdate()
    {
        if (!isPaused)
        {
            // 1. Determine the correct speed based on player's state
            if (playerAnim.IsCurrentStateON("roll"))
            {
                SetRollingSpeed(); // Roll speed is highest priority
            }
            else if (_isRunning)
            {
                SetRunningSpeed(); // Running speed is next
            }
            else
            {
                SetWalkingSpeed();  // Default is walking speed
            }

            MoveCharacter();
        }
    }

    #region Movement

    void MoveCharacter()
    {
        rb.MovePosition(rb.position + _direction * walkingSpeed * Time.fixedDeltaTime);
    }

    public void MovePlayerToRespawnPoint()
    {
        Vector2 respawnPoint = new Vector2(6f, -6f);

        // Use transform.position for an immediate, guaranteed teleport
        transform.position = respawnPoint;
    }

    private IEnumerator RespawnPlayer(float secondsToRespawn)
    {
        yield return new WaitForSeconds(secondsToRespawn);

        // Teleport the player and reset their state
        MovePlayerToRespawnPoint();
        playerAnim.CallTriggerAnim("respawn");
        AudioManager.instance.PlaySFX(playerAnim.respawnSFX);

        // Reset player state after respawn
        IsDead = false;
        isPaused = false;
        PlayerCurrentHealth = PlayerTotalHealth;
        HealthBar.fillAmount = PlayerCurrentHealth / PlayerTotalHealth;
    }

    /// <summary>
    /// Input.GetAxisRaw("Horizontal"): checks horizontal input (A, D || <-, ->) axis. Returns -1 for left, 1 for right, and 0 when no key is pressed or when opposing keys are pressed simultaneously. "Raw" means it gives an immediate -1, 0, or 1 without any smoothing.
    /// </summary>
    void OnInput()
    {
        _direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    void SetWalkingSpeed()
    {
        walkingSpeed = initialSpeed;
    }

    void SetRunningSpeed()
    {
        walkingSpeed = runSpeed;
    }

    void SetRollingSpeed()
    {
        walkingSpeed = runSpeed + 2;
    }

    #region Combat
    void OnWieldingSword()
    {
        if (isDead)
        {
            return;
        }

        if (_attackCounter <= attackEveryXSeconds)
        {
            if (!_canAttack)
            {
                _attackCounter += Time.deltaTime;

                if (!attackCooldown.enabled)
                {
                    attackCooldown.enabled = true;
                }
                attackCooldown.fillAmount = _attackCounter / attackEveryXSeconds;
            }
            else
            {

            }
        }

        // Check if the player is currently rolling. If so, exit the method.
        if (_hasClickedToRoll)
        {
            // Don't do anything if we are currently rolling.
            return;
        }
        else
        {
            AttackLogic();

        }
    }

    private void AttackLogic()
    {
        if (handlingObj == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!_isSwordAttacking)
                {
                    if (_canAttack && !playerAnim.IsPlayingAnimation("got_hit"))
                    {
                        _isSwordAttacking = true;
                        _canAttack = false;

                        // Reset attack counter and cooldown bar
                        _attackCounter = 0f;
                        attackCooldown.fillAmount = 0f;
                        attackCooldown.enabled = true;
                    }
                    if (_playerSpriteRenderer != null)
                    {
                        _playerSpriteRenderer.sortingOrder = cuttingSortingOrder;
                        StartCoroutine(PlayerSpriteOrderInLayer(SortingOrderDelayTime));
                    }
                }
            }
        }
        if (!_canAttack)
        {
            // If the cooldown is over, allow the next attack
            if (_attackCounter >= attackEveryXSeconds)
            {
                _canAttack = true;
                _attackCounter = 0f;

                attackCooldown.fillAmount = 1f;
                attackCooldown.enabled = false;
            }
        }
    }

    public void TakeDamage(int amount)
    {
        PlayerCurrentHealth = PlayerCurrentHealth - amount;
        HealthBar.fillAmount = ((float)PlayerCurrentHealth / PlayerTotalHealth);
    }

    public void DieAndRespawn()
    {
        IsDead = true;
        isPaused = true;

        //only  for the sake of changing any transition eg. 8 which is attacking
        //back to idle(0), to then die so it won't repeat attacking anim when dying
        playerAnim.PlayIdleAnimation();

        //try stop all animations
        playerAnim.StopAllAnimations();
        playerAnim.CallTriggerAnim("death");

        StartCoroutine(RespawnPlayer(3));
    }
    #endregion

    private void OnEating()
    {
        if (_eatCounter <= canEatEveryXSeconds)
        {
            if (!_canEat)
            {
                _eatCounter += Time.deltaTime;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_canEat)
            {
                //hasFood method, to check if theres carrot or fish, if yes, eat and -1
                if (PlayerItems.Instance.HasFood())
                {
                    if (PlayerItems.Instance.EatFood())
                    {
                        playerAnim.CallTriggerAnim("eating");
                        //sound of eating is inside the anim /\
                        _canEat = false;
                        playerCurrentHealth = playerCurrentHealth + 1;
                        HealthBar.fillAmount = HealthBar.fillAmount + 1;
                        HealthBar.fillAmount = PlayerCurrentHealth / PlayerTotalHealth;
                    }
                }
                else
                {
                    // If the cooldown is over, allow the next attack
                    if (_eatCounter >= canEatEveryXSeconds)
                    {
                        _canEat = true;
                        _eatCounter = 0f;
                    }

                    //sound has no food.
                    AudioManager.instance.PlaySFX(playerAnim.hasNoFoodSFX);
                }
            }
        }


        // If the _eatCounter is over canEatEveryXSeconds, allow to eat
        if (_eatCounter >= canEatEveryXSeconds)
        {
            _canEat = true;
            //play eating sound.
        }
        else
        {
            _canEat = false;
        }
    }

    void OnCutting()
    {
        if (handlingObj == 1)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _isCutting = true;

                if (_playerSpriteRenderer != null)
                {
                    _playerSpriteRenderer.sortingOrder = cuttingSortingOrder;
                    StartCoroutine(PlayerSpriteOrderInLayer(SortingOrderDelayTime));
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                _isCutting = false;
            }
        }
        else
        {
            _isCutting = false;
        }
    }

    void OnDigging()
    {
        if (handlingObj == 2)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (playerAnim.IsCurrentStateON("idle"))
                {
                    _isDigging = true;
                }
                if (_playerSpriteRenderer != null)
                {
                    _playerSpriteRenderer.sortingOrder = cuttingSortingOrder;
                    StartCoroutine(PlayerSpriteOrderInLayer(SortingOrderDelayTime));
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                _isDigging = false;
            }
        }
        else
        {
            _isDigging = false;
        }
    }

    void OnWatering()
    {
        if (handlingObj == 3)
        {
            if (Input.GetMouseButtonDown(0) && PlayerItems.Instance.CurrentWater > 0)
            {
                _isWatering = true;

                _playerSpriteRenderer.sortingOrder = cuttingSortingOrder;
                StartCoroutine(PlayerSpriteOrderInLayer(SortingOrderDelayTime));
            }
            if (Input.GetMouseButtonUp(0) || PlayerItems.Instance.CurrentWater <= 0)
            {
                _isWatering = false;
            }

            //While IsWatering=true and player water is higher than jug min limit, PourWater.
            if (IsWatering)
            {
                if (PlayerItems.Instance.CurrentWater > PlayerItems.Instance.WaterMinLimit)
                {
                    PlayerItems.Instance.PourWater();
                }
            }
        }
        else
        {
            _isWatering = false;
        }
    }

    void OnSeeding()
    {
        if (handlingObj == 4)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _isSeeding = true;

                if (_playerSpriteRenderer != null)
                {
                    _playerSpriteRenderer.sortingOrder = cuttingSortingOrder;
                    StartCoroutine(PlayerSpriteOrderInLayer(SortingOrderDelayTime));
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                _isSeeding = false;
            }
        }
        else
        {
            _isSeeding = false;
        }
    }

    void OnFillingUpWater()
    {
        if (PlayerItems.Instance.IsRefillingWater)
        {
            _isFillingUpWater = true;

            if (_playerSpriteRenderer != null)
            {
                _playerSpriteRenderer.sortingOrder = cuttingSortingOrder;
                StartCoroutine(PlayerSpriteOrderInLayer(SortingOrderDelayTime));
            }
        }
        else
        {
            _isFillingUpWater = false;
        }
    }

    void OnPlayerRun()
    {
        // Check if the player is holding the run key.
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // Only allow running if the character is moving.
            if (direction.sqrMagnitude > 0)
            {
                _isRunning = true;
            }
            else
            {
                // If the player stops, but is still holding the run key, they should stop running.
                _isRunning = false;
            }
        }
        else
        {
            // If the player is not holding the run key at all, they are not running.
            _isRunning = false;
        }
    }

    public void OnRolling()
    {
        if (_isSwordAttacking)
        {
            // Don't do anything if we are currently attacking.
            return;
        }

        //check and proper address roll bool
        playerAnim.RollingLogic();

        if (Input.GetMouseButtonDown(1))
        {
            if (!playerAnim.IsRolling)
            {
                _hasClickedToRoll = true;
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            _hasClickedToRoll = false;
        }
    }
    #endregion

    #region Utility
    private IEnumerator PlayerSpriteOrderInLayer(float num)
    {
        yield return new WaitForSeconds(num);
        _playerSpriteRenderer.sortingOrder = _originalSortingOrder;
    }
    #endregion

    #region GettersAndSetters
    public Vector2 direction
    {
        get { return _direction; }
        set { _direction = value; }
    }
    public bool isRunning
    {
        get { return _isRunning; }
        set { _isRunning = value; }
    }
    public void SetSwordAttackingFalse()
    {
        _isSwordAttacking = false;
    }
    public bool IsCutting { get => _isCutting; set => _isCutting = value; }
    public bool IsDigging { get => _isDigging; set => _isDigging = value; }
    public bool IsWatering { get => _isWatering; set => _isWatering = value; }
    public bool IsFillingUpWater { get => _isFillingUpWater; set => _isFillingUpWater = value; }
    public bool IsSeeding { get => _isSeeding; set => _isSeeding = value; }
    public int CurrentHandlingObj { get => handlingObj; set => handlingObj = value; }
    public bool IsFishing { get => _isFishing; set => _isFishing = value; }
    public bool IsSwordAttacking { get => _isSwordAttacking; set => _isSwordAttacking = value; }
    public bool HasClickedToRoll { get => _hasClickedToRoll; set => _hasClickedToRoll = value; }
    public int PlayerTotalHealth { get => playerTotalHealth; set => playerTotalHealth = value; }
    public int PlayerCurrentHealth { get => playerCurrentHealth; set => playerCurrentHealth = value; }
    public Image HealthBar { get => healthBar; set => healthBar = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    public string PlayerXFacing { get => playerXFacing; set => playerXFacing = value; }
    public Image AttackCooldown { get => attackCooldown; set => attackCooldown = value; }
    #endregion
}
