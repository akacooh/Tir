using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour, IGameStateChangeResponder
{
    [SerializeField] private GameObject enemy;
    [SerializeField] private float spawnDelay;
    [SerializeField] private int maxTargets;

    //used for same spawn delay after pause:
    private float tempSpawnDelay;
    private float lastSpawnTime;

    private BoxCollider spawnArea;
    private Vector3 min;
    private Vector3 max;
    private List<GameObject> targetDummies; //contains childs(responsible for models) of actual objects
    
    public UnityEvent<GameObject> newTargetCreated;

    void Start()
    {
        targetDummies = new List<GameObject>();

        spawnArea = GetComponent<BoxCollider>();
        min = spawnArea.transform.TransformPoint(spawnArea.center - spawnArea.size * 0.5f);
        max = spawnArea.transform.TransformPoint(spawnArea.center + spawnArea.size * 0.5f);
    }

    IEnumerator SpawnTick() {
        while(true) {
            yield return new WaitForSeconds(tempSpawnDelay);
            SpawnEnemy();
        }
    }

    //instantiated prefabs are saved in targetDummies. only a child(model) going to Game script for event subscribing; 
    void SpawnEnemy() {
        int freeSpot = CheckAvailableTarget();
        lastSpawnTime = Time.time;

        if (freeSpot == -1) return;
        if (freeSpot < targetDummies.Count) {
            targetDummies[freeSpot].transform.parent.position = GetRandomPosition();            
            targetDummies[freeSpot].SetActive(true);
        } else {
            var newTarget = Instantiate(enemy, GetRandomPosition(), Quaternion.identity, transform);
            GameObject model = newTarget.transform.GetChild(0).gameObject;
            targetDummies.Add(model);
            newTargetCreated.Invoke(model);
        }
    }

    int CheckAvailableTarget() {
        if (targetDummies.Count == maxTargets) return -1;

        int i;
        for (i = 0; i < targetDummies.Count; i++) {
            if (!targetDummies[i].activeSelf) {
                return i;
            }
        }
        return i + 1;
    }

    Vector3 GetRandomPosition() {
        bool spaceIsFree = false;
        float rndX = 0, rndY = 0, rndZ = 0;
        while (!spaceIsFree) {
            rndX = Random.Range(min.x, max.x);
            rndY = Random.Range(min.y, max.y);
            rndZ = Random.Range(min.z, max.z);
            spaceIsFree = !Physics.CheckSphere(new Vector3(rndX, rndY, rndZ), 0.5f, LayerMask.GetMask("Enemy"));
        }
        return new Vector3(rndX, rndY, rndZ);        
    }

    private void DisableAllTargets() {
        foreach (var target in targetDummies) {
            target.SetActive(false);
        }
    }

    public void OnStateChanged(GameState state) {
        //pause
        if (state == GameState.Pause) {
            gameObject.SetActive(false);
            tempSpawnDelay = spawnDelay - (Time.time - lastSpawnTime);

        //start
        } else if (state == GameState.Start){
            gameObject.SetActive(false);
            tempSpawnDelay = spawnDelay;

        //normal
        } else if (state == GameState.Play) {
            gameObject.SetActive(true);
            StartCoroutine(SpawnTick());
            tempSpawnDelay = spawnDelay;

        //end game
        } else if (state == GameState.End) {
            DisableAllTargets();
            gameObject.SetActive(false);
            
        } else {
            throw new UnityException($"No behaviour for state {state}");
        }
    }
}
