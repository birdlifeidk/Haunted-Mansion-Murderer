using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HeneGames.DialogueSystem
{
    public class DialogueManager : MonoBehaviour
    {
          private int currentSentence;
    private float coolDownTimer;
    private bool dialogueIsOn;
    private DialogueTrigger dialogueTrigger;
    public Transform teleportBack; 

    private bool dialogueDone; 

    // NEW: track if player is inside the trigger
    private bool playerInTrigger = false;

    public enum TriggerState
    {
        Collision,
        Input
    }

    [Header("References")]
    [SerializeField] private AudioSource audioSource;

    [Header("Events")]
    public UnityEvent startDialogueEvent;
    public UnityEvent nextSentenceDialogueEvent;
    public UnityEvent endDialogueEvent;

    [Header("Dialogue")]
    [SerializeField] private TriggerState triggerState;
    [SerializeField] private List<NPC_Centence> sentences = new List<NPC_Centence>();

    private void Update()
    {
       

        // Timer
        if (coolDownTimer > 0f)
        {
            coolDownTimer -= Time.deltaTime;
        }

        if (triggerState == TriggerState.Input && playerInTrigger && Input.GetKeyDown(KeyCode.F1) && !dialogueIsOn && !dialogueDone)
        {
            Debug.Log("Dialogue Works");
            if (dialogueTrigger != null)
            {
                dialogueTrigger.startDialogueEvent.Invoke();
            }

            startDialogueEvent.Invoke();

            DialogueUI.instance.StartDialogue(this);

            DialogueUI.instance.ShowInteractionUI(false);

            dialogueIsOn = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (other.gameObject.TryGetComponent<DialogueTrigger>(out DialogueTrigger _trigger))
        {
            dialogueTrigger = _trigger;

            if (triggerState == TriggerState.Collision && !dialogueIsOn)
            {
                StartDialogue();
            }
            else if (triggerState == TriggerState.Input)
            {
                playerInTrigger = true;
                DialogueUI.instance.ShowInteractionUI(true);
            }
        }
    }
    

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInTrigger = false;
        DialogueUI.instance.ShowInteractionUI(false);

        // Only stop dialogue if it's still running
        if (dialogueIsOn)
            StopDialogue();

        dialogueTrigger = null;
    }

    public void StartDialogue()
        {
            if(dialogueTrigger != null)
            {
                dialogueTrigger.startDialogueEvent.Invoke();
            }

            currentSentence = 0;

            ShowCurrentSentence();

            PlaySound(sentences[currentSentence].sentenceSound);

            coolDownTimer = sentences[currentSentence].skipDelayTime;
        }

        public void NextSentence(out bool lastSentence)
        {
            if (coolDownTimer > 0f)
            {
                lastSentence = false;
                return;
            }

            currentSentence++;

            if (dialogueTrigger != null)
            {
                dialogueTrigger.nextSentenceDialogueEvent.Invoke();
            }

            nextSentenceDialogueEvent.Invoke();

            if (currentSentence > sentences.Count - 1)
            {
                StopDialogue();

                lastSentence = true;

                endDialogueEvent.Invoke();

                return;
            }

            lastSentence = false;

            PlaySound(sentences[currentSentence].sentenceSound);

            ShowCurrentSentence();

            coolDownTimer = sentences[currentSentence].skipDelayTime;
        }

        public void StopDialogue()
        {
            if (dialogueTrigger != null)
            {
                dialogueTrigger.endDialogueEvent.Invoke();
            }

            DialogueUI.instance.ClearText();

            if(audioSource != null)
            {
                audioSource.Stop();
            }

            dialogueDone = true;


            dialogueIsOn = false;
            dialogueTrigger = null;
        }

        private void PlaySound(AudioClip _audioClip)
        {
            if (_audioClip == null || audioSource == null)
                return;

            audioSource.Stop();

            audioSource.PlayOneShot(_audioClip);
        }

        private void ShowCurrentSentence()
        {
            if (sentences[currentSentence].dialogueCharacter != null)
            {
                DialogueUI.instance.ShowSentence(sentences[currentSentence].dialogueCharacter, sentences[currentSentence].sentence);

                sentences[currentSentence].sentenceEvent.Invoke();
            }
            else
            {
                DialogueCharacter _dialogueCharacter = new DialogueCharacter();
                _dialogueCharacter.characterName = "";
                _dialogueCharacter.characterPhoto = null;

                DialogueUI.instance.ShowSentence(_dialogueCharacter, sentences[currentSentence].sentence);

                sentences[currentSentence].sentenceEvent.Invoke();
            }
        }

        public int CurrentSentenceLenght()
        {
            if(sentences.Count <= 0)
                return 0;

            return sentences[currentSentence].sentence.Length;
        }
    }

    [System.Serializable]
    public class NPC_Centence
    {
        [Header("------------------------------------------------------------")]

        public DialogueCharacter dialogueCharacter;

        [TextArea(3, 10)]
        public string sentence;

        public float skipDelayTime = 0.5f;

        public AudioClip sentenceSound;

        public UnityEvent sentenceEvent;
    }
}