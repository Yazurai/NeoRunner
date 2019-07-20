using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TrainingManager : MonoBehaviour {
    public Vector3 StartPosition;
    public int CountPerGeneration;
    public GameObject Prefab;
    public int Seed;
    public int BlockSeed;
    public BlockGenerator BlockGen;
    public int ValueCount;
    public int speed;

    private int ActiveCount;
    List<GameObject> Players;

    public List<float> HighestFitness;
    public List<string> HighestFitnessDna;
    public List<float> AverageFitness;

    public void WriteAllData() {
        int FileNumber = 1;
        string FileName = "data";
        while (File.Exists(FileName + FileNumber)) {
            FileNumber++;
        }
        FileName += FileNumber;
        FileName += ".txt";
        StreamWriter Sw = new StreamWriter(FileName);
        Sw.WriteLine("Count per generation");
        Sw.WriteLine(CountPerGeneration);
        Sw.WriteLine("Generation count");
        Sw.WriteLine(HighestFitness.Count);
        Sw.WriteLine("Highest fitnesses");
        for (int i = 0; i < HighestFitness.Count; i++) {
            Sw.WriteLine(HighestFitness[i]);
        }
        Sw.WriteLine("Highest fitnesses (DNA)");
        for (int i = 0; i < HighestFitnessDna.Count; i++) {
            Sw.WriteLine(HighestFitnessDna[i]);
        }
        Sw.WriteLine("Average fitnesses");
        for (int i = 0; i < AverageFitness.Count; i++) {
            Sw.WriteLine(AverageFitness[i]);
        }
        Sw.Close();
    }

    private void Start() {
        Time.timeScale = speed;

        Players = new List<GameObject>();

        HighestFitness = new List<float>();
        HighestFitnessDna = new List<string>();
        AverageFitness = new List<float>();

        StartFirstGeneration();
    }

    public void StartFirstGeneration() {
        Random.InitState(Seed);

        //BlockGen.Seed = Random.Range(0, 10000);
        BlockGen.StartGame();

        Players.Clear();
        for (int i = 0; i < CountPerGeneration; i++) {
            GameObject newPlayer = Instantiate(Prefab, StartPosition, Quaternion.identity);
            newPlayer.GetComponent<PlayerController>().Set(Random.Range(0, 10000));
            newPlayer.GetComponent<PlayerController>().Tm = this;
            Players.Add(newPlayer);
        }
        ActiveCount = CountPerGeneration;
    }

    public void SetInactive() {
        ActiveCount--;
        if (ActiveCount <= 0) {
            BlockGen.EndGame();
            EvaluateGeneration();
        }
    }

    public void CreateNewGeneration(List<float[]> dnaData) {
        BlockGen.StartGame();

        Clear();
        for (int i = 0; i < CountPerGeneration; i++) {
            GameObject newPlayer = Instantiate(Prefab, StartPosition, Quaternion.identity);
            Players.Add(newPlayer);
            newPlayer.GetComponent<PlayerController>().NN.Set(dnaData[i]);
            newPlayer.GetComponent<PlayerController>().Tm = this;
        }
        ActiveCount = CountPerGeneration;
    }

    private void Clear() {
        for (int i = 0; i < Players.Count; i++) {
            Destroy(Players[i]);
        }
        Players.Clear();
    }

    public void EvaluateGeneration() {
        float max = 0;
        string maxDna = "";
        float sum = 0;
        for (int i = 0; i < Players.Count; i++) {
            if (max < Players[i].GetComponent<PlayerController>().getFitness()) {
                max = Players[i].GetComponent<PlayerController>().getFitness();
                maxDna = Players[i].GetComponent<PlayerController>().NN.ToString();
            }
            sum += Players[i].GetComponent<PlayerController>().getFitness();
        }
        float average = sum / Players.Count;

        HighestFitness.Add(max);
        HighestFitnessDna.Add(maxDna);
        AverageFitness.Add(average);

        CreateNewGeneration(EvolveDna());
    }

    private float[][] Crossover(int first, int second) {
        float[] FirstParent = Players[first].GetComponent<PlayerController>().NN.ToArray();
        float[] SecondParent = Players[second].GetComponent<PlayerController>().NN.ToArray();

        float[] FirstChild = new float[ValueCount];
        float[] SecondChild = new float[ValueCount];
        for (int i = 0; i < ValueCount; i++) {
            if (Random.Range(0, 2) == 0) {
                FirstChild[i] = FirstParent[i];
                SecondChild[i] = SecondParent[i];
            } else {
                FirstChild[i] = SecondParent[i];
                SecondChild[i] = FirstParent[i];
            }
        }
        float[][] returnValue = new float[2][];
        returnValue[0] = FirstChild;
        returnValue[1] = SecondChild;
        return returnValue;
    }

    public List<float[]> EvolveDna() {
        List<float[]> newGenerationDna = new List<float[]>();

        float Sum = AverageFitness[AverageFitness.Count - 1] * CountPerGeneration;
        for (int i = 0; i < (CountPerGeneration - 6) / 2; i++) {
            float Acc = 0;
            float Rand1 = Random.Range(0, Sum);
            float Rand2 = Random.Range(0, Sum);
            int FstDna = 0;
            int ScdDna = 0;

            for (int j = 0; j < CountPerGeneration; j++) {
                if (Rand1 >= Acc && Rand1 <= Acc + Players[j].GetComponent<PlayerController>().getFitness()) {
                    FstDna = j;
                }
                if (Rand2 >= Acc && Rand2 <= Acc + Players[j].GetComponent<PlayerController>().getFitness()) {
                    ScdDna = j;
                }

                Acc += Players[j].GetComponent<PlayerController>().getFitness();
            }

            float[][] newChildren = Crossover(FstDna, ScdDna);
            newGenerationDna.Add(newChildren[0]);
            newGenerationDna.Add(newChildren[1]);
        }

        for (int i = 0; i < 6; i++) {
            float[] newChild = new float[ValueCount];
            for (int j = 0; j < ValueCount; j++) {
                newChild[j] = Random.Range(0, 1f);
            }
            newGenerationDna.Add(newChild);
        }

        return newGenerationDna;
    }

    //0.1641158,0.899922,0.2659848,0.5886877,0.6645796,0.3119618,0.04158974,0.2639306,0.9880725,0.6294967,0.09893215,0.6426412,0.6037291,0.8179359,0.006338954,0.5279908,0.3507249,0.4387364,0.08021104,0.4338344,0.7026079,0.1391242,0.626871,0.08541703,
    //0.05342102,0.3115221,0.2770197,0.01327813,0.4860693,0.659402,0.5989395,0.1773937,0.04986048,0.9246622,0.3800393,0.6275007,0.8003786,0.5774825,0.4455187,0.05006289,0.7854749,0.04258835,0.6059451,0.9106202,0.2305104,0.582977,0.6806455,0.975076,
}
