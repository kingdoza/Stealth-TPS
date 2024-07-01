using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {
    [SerializeField] private GameObject prefab;
    private Queue<GameObject> pool = new Queue<GameObject>();

    protected void Awake() {
        Initialize(10);
    }

    public void Initialize(int _objectCount) {
        for(int i = 0; i < _objectCount; ++i) {
            MakeNewPooledObject();
        }
    }

    private GameObject MakeNewPooledObject() {
        GameObject gameObject = Instantiate(prefab, transform);
        gameObject.GetComponent<PooledObject>().ObjectPool = this;
        Insert(gameObject);
        return gameObject;
    }

    public void Insert(GameObject _object) {
        pool.Enqueue(_object);
        _object.SetActive(false);
    }

    public GameObject PullOut() => PullOut(Vector3.zero, Quaternion.identity);

    public GameObject PullOut(Vector3 _position) => PullOut(_position, Quaternion.identity);

    public GameObject PullOut(Quaternion _rotation) => PullOut(Vector3.zero, _rotation);

    public GameObject PullOut(Vector3 _position, Quaternion _rotation) {
        GameObject gameObject;
        if(pool.Count <= 0)
            gameObject = MakeNewPooledObject();
        gameObject = pool.Dequeue();
        gameObject.transform.position = _position;
        gameObject.transform.rotation = _rotation;
        gameObject.SetActive(true);
        return gameObject;
    }
}