using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public int Sensors;
    public Rigidbody Rb;
    private bool Active = true;
    public int seed;
    public TrainingManager Tm;
    public bool InfoFromDNA;
    public string DNA;

    public NeuralNetwork NN;

    // Use this for initialization
    void Awake() {
        NN.InputCount = 3;
        NN.HiddenLayerCount = 3;
        NN.OutputCount = 3;
        NN.Initialise();
        //Set(seed);
        if (InfoFromDNA) {
            NN.Set(DNA);
        }
        StartCoroutine("Test");
    }

    public bool IsActive() {
        return Active;
    }

    public float getFitness() {
        return NN.GetFitness();
    }

    public void Set(int seed) {
        this.seed = seed;
        NN.Set(seed);
    }

    IEnumerator Test() {
        while (Active) {
            Step();
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void Step() {
        float[] Inputs = Scan();
        float[] Prediction = NN.GetOutput(Inputs);

        Vector3 NextPos = gameObject.transform.position;

        switch (OutputToDirection(Prediction)) {
            case 0:
                if (gameObject.transform.position.z > -5) {
                    NextPos.z -= 1;
                    Rb.position = NextPos;
                }
                break;
            case 1:
                break;
            case 2:
                if (gameObject.transform.position.z < 5) {
                    NextPos.z += 1;
                    Rb.position = NextPos;
                }
                break;
            default:
                break;
        }

        NN.AddToFitness(ScanFitness());
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Block") {
            Deactivate();
        }
    }

    private void Deactivate() {
        Active = false;
        Tm.SetInactive();
        gameObject.SetActive(false);
    }

    private int OutputToDirection(float[] output) {
        int MaxIndex = 0;
        for (int i = 0; i < output.Length; i++) {
            if (output[i] > output[MaxIndex]) {
                MaxIndex = i;
            }
        }
        return MaxIndex;
    }

    public float ScanFitness() {
        float Fitness = 0;
        RaycastHit hit;

        if (Physics.Raycast(gameObject.transform.position, new Vector3(-1, 0, 0), out hit)) {
            Fitness = hit.distance;
        } else {
            Fitness = 60;
        }
        return Fitness;
    }

    public float[] Scan() {
        float[] Inputs = new float[3];
        for (int i = 0; i < Inputs.Length; i++) {
            RaycastHit hit;
            Vector3 RayPos = gameObject.transform.position;

            RayPos.x += 1;
            RayPos.z += -(Inputs.Length - 1) / 2 + i;

            if (Physics.Raycast(RayPos, new Vector3(-1, 0, 0), out hit, 1 << 8)) {
                if (hit.transform.gameObject.name == "Wall") {
                    Inputs[i] = 0;
                } else {
                    Inputs[i] = hit.distance;
                }
            } else {
                Inputs[i] = 250;
            }
        }

        return Inputs;
    }
}
