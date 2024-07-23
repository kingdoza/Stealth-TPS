using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientManager : MonoBehaviour {
    public Client[] Clients { get; private set; }
    public Client CurrentClient;

    private void Awake() {
        Clients = GetComponentsInChildren<Client>();
        InitClients();
        ChooseRandomClient();
    }

    private void InitClients() {
        for(int i = 0 ; i < Clients.Length; ++i) {
            Clients[i].Init();
        }
    }

    public void ChooseRandomClient() {
        int randomIndex = Random.Range(0, Clients.Length);
        CurrentClient = Clients[randomIndex];
    }
}
