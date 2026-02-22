using UnityEngine;

public class PlayerItems : MonoBehaviour
{
    //currents
    [Header("Current Materials")]
    [SerializeField] private float currentWater;
    [SerializeField] private int currentWood;
    [SerializeField] private int currentCarrot;
    [SerializeField] private int currentFish;

    //max materials
    [Header("Max Materials")]
    [SerializeField] private float waterMaxLimit = 50f;
    [SerializeField] private float woodMaxLimit = 50f;
    [SerializeField] private float carrotMaxLimit = 50f;
    [SerializeField] private float fishMaxLimit = 10f;

    [Header("Settings")]
    //water
    [SerializeField] private float waterMinLimit = 0f;
    [SerializeField] private float waterPourRate;
    [SerializeField] private bool isFillingUpWater;
    [SerializeField] private bool isFishing;

    //wood
    [SerializeField] private float woodMinLimit = 0f;

    //carrot
    [SerializeField] private float carrotMinLimit = 0f;

    //fish
    [SerializeField] private float fishMinLimit = 0f;

    #region Monobehaviour Singleton Within Awake
    public static PlayerItems Instance { get; private set; }

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            // If it does, destroy this duplicate to enforce the singleton pattern
            Destroy(this.gameObject);
        }
        else
        {
            // If this is the first instance, set it and mark it not to be destroyed
            Instance = this;
            DontDestroyOnLoad(this.gameObject); // Optional, but recommended for persistent singletons
        }



        if (waterPourRate == 0)
        {
            waterPourRate = 0.2f;
        }
    }
    #endregion

    //water getter & setter
    public float CurrentWater { get => currentWater; set => currentWater = value; }
    public float WaterMinLimit { get => waterMinLimit; set => waterMinLimit = value; }
    public float WaterMaxLimit { get => waterMaxLimit; set => waterMaxLimit = value; }
    public bool IsRefillingWater { get => isFillingUpWater; set => isFillingUpWater = value; }
    public float WaterPourRate { get => waterPourRate; set => waterPourRate = value; }

    //wood getter & setter
    public int CurrentWood { get => currentWood; set => currentWood = value; }
    public float WoodMinLimit { get => woodMinLimit; set => woodMinLimit = value; }
    public float WoodMaxLimit { get => woodMaxLimit; set => woodMaxLimit = value; }

    //carrot getter & setter
    public int CurrentCarrot { get => currentCarrot; set => currentCarrot = value; }
    public float CarrotMinLimit { get => carrotMinLimit; set => carrotMinLimit = value; }
    public float CarrotMaxLimit { get => carrotMaxLimit; set => carrotMaxLimit = value; }

    //fish getter & setter
    public int CurrentFish { get => currentFish; set => currentFish = value; }
    public float FishMaxLimit { get => fishMaxLimit; set => fishMaxLimit = value; }
    public float FishMinLimit { get => fishMinLimit; set => fishMinLimit = value; }
    public bool IsHuntingFish { get => isFishing; set => isFishing = value; }

    /// <summary>
    /// add to current water and has a limit so it doesn't exceed
    /// </summary>
    /// <param name="waterAddRate"> water Rate to add </param>
    public void AddWater(float waterAddRate)
    {
        //adicionar água e automaticamente limitar o valor máximo
        currentWater = Mathf.Min(currentWater + waterAddRate, waterMaxLimit);
    }

    public void PourWater()
    {
        //remover água e automaticamente limitar o valor minimo pra 0
        currentWater = Mathf.Max(currentWater - waterPourRate, 0);

        //currentWater -= 0.2f;
        //if (currentWater >= 0)
        //{
        //    currentWater = currentWater - playerPourWaterRate;
        //}
    }

    public bool HasFood()
    {
        if (currentCarrot > 0 || currentFish > 0)
        {
            return true;
        }
        return false;
    }

    public bool EatFood()
    {
        if (currentCarrot > 0)
        {
            currentCarrot--;
            return true;
        }
        else if (currentFish > 0)
        {
            currentFish--;
            return true;
        }

        if (currentCarrot <= 0 || currentFish <= 0)
        {
            Debug.Log("There's no food in Player Inventory.");
        }
        return false;
    }
}
