using System.Collections;
using UnityEngine;

public class BlockController : MonoBehaviour {
    private BlockGenerator Bg;

    public void Activate(Vector3 position, float width, BlockGenerator bg) {
        Bg = bg;
        gameObject.SetActive(true);
        gameObject.transform.position = position;
        gameObject.transform.localScale = new Vector3(0.9f, 0.9f, width);
        StartCoroutine("Move");
    }

    IEnumerator Move() {
        while (true) {
            Vector3 pos = gameObject.transform.position;
            pos.x++;
            gameObject.transform.position = pos;
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.name == "Start Block") {
            StopCoroutine("Move");
            Bg.DeactivateBlock(this);
            gameObject.SetActive(false);
        }
    }
}
