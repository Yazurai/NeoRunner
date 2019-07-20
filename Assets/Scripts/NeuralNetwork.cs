using System;
using UnityEngine;

public class NeuralNetwork : MonoBehaviour {
    public int InputCount;
    public int HiddenLayerCount;
    public int OutputCount;
    private float Fitness;

    private float[] Inputs;
    private float[][] HLWeights;
    private float[] HLBiases;
    private float[][] OutputWeights;
    private float[] OutputBiases;

    public NeuralNetwork() {
        this.InputCount = 0;
        this.HiddenLayerCount = 0;
        this.OutputCount = 0;
        this.Fitness = 0;
        Initialise();
    }

    public NeuralNetwork(int i, int hl, int o) {
        this.InputCount = i;
        this.HiddenLayerCount = hl;
        this.OutputCount = o;
        this.Fitness = 0;
        Initialise();
    }

    private void Start() {
        UnityEngine.Random.InitState(Mathf.RoundToInt(Time.time));
    }

    public float GetFitness() {
        return Fitness;
    }

    public void AddToFitness(float amount) {
        Fitness += amount;
    }

    public void Initialise() {
        Inputs = new float[InputCount];

        HLWeights = new float[HiddenLayerCount][];
        for (int i = 0; i < HiddenLayerCount; i++) {
            HLWeights[i] = new float[InputCount];
        }

        HLBiases = new float[HiddenLayerCount];

        OutputWeights = new float[OutputCount][];
        for (int i = 0; i < OutputCount; i++) {
            OutputWeights[i] = new float[HiddenLayerCount];
        }
        OutputBiases = new float[OutputCount];
    }

    public void Set(float[] values) {
        int Index = 0;
        for (int i = 0; i < HiddenLayerCount; i++) {
            for (int j = 0; j < InputCount; j++) {
                HLWeights[i][j] = values[Index];
                Index++;
            }
        }
        for (int i = 0; i < HiddenLayerCount; i++) {
            HLBiases[i] = values[Index];
            Index++;
        }
        for (int i = 0; i < OutputCount; i++) {
            for (int j = 0; j < HiddenLayerCount; j++) {
                OutputWeights[i][j] = values[Index];
                Index++;
            }
        }
        for (int i = 0; i < OutputCount; i++) {
            OutputBiases[i] = values[Index];
            Index++;
        }
    }

    public void Set(string values) {
        string[] SplitString = values.Split(',');
        float[] ParsedValues = Array.ConvertAll(SplitString, float.Parse);
        Set(ParsedValues);
    }

    public void Set(int seed) {
        float[] values = new float[ValueCount()];
        UnityEngine.Random.InitState(seed);
        for (int i = 0; i < values.Length; i++) {
            values[i] = UnityEngine.Random.Range(0, 1f);
        }
        Set(values);
    }

    public int ValueCount() {
        int sum = InputCount * HiddenLayerCount + HiddenLayerCount + OutputCount * HiddenLayerCount + OutputCount;
        return sum;
    }

    public float[] ToArray() {
        float[] Output = new float[ValueCount()];
        int Index = 0;
        for (int i = 0; i < HiddenLayerCount; i++) {
            for (int j = 0; j < InputCount; j++) {
                Output[Index] = HLWeights[i][j];
                Index++;
            }
        }
        for (int i = 0; i < HiddenLayerCount; i++) {
            Output[Index] = HLBiases[i];
            Index++;
        }
        for (int i = 0; i < OutputCount; i++) {
            for (int j = 0; j < HiddenLayerCount; j++) {
                Output[Index] = OutputWeights[i][j];
                Index++;
            }
        }
        for (int i = 0; i < OutputCount; i++) {
            Output[Index] = OutputBiases[i];
            Index++;
        }
        return Output;
    }

    override
    public string ToString() {
        string Output = "";
        for (int i = 0; i < HiddenLayerCount; i++) {
            for (int j = 0; j < InputCount; j++) {
                Output += HLWeights[i][j] + ",";
            }
        }
        for (int i = 0; i < HiddenLayerCount; i++) {
            Output += HLBiases[i] + ",";
        }
        for (int i = 0; i < OutputCount; i++) {
            for (int j = 0; j < HiddenLayerCount; j++) {
                Output += OutputWeights[i][j] + ",";
            }
        }
        for (int i = 0; i < OutputCount; i++) {
            Output += OutputBiases[i] + ",";
        }
        return Output;
    }

    private float RELU(float n) {
        return (n > 0) ? n : 0;
    }

    private float MatriceSum(float[] a, float[] b) {
        float sum = 0;
        for (int i = 0; i < a.Length; i++) {
            sum += a[i] * b[i];
        }
        return sum;
    }

    private float SquaredError(float[] expected, float[] actual) {
        float sum = 0;
        for (int i = 0; i < expected.Length; i++) {
            sum += 0.5f * Mathf.Pow(expected[i] - actual[i], 2);
        }
        return sum;
    }

    public float[] GetOutput(float[] inputs) {
        Inputs = inputs;
        float[] HLActivations = new float[HiddenLayerCount];
        for (int i = 0; i < HiddenLayerCount; i++) {
            HLActivations[i] = RELU(MatriceSum(Inputs, HLWeights[i]) + HLBiases[i]);
        }
        float[] Outputs = new float[OutputCount];
        for (int i = 0; i < OutputCount; i++) {
            Outputs[i] = RELU(MatriceSum(HLActivations, OutputWeights[i]) + OutputBiases[i]);
        }
        return Outputs;
    }
}
