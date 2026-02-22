using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkeletonAnimationControl : MonoBehaviour
{
    private Animator skeletonAnim;

    [SerializeField] private Transform transformAttackPoint;
    [SerializeField] private float attackRadius;
    [SerializeField] private LayerMask playerLayer;

    private GameObject playerGameObject;
    private Player player;
    private PlayerAnim playerAnim;

    private SkeletonScript skeletonScript;

    private void Awake()
    {
        skeletonAnim = GetComponent<Animator>();

        playerGameObject = GameObject.FindGameObjectWithTag("Player");
        player = playerGameObject.GetComponent<Player>();

        playerAnim = playerGameObject.GetComponent<PlayerAnim>();

        skeletonScript = GetComponentInParent<SkeletonScript>();
    }

    // Update is called once per frame
    void Update()
    {
        HealthBarOrientation();
    }

    /// <summary>
    /// Logic for the Health bar orientation to always decreases from right to left.
    /// </summary>
    private void HealthBarOrientation()
    {
        if (skeletonScript.SkeletonXFacing.Equals("left"))
        {
            skeletonScript.HealthBar.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
        //esquerda
        if (skeletonScript.SkeletonXFacing.Equals("right"))
        {
            skeletonScript.HealthBar.fillOrigin = (int)Image.OriginHorizontal.Right;
        }
    }
    public void PlaySFX(AudioClip audioClip)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(audioClip);
        }
    }

    public void PlayIntegerAnimation(int value)
    {
        skeletonAnim.SetInteger("transition", value);
    }

    /// <summary>
    /// Perform Attack Animation.
    /// </summary>
    public void AttackAnimation()
    {
        skeletonAnim.SetTrigger("attack");
        Debug.Log("Skeleton hit Player.");
    }

    /// <summary>
    /// PASSIVE Checker function. If hit != null, then make Player play OnGettingHit animation.<br></br>
    /// Eg. If Player Layer overlaps Skeleton's attack point, Player performs OnGettingHit() Anim and lose health.
    /// </summary>
    public void OnSkeletonAttack()
    {
        if (!skeletonScript.IsDead)
        {
            Collider2D hit = Physics2D.OverlapCircle(transformAttackPoint.position, attackRadius, playerLayer);

            if (hit != null)
            {
                playerAnim.OnGettingHit();
            }
        }
    }

    public void OnGettingHit()
    {
        if (skeletonScript.SkeletonCurrentHealth <= 1)
        {
            
            skeletonAnim.SetTrigger("death");
            skeletonScript.IsDead = true;

            //FadeAndDestroy(skeletonScript.gameObject);
            skeletonScript.SkeletonCurrentHealth--;
            skeletonScript.HealthBar.fillAmount = skeletonScript.SkeletonCurrentHealth / skeletonScript.SkeletonTotalHealth;

            Destroy(skeletonScript.gameObject, 2f);
        }
        else if (skeletonScript.SkeletonCurrentHealth > 1)
        {
            skeletonAnim.SetTrigger("gotHit");
            skeletonAnim.ResetTrigger("attack");
            Debug.Log("Player hit Skeleton.");

            skeletonScript.SkeletonCurrentHealth--;
            skeletonScript.HealthBar.fillAmount = skeletonScript.SkeletonCurrentHealth / skeletonScript.SkeletonTotalHealth;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transformAttackPoint.position, attackRadius);
    }

    private IEnumerator FadeAndDestroy(GameObject gameObj)
    {
        // Get the SpriteRenderer component from the GameObject that was passed in
        SpriteRenderer spriteRenderer = gameObj.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            // If it's on a child object, use GetComponentInChildren
            spriteRenderer = gameObj.GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            // ... (rest of the fading logic is correct)
            float fadeDuration = 3f;
            float elapsedTime = 0f;
            Color startColor = spriteRenderer.color;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float newAlpha = 1 - (elapsedTime / fadeDuration);
                spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
                yield return null;
            }

            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 0);
        }

        // Destroy the GameObject after the fade is complete
        Destroy(gameObj);
    }
}
