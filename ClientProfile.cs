using System;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;

public class ClientProfile : MonoBehaviour {
    [SerializeField] private Client client;
    [SerializeField] private TextMeshProUGUI nameUI;
    [SerializeField] private TextMeshProUGUI genderUI;
    [SerializeField] private TextMeshProUGUI ageUI;
    [SerializeField] private TextMeshProUGUI jobUI;
    [SerializeField] private TextMeshProUGUI residenceUI;
    [SerializeField] private TextMeshProUGUI sensitivityUI;
    [SerializeField] private Image[] totemsUI;
    [SerializeField] private RectTransform sleepGraphContainer;
    [SerializeField] private TextMeshProUGUI quoteUI;
    [SerializeField] private TextMeshProUGUI storyUI;
    [SerializeField] private DialogController dialogController;
    [SerializeField] private TotemSlots totemSlots;
    [SerializeField] private GameObject gameStartButton;

    private void Start () {
        gameStartButton.SetActive(false);
        dialogController.dialogEndEvent.AddListener(OnDialogEnd);
        client = GameManager.Instance.clientManager.CurrentClient;
        SetProfileUI();
    }

    private void SetProfileUI() {
        nameUI.text = client.Name;
        genderUI.text = client.Gender.ToLocalizedString();
        ageUI.text = client.Age + "세";
        jobUI.text = client.Job;
        residenceUI.text = client.Residence;
        sensitivityUI.text = client.Sensitivity.ToLocalizedString();
        sensitivityUI.color = client.Sensitivity.ToLocalizedColor();
        client.SleepGraph.DrawAt(sleepGraphContainer);
        quoteUI.text = client.Quote;
        dialogController.SetDialogSentences(client.Story);
        dialogController.PassToNextSentence();
        totemSlots.SetSlots(client.Totems, true);
        //storyUI.text = client.Story;
    }

    private void OnDialogEnd() {
        gameStartButton.SetActive(true);
    }
}
