using Unity.VisualScripting;
using UnityEngine;

public class SlotFarm : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip carrotHoleSFX;

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    //[SerializeField] private Sprite groundSprite;
    [SerializeField] private Sprite hole;
    [SerializeField] private Sprite carrot;
    [SerializeField] private GameObject carrotPrefab;

    [Header("Settings")]
    [SerializeField] private int digAmount;
    [SerializeField] private float neededWaterToGrowCarrot;
    [SerializeField] private bool waterDetected;
    [SerializeField] private bool hasHoleBeenSeeded;
    [SerializeField] private bool isCarrotGrown;
    [SerializeField] private float pouredWaterInSlot;
    [SerializeField] private float playerInitialCurrentWater;
    [SerializeField] private int totalCarrot;
    [SerializeField] private bool isCarrotPlanted;

    private int initialDigAmount;
    private bool dugHole;
    private bool hasDroppedCarrot;

    private GameObject playerGameObject;
    private PlayerAnim playerAnim;

    private void Awake()
    {
        initialDigAmount = digAmount;
        playerGameObject = GameObject.FindGameObjectWithTag("Player");
        playerAnim = playerGameObject.GetComponent<PlayerAnim>();
    }

    private void Update()
    {
        if (dugHole)
        {
            // Opcional: subtraia a semente do inventário do jogador aqui
            // playerItems.carrots--;

            if (hasHoleBeenSeeded)
            {
                if (waterDetected && !isCarrotGrown)
                {
                    pouredWaterInSlot += 4f * Time.deltaTime; // Usar Time.deltaTime para regar de forma consistente

                    // 3. A rega foi concluída, a cenoura cresce
                    if (pouredWaterInSlot >= neededWaterToGrowCarrot)
                    {
                        pouredWaterInSlot = neededWaterToGrowCarrot; // Garante que não ultrapassa o limite
                        isCarrotGrown = true;

                        //sound setting, play sound once
                        isCarrotPlanted = true;
                        if (isCarrotPlanted)
                        {
                            audioSource.PlayOneShot(carrotHoleSFX);
                        }
                        isCarrotPlanted = false;

                        spriteRenderer.sprite = carrot;
                    }
                }
            }
        }
    }

    public void OnHit()
    {

        if (isCarrotGrown)
        {
            if (!hasDroppedCarrot)
            {
                //drop a carrot
                for (int i = 0; i < totalCarrot; i++)
                {
                    Instantiate(carrotPrefab, transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(1f, -1f), 0f), transform.rotation);
                }
                playerAnim.PlaySFX(playerAnim.ItemDropSound);
                hasDroppedCarrot = true;
            }
            ResetFarmSlot();
        }
        else
        {
            // O buraco não tem uma cenoura crescida, então o hit é para cavar
            digAmount--;

            if (digAmount <= initialDigAmount / 2)
            {
                spriteRenderer.sprite = hole;
                dugHole = true;
                Debug.Log("Buraco cavado!");
            }
        }
    }

    public void ResetFarmSlot()
    {
        spriteRenderer.sprite = null; // Ou um sprite de terra
        dugHole = false;
        isCarrotGrown = false;
        hasDroppedCarrot = false;
        pouredWaterInSlot = 0f;
        hasHoleBeenSeeded = false;
        digAmount = initialDigAmount; // Reinicia a quantidade de golpes para cavar
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Dig"))
        {
            //Debug.Log("bateu");
            OnHit();
        }

        if (collision.CompareTag("Water"))
        {
            waterDetected = true;
        }
        if (collision.CompareTag("Seed"))
        {
            hasHoleBeenSeeded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            waterDetected = false;
        }
        if (collision.CompareTag("Seed"))
        {
            //seedDetected = false;
        }
    }
}
