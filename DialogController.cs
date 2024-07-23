using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DialogController : MonoBehaviour, IPointerDownHandler {
    [SerializeField] private TextMeshProUGUI dialog;
    private Queue<string> sentences = new Queue<string>();
    private string currentSentence;
    [SerializeField] private float typingDelay = 0.1f;
    private bool isTyping = false;
    public UnityEvent dialogEndEvent = new UnityEvent();

    public void SetDialogSentences(string[] lines) {
        sentences.Clear();
        foreach (string line in lines) {
            sentences.Enqueue(line);
        }
    }

    public void PassToNextSentence() {
        if(sentences.Count <= 0) {
            dialogEndEvent?.Invoke();
            return;
        }
        currentSentence = sentences.Dequeue();
        StartCoroutine(KeepTypingSentence());
    }

    private IEnumerator KeepTypingSentence() {
        isTyping = true;
        dialog.text = string.Empty;
        foreach (char letter in currentSentence.ToCharArray()) {
            dialog.text += letter;
            yield return new WaitForSeconds(typingDelay);
        }
        isTyping = false;
    }

    public void OnPointerDown(PointerEventData eventData) {
        if(isTyping) {
            StopAllCoroutines();
            dialog.text = currentSentence;
            isTyping = false;
        }
        else {
            PassToNextSentence();
        }
    }
}
