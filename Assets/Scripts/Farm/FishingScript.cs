using UnityEngine;

public class FishingScript : MonoBehaviour
{
    [SerializeField] private bool playerDetected;
    [SerializeField] private Player player;
    [SerializeField] private int fishingSuccessPercentage;
    [SerializeField] private GameObject fishPrefab;
    private PlayerAnim playerAnim;

    private void Awake()
    {
        playerAnim = player.GetComponent<PlayerAnim>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerDetected && Input.GetKeyDown(KeyCode.F))
        {
            if (PlayerItems.Instance.CurrentFish < PlayerItems.Instance.FishMaxLimit)
            {
                playerAnim.OnFishingStarted();
                
            }
            else
            {
                PlayerItems.Instance.IsHuntingFish = false;
            }
        }


        if (Input.GetKeyUp(KeyCode.F) || PlayerItems.Instance.CurrentFish >= PlayerItems.Instance.FishMaxLimit)
        {
            PlayerItems.Instance.IsHuntingFish = false;
        }
    }

    /// <summary>
    /// Percentage chance of catching a fish
    /// </summary>
    public bool OnFishing()
    {
        int randomValue = Random.Range(1, 100);


        if (randomValue <= fishingSuccessPercentage)
        {
            Instantiate(fishPrefab, player.transform.position + new Vector3(Random.Range(-0.2f, -2f), Random.Range(-1f, 1f), 0), Quaternion.identity);
            return true;
        } else
        {
            //couldn't catch fish
            Debug.Log("Couldn't catch fish.");
            return false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerDetected = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerDetected = false;
        }
    }
}
