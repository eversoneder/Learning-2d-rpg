using UnityEngine;

public class WaterFarm : MonoBehaviour
{

    [SerializeField] private bool playerDetected;
    [SerializeField] private float waterIntakeRate;
    private GameObject player;

    private void Awake()
    {
        if (waterIntakeRate == 0)
        {
            waterIntakeRate = 0.4f;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerDetected && Input.GetKey(KeyCode.R))
        {
            if (PlayerItems.Instance.CurrentWater < PlayerItems.Instance.WaterMaxLimit)
            {
                PlayerItems.Instance.AddWater(waterIntakeRate);
                PlayerItems.Instance.IsRefillingWater = true;
            }
            else {
                PlayerItems.Instance.IsRefillingWater = false;
            }
        }


        if (Input.GetKeyUp(KeyCode.R) || PlayerItems.Instance.CurrentWater >= PlayerItems.Instance.WaterMaxLimit)
        {
            PlayerItems.Instance.IsRefillingWater = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) { 
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
