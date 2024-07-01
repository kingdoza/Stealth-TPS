using UnityEngine;
using UnityEngine.AI;

public class NavMeshMove : MonoBehaviour {
    private NavMeshAgent nav;
    [SerializeField] private Transform targetPos;

    private void Start() {
        nav = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        nav.SetDestination(targetPos.position);
    }
}