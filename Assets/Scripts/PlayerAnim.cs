using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PlayerAnim : MonoBehaviour
{
    private Player player;
    private Animator playerAnim;

    [Header("Combat Settings")]
    [SerializeField] private Transform transformSwordAttackPoint;
    [SerializeField] private float attackRadius;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float minPitchSwordAtt = 0.7f;
    [SerializeField] private float maxPitchSwordAtt = 1.3f;
    [SerializeField] private bool _isRolling;
    public bool playerIsMoving;


    [Header("Combat AudioClips")]
    [SerializeField] private AudioClip swordAttackSFX;
    [SerializeField] private AudioClip deathSFX;
    public AudioClip respawnSFX;
    [SerializeField] private AudioClip EatingFoodSFX;
    public AudioClip hasNoFoodSFX;

    [Header("Fishing Settings")]
    private FishingScript fishingScript;
    private GameObject fishingGameObject;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip itemDropSound;
    [SerializeField] private AudioClip itemPickUpSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Player>();
        playerAnim = GetComponent<Animator>();

        fishingGameObject = GameObject.FindWithTag("Fishing");
        fishingScript = fishingGameObject.GetComponent<FishingScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.isPaused)
        {
            OnMove();
            AnimationLogic();
        }
        WalkingDirectionSettings();
    }

    #region Movement
    void OnMove()
    {
        //if player is currently receiving movement input
        if (player.direction.sqrMagnitude > 0)
        {
            playerIsMoving = true;
            PlayWalkingAnimation();
        }
        else//go back to idle
        {
            playerIsMoving = false;
            PlayIdleAnimation();
        }
    }

    void WalkingDirectionSettings()
    {
        //esquerda(left)
        if (player.direction.x < 0)
        {
            transform.eulerAngles = new Vector2(0, 180);
            player.PlayerXFacing = "left";
        }
        //direita(right)
        else if(player.direction.x > 0)
        {
            transform.eulerAngles = new Vector2(0, 0);
            player.PlayerXFacing = "right";
        }
        HealthBarOrientation();
        AttackCooldownOrientation();
    }

    private void HealthBarOrientation()
    {
        //esquerda
        if (player.PlayerXFacing.Equals("right"))
        {
            player.HealthBar.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
        else if(player.PlayerXFacing.Equals("left"))
        {
            player.HealthBar.fillOrigin = (int)Image.OriginHorizontal.Right;
        }
    }

    private void AttackCooldownOrientation()
    {
        //esquerda
        if (player.PlayerXFacing.Equals("right"))
        {
            player.AttackCooldown.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
        else if(player.PlayerXFacing.Equals("left"))
        {
            player.AttackCooldown.fillOrigin = (int)Image.OriginHorizontal.Right;
        }
    }
    #endregion

    #region Animation Trigger & Logic
    private void AnimationLogic()
    {
        if (player.IsDead)
        {
            return;
        }

        if (player.IsSwordAttacking)
        {
            PlaySwordAttackAnimation();
        }
        else if (player.HasClickedToRoll)
        {
            PlayRollAnimation();
        }
        else if (player.isRunning)
        {
            PlayRunningAnimation();
        }
        else if (player.IsCutting)
        {
            PlayTreeCuttingAnimation();
        }
        else if (player.IsDigging)
        {
            PlayDiggingAnimation();
        }
        else if (player.IsWatering)
        {
            PlayWateringAnimation();
        }
        else if (player.IsFillingUpWater)
        {
            PlayFillingUpWaterAnimation();
        }
        else if (player.IsSeeding)
        {
            PlaySeedingAnimation();
        }
    }
    public void PlayIdleAnimation()
    {
        playerAnim.SetInteger("transition", 0);
    }
    void PlayWalkingAnimation()
    {
        playerAnim.SetInteger("transition", 1);
    }
    void PlayRunningAnimation()
    {
        playerAnim.SetInteger("transition", 2);
    }
    void PlaySwordAttackAnimation()
    {
        playerAnim.SetInteger("transition", 8);
    }
    void PlayRollAnimation()
    {
        //if anim is not active, set the roll animation triggger.
        if (!playerAnim.GetCurrentAnimatorStateInfo(0).IsName("roll"))
        {
            playerAnim.SetTrigger("isRoll");
        }
        else
        {
            //walking animation
            PlayWalkingAnimation();
        }
    }
    public bool IsPlayingAnimation(string value)
    {
        if (playerAnim.GetCurrentAnimatorStateInfo(0).IsName(value))
        {
            return true;
        }
        return false;
    }
    void PlayTreeCuttingAnimation()
    {
        playerAnim.SetInteger("transition", 3);
    }
    void PlayDiggingAnimation()
    {
        playerAnim.SetInteger("transition", 4);
    }
    void PlayWateringAnimation()
    {
        playerAnim.SetInteger("transition", 5);
    }
    void PlayFillingUpWaterAnimation()
    {
        playerAnim.SetInteger("transition", 6);
    }
    void PlaySeedingAnimation()
    {
        playerAnim.SetInteger("transition", 7);
    }
    #endregion

    #region Combat
    /// <summary>
    /// PASSIVE Checker function. If hit != null, then make Enemy(ies) play OnGettingHit animation.<br></br>
    /// Eg. If Enemy(ies) Layer overlaps Player's attack point, Enemy(ies) performs OnGettingHit() Anim and lose health.
    /// </summary>
    public void OnPlayerAttack()
    {
        // This finds ALL enemies within the attack radius
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(transformSwordAttackPoint.position, attackRadius, enemyLayer);
        //Debug.Log("Found " + enemiesHit.Length + " enemies in the attack radius.");

        if (!player.IsDead)
        {
            // Loop through all the enemies we hit
            foreach (Collider2D hit in enemiesHit)
            {
                //Debug.Log("Processing enemy: " + hit.gameObject.name);


                hit.GetComponentInChildren<SkeletonAnimationControl>().OnGettingHit();

                //EITHER WORKS, TOP /\ OR BOTTOM LOGIC \/
                //SkeletonAnimationControl skeletonAnim = hit.GetComponentInChildren<SkeletonAnimationControl>();
                //if (skeletonAnim != null)
                //{
                //    skeletonAnim.OnGettingHit();
                //    Debug.Log("Player hit Skeleton.");
                //}
            }
        }
    }

    public void OnGettingHit()
    {
        if (player.IsDead)
        {
            //CallTriggerAnim("death");
            return;
        }
        else
        {
            if (player.PlayerCurrentHealth > 1)
            {
                //ResetTriggerAnimation("attack");
                StopAllAnimations();
                playerAnim.SetTrigger("gotHit");
                Debug.Log("Skeleton hit Player.");

                player.TakeDamage(1);
            }
            else if (player.PlayerCurrentHealth <= 1)
            {
                player.TakeDamage(1);
                Debug.Log("Skeleton hit Player.");

                AudioManager.instance.PlayDeathSFX(deathSFX);
                //go idle to avoid keep attacking while dead
                player.DieAndRespawn();


                //FadeAndDestroy(skeletonScript.gameObject);
                //Destroy(player.gameObject, 2f);
            }

        }

    }

    public void ResetTriggerAnimation(string value)
    {
        playerAnim.ResetTrigger(value);

        if (value.Equals("attack"))
        {
            player.IsSwordAttacking = false;
        }
    }
    public void StopAllAnimations()
    {
        if (player.IsSwordAttacking)
        {
            playerAnim.ResetTrigger("attack");
            player.IsSwordAttacking = false;
        }
        playerAnim.StopPlayback();
    }

    public void CallTriggerAnim(string animName)
    {
        playerAnim.SetTrigger(animName);
    }

    #endregion

    public void StartRoll()
    {
        _isRolling = true;
    }
    public void EndRoll()
    {
        _isRolling = false;
    }

    public void RollingLogic()
    {
        if (playerAnim.GetCurrentAnimatorStateInfo(0).IsName("roll"))
        {
            _isRolling = true;
        }
        else
        {
            _isRolling = false;
        }
    }

    /// <summary>
    /// This method is called when player presses "F" to start fishing
    /// </summary>
    public void OnFishingStarted()
    {
        playerAnim.SetTrigger("isFishing");
        player.isPaused = true;
    }

    /// <summary>
    /// this method is called when at the end of fishing animation 
    /// </summary>
    public void OnFishingEnded()
    {
        bool caughtFish;
        caughtFish = fishingScript.OnFishing();
        player.isPaused = false;

        // if fish success rate was choosen (instantiated a fish), run playSFX to make sound of fish drop
        if (caughtFish)
        {
            AudioManager.instance.PlaySFX(itemDropSound);
        }
    }

    public void OnHammeringStarted()
    {
        playerAnim.SetBool("isHammering", true);
    }

    public void OnHammeringFinished()
    {
        playerAnim.SetBool("isHammering", false);
    }

    public bool IsCurrentStateON(string value)
    {
        if (playerAnim.GetCurrentAnimatorStateInfo(0).IsName(value))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //internal object GetCurrentAnimatorStateInfo(int v)
    //{
    //    throw new System.NotImplementedException();
    //}
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transformSwordAttackPoint.position, attackRadius);
    }

    #region Audio
    public void PlaySFX(AudioClip audioClip)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(audioClip);
        }
    }
    public void SwordAttacSFX()
    {
        float randomPitch = Random.Range(minPitchSwordAtt, maxPitchSwordAtt);
        AudioManager.instance.PlaySFXWithPitch(swordAttackSFX, randomPitch);
    }
    public void PlaySoundPickUpItem()
    {
        AudioManager.instance.PlaySFX(itemPickUpSound);
    }
    #endregion

    public AudioClip ItemDropSound { get => itemDropSound; set => itemDropSound = value; }
    public AudioClip ItemPickUpSound { get => itemPickUpSound; set => itemPickUpSound = value; }
    public bool IsRolling { get => _isRolling; set => _isRolling = value; }
}
