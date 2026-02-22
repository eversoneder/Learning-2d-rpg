using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "DialogueControl", menuName = "Scriptable Objects/DialogueControl")]
public class DialogueControl : MonoBehaviour
{
    private Player player;
    //private NPC_Dialogue npcDialogue;
    //private bool hasPlayerPressedT;

    [System.Serializable]
    public enum idiom
    {
        pt,
        eng,
        spa,
    }
    public idiom language;

    [Header("Components")]
    public GameObject dialogueObj;
    public Image profileSprite;
    public Text speechText;
    //public TextMeshPro speechText;
    public Text actorNameText;

    [Header("Settings")]
    public float typingSpeed; //velocidade da fala do NPC

    //control variables
    private int index; //how many NPC sentences
    private bool isDialogueShowing;

    private string[] sentences;
    private string[] currentActorName;
    private Sprite[] actorSprite;


    public bool IsDialogueShowing { get => isDialogueShowing; set => isDialogueShowing = value; }

    public static DialogueControl Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // If it does, destroy this duplicate to enforce the singleton pattern
            Destroy(this.gameObject);
        }
        else
        {
            // If this is the first instance, set it and mark it not to be destroyed
            Instance = this;

            player = GameObject.FindWithTag("Player").GetComponent<Player>();
            //npcDialogue = GameObject.Find("NPC_1").GetComponent<NPC_Dialogue>();
            

            DontDestroyOnLoad(this.gameObject); // Optional, but recommended for persistent singletons
        }
    }

    void Start()
    {
    }

    void Update()
    {
        //hasPlayerPressedT = npcDialogue.HasPlayerPressedT;
    }

    IEnumerator TypeSentence()
    {
        foreach (char letter in sentences[index].ToCharArray())
        {
            speechText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        } 
    }

    /// <summary>
    ///  skip current NPC sentence and go to the next sentence.
    /// </summary>
    public void NextSentence()
    {
        if (speechText.text == sentences[index])
        {
            if (index < sentences.Length - 1)
            {
                index++;
                profileSprite.sprite = actorSprite[index];
                actorNameText.text = currentActorName[index];
                speechText.text = "";
                StartCoroutine(TypeSentence());
            }
            else // when the dialogue texts are finished
            {
                speechText.text = "";
                actorNameText.text = ""; 
                index = 0;
                dialogueObj.SetActive(false);
                sentences = null;
                isDialogueShowing = false;
                player.isPaused = false;
                //npcDialogue.HasPlayerPressedT = false;
            }
        }
    }

    /// <summary>
    /// Call NPC speech.
    /// </summary>
    public void Speech(string[] txt, string[] actrName, Sprite[] actorSprt)
    {
        //if NPC is not speaking already
        if (!isDialogueShowing)
        {
            player.isPaused = true;
            dialogueObj.SetActive(true);
            sentences = txt;
            currentActorName = actrName;
            actorSprite = actorSprt;
            profileSprite.sprite = actorSprite[index];
            actorNameText.text = currentActorName[index];

            StartCoroutine(TypeSentence());
            isDialogueShowing = true;

        } else {
            NextSentence();
        }
        //NPC_Dialogue nd = new NPC_Dialogue();
        //if (!nd.playerIsInNPCRange)
        //{
        //    isDialogueShowing = false;
        //}
    }

    public bool GetIsDialogShowing()
    {
        return isDialogueShowing;
    }

    public void SetIsDialogShowing(bool isShowing)
    {
        isDialogueShowing = isShowing;
    }

    /// <summary>
    /// Closes the dialogue box and resets the state.
    /// </summary>
    public void CloseDialogue()
    {
        // Stop the typing coroutine to prevent errors
        StopAllCoroutines();

        // Reset dialogue UI elements
        speechText.text = "";
        actorNameText.text = "";

        // Reset control variables
        index = 0;
        dialogueObj.SetActive(false);
        sentences = null;
        isDialogueShowing = false;
        player.isPaused = false;
    }

}
