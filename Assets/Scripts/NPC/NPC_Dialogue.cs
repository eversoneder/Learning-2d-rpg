using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class NPC_Dialogue : MonoBehaviour
{
    public float dialogueRange;
    public LayerMask playerLayer;

    public DialogueSettings dialogueSettings;
    private List<string> sentences = new List<string>();
    private List<Sprite> actorSprite = new List<Sprite>();
    private List<string> actorName = new List<string>();

    float currentNPCRangeWarningTimer = 0f;
    float cooldownDuration = 3f; // Duration of cooldown in seconds

    public bool playerIsInNPCRange;
    private bool hasPlayerPressedT;

    public bool HasPlayerPressedT { get => hasPlayerPressedT; set => hasPlayerPressedT = value; }

    private void Start()
    {
        GetNPCDialogue();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && playerIsInNPCRange) {
            DialogueControl.Instance.Speech(sentences.ToArray(), actorName.ToArray(), actorSprite.ToArray());
            hasPlayerPressedT = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (hasPlayerPressedT)
        //{ 
            ShowDialogue();
            //Debug.Log("hasPlayerPressedT = true."); 
        //}
    }

    void ShowDialogue()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, dialogueRange, playerLayer);

        // Reduce cooldown over time
        if (currentNPCRangeWarningTimer > 0f) 
        {
            currentNPCRangeWarningTimer -= Time.fixedDeltaTime;
        } else
        {
            if (hit != null)
            {
                Debug.Log("Player is inside NPC's dialog range.");
                currentNPCRangeWarningTimer = cooldownDuration; // Reset cooldown
                playerIsInNPCRange = true;
            }
            else {
                playerIsInNPCRange = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, dialogueRange);
    }

    void GetNPCDialogue()
    {
        for (int i = 0; i < dialogueSettings.dialogues.Count; i++) {

            switch (DialogueControl.Instance.language)
            {
                case DialogueControl.idiom.pt:
                    sentences.Add(dialogueSettings.dialogues[i].sentence.portuguese);
                    break;
                case DialogueControl.idiom.eng:
                    sentences.Add(dialogueSettings.dialogues[i].sentence.english);
                    break;
                case DialogueControl.idiom.spa:
                    sentences.Add(dialogueSettings.dialogues[i].sentence.spanish);
                    break;
                default:
                    break;
            }

            actorSprite.Add(dialogueSettings.dialogues[i].profile);
            actorName.Add(dialogueSettings.dialogues[i].actorName);
            
        }
    }
}
