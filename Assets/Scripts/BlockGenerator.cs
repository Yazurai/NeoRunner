using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour {
    public int SpawnRate;
    public int Seed;

    public float BlockSize;

    public GameObject BlockPrefab;

    List<BlockController> InactivePool;
    List<BlockController> ActivePool;

    // Use this for initialization
    void Awake() {
        Random.InitState(Seed);

        InactivePool = new List<BlockController>();
        ActivePool = new List<BlockController>();

        //StartGame();
    }

    public void StartGame() {
        Random.InitState(Seed);
        StartCoroutine("Spawn");
    }

    public void EndGame() {
        for (int i = 0; i < ActivePool.Count; i++) {
            StopAllCoroutines();
            InactivePool.Add(ActivePool[i]);
            ActivePool[i].gameObject.SetActive(false);
        }
        ActivePool.Clear();
    }

    IEnumerator Spawn() {
        while (true) {
            for (int i = 0; i < Random.Range(1, 3); i++) {
                ActivateBlock();
            }
            yield return new WaitForSeconds(0.2f * Mathf.RoundToInt(Random.Range(2, SpawnRate)));
        }
    }

    void ActivateBlock() {
        if (InactivePool.Count > 0) {
            BlockController NewBlock = InactivePool[0];
            InactivePool.RemoveAt(0);
            ActivePool.Add(NewBlock);
            NewBlock.Activate(new Vector3(-25, .5f, Mathf.RoundToInt(Random.Range(-5, 6))), Random.Range(0.9f, BlockSize), this);
        } else {
            GameObject newBlock = Instantiate(BlockPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            ActivePool.Add(newBlock.GetComponent<BlockController>());
            newBlock.GetComponent<BlockController>().Activate(new Vector3(-25, .5f, Mathf.RoundToInt(Random.Range(-5, 6))), Random.Range(0.9f, BlockSize), this);
        }
    }

    public void DeactivateBlock(BlockController block) {
        ActivePool.Remove(block);
        InactivePool.Add(block);
    }
}
