using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TotemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public Totem Totem { get; set; }
    [SerializeField] private Image image;
    [SerializeField] private Image border;
    public UnityEvent<TotemSlot> onMouseEvent;
    public UnityEvent onMouseExit;
    private Color highlightColor = new Color32(0, 255, 173, 255);

    public void Hide() {
        image.color = Color.clear;
        border.color = Color.clear;
    }

    public void Show() {
        image.color = new Color(1, 0, 0, 0.18f);
        border.color = Color.white;
    }

    public void Active() {
        Debug.Log(Totem);
        border.color = highlightColor;
        Totem.Activation = true;
    }

    public void Deactive() {
        Debug.Log(Totem);
        border.color = Color.white;
        Totem.Activation = false;
    }

    public void SetOff() {
        Deactive();
        image.color = new Color(1, 1, 1, 0.2f);
    }

    public void SetImage(Totem totem) {
        this.Totem = totem;
        image.sprite = totem.Image;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        onMouseEvent?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData) {
        onMouseExit?.Invoke();
    }

    public void SetTotem(Totem totem) {
        this.Totem = totem;
    }
}
