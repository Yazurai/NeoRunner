using UnityEngine;

public class ControlDemo : MonoBehaviour {
    public Rigidbody Rb;

    void Update() {
        Rb.AddForce(Input.GetAxis("Horizontal") * Vector3.forward * 2);
        Rb.AddForce(Input.GetAxis("Vertical") * Vector3.right * 2);
    }
}
