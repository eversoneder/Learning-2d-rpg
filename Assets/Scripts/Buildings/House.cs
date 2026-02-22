using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class House : MonoBehaviour
{
    [Header("House GUI Settings")]
    [SerializeField] private SpriteRenderer houseSprite;
    [SerializeField] private Color alphaHouseColor;
    [SerializeField] private Color regularHouseColor;
    [SerializeField] private Transform playerBuildingPixel;

    [Header("House Timings")]
    [SerializeField] private int woodAmountToBuildHouse;
    [SerializeField] private float fullConstructionTime;
    [SerializeField] private float timeCount;

    [Header("Settings")]
    [SerializeField] private bool isBuilding;
    [SerializeField] private bool isHouseFinished;
    [SerializeField] private bool playerDetected;
    [SerializeField] private GameObject houseCollider;


    private GameObject playerGameObject;
    private Player player;


    private PlayerItems playerItems;
    private PlayerAnim playerAnim;
    private bool setPlayerBuildingPos;

    private void Awake()
    {
        playerGameObject = GameObject.FindWithTag("Player");
        player = playerGameObject.GetComponent<Player>();
        playerItems = player.GetComponent<PlayerItems>();

        playerAnim = player.GetComponent<PlayerAnim>();
    }

    // Update is called once per frame
    void Update()
    {
        // Only allow the player to start building if they are detected,
        // press 'E' once, and the house is not already finished.
        if (playerDetected && Input.GetKeyDown(KeyCode.B) && !isHouseFinished)
        {
            //start building and start timer to finish building
            if (PlayerItems.Instance != null && PlayerItems.Instance.CurrentWood >= woodAmountToBuildHouse)
            {
                Debug.Log("Building started.");
                isBuilding = true;

                player.transform.position = playerBuildingPixel.position;
                player.transform.eulerAngles = new Vector2(0, 0);
                setPlayerBuildingPos = true;

                playerAnim.OnHammeringStarted();
                player.isPaused = true;
                //set house sprite enabled with alphaHouseColor
                houseSprite.color = alphaHouseColor;
            }
            else
            {
                Debug.Log("Player doesn't have enough wood.");
            }
        }

        // This is the core building loop, which only runs if isBuilding is true
        if (isBuilding)
        {
            timeCount += Time.deltaTime;
            // The house is finished when the timer runs out
            if (timeCount >= fullConstructionTime)
            {
                //change house sprite color from alpha to regular (done building)
                houseSprite.color = regularHouseColor;

                PlayerItems.Instance.CurrentWood -= 25;
                playerAnim.OnHammeringFinished();

                isHouseFinished = true; // House is now finished
                houseCollider.SetActive(true);
                isBuilding = false; // Stop the building process

                player.isPaused = false;
                setPlayerBuildingPos = false;
            }
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

