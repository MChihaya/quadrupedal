// SerialID: [77a855b2-f53d-4b80-9c94-c40562952b74]
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System;

[Serializable]
public class NNBrain : Brain
{

    private float MutationRate { get; set; } = 0.20f;
    private float RandomMin { get; set; } = -1;
    private float RandomMax { get; set; } = 1;

    // List of biases and weights of each layer
    // 重み
    [SerializeField] private List<Matrix> weights = new List<Matrix>();
    public List<Matrix> Weights { get { return weights; } }
    // バイアス
    [SerializeField] private List<Matrix> biases = new List<Matrix>();
    public List<Matrix> Biases { get { return biases; } }
    // 入力値の次元
    [SerializeField] private int inputSize = 0;
    public int InputSize { get { return inputSize; } private set { inputSize = value; } }
    // 隠れ層のノードの数
    [SerializeField] private int hiddenSize = 0;
    public int HiddenSize { get { return hiddenSize; } private set { hiddenSize = value; } }
    // 隠れ層の層の数
    [SerializeField] private int hiddenLayers = 0;
    public int HiddenLayers { get { return hiddenLayers; } private set { hiddenLayers = value; } } // may be equal to 0
    // 出力の次元
    [SerializeField] private int outputSize = 0;
    public int OutputSize { get { return outputSize; } private set { outputSize = value; } }
    // 観測値を得る
    public double[] GetAction(List<double> observation) {
        if (observation.Count != InputSize) {
            throw new ArgumentException($"Input size mismatch: observation.Count ({observation.Count}) does not match the expected InputSize ({InputSize}). Please check if selected sensors match for the trained data and your custom brain.");
        }
        var action = Predict(observation.ToArray());
        return action;
    }
    // 初期化定義
    public NNBrain(int inputSize, int hiddenSize, int hiddenLayers, int outputSize) {
        InputSize = inputSize;
        OutputSize = outputSize;
        HiddenLayers = hiddenLayers;
        HiddenSize = hiddenSize;
        CreateMatrix(inputSize, hiddenSize, hiddenLayers, outputSize);
        InitAllMatrix();//行列をランダムに初期化する
    }
    // コピー
    public NNBrain(NNBrain other) {
        InputSize = other.InputSize;
        OutputSize = other.OutputSize;
        HiddenLayers = other.HiddenLayers;
        HiddenSize = other.HiddenSize;

        for(int i = 0; i < other.Weights.Count; i++) {
            Matrix w = other.Weights[i].Copy();
            Matrix b = other.Biases[i].Copy();
            Weights.Add(w);
            Biases.Add(b);
        }
    }
    // 行列の作成：各層の計算を行列で表している。
    private void CreateMatrix(int inputSize, int hiddenSize, int hiddenLayers, int outputSize) {
        for(int i = 0; i < hiddenLayers + 1; i++) {
            int inSize = (i == 0) ? inputSize : hiddenSize;
            int outSize = (i == hiddenLayers) ? outputSize : hiddenSize;
            Weights.Add(new Matrix(inSize, outSize));
            Biases.Add(new Matrix(1, outSize));
        }
    }

    // 順方向に計算する。
    public double[] Predict(double[] inputs) {
        var output = new Matrix(inputs);
        var result = new double[OutputSize];
        for(int i = 0; i < HiddenLayers + 1; i++) {
            if (output.Column != Weights[i].Row) {
                throw new InvalidOperationException($"Dimension mismatch: output.Columns ({output.Column}) and Weights[{i}].Rows ({Weights[i].Row}) do not match. Please check sensor count.");
            }
            output = output.Mul(Weights[i]);
            var b = Biases[i];
            if(i != HiddenLayers) {
                for(int c = 0; c < b.Column; c++) {
                    output[0, c] = Tanh(output[0, c] + b[0, c]);
                }
            }
            else {
                for(int c = 0; c < b.Column; c++) {
                    output[0, c] = output[0, c] + b[0, c];
                }
            }
        }
        for(int c = 0; c < OutputSize; c++) {
            result[c] = output[0, c];
        }
        return result;
    }
    //　活性化：シグモイド関数
    private float Sigmoid(double x) {
        return 1 / (1 - Mathf.Exp(-1 * (float)x));
    }
    // 活性化：ハイパボリック
    private double Tanh(double x) {
        return Math.Tanh(x);
    }

    public void SetDNA(double[] dna, bool mutation = true) {
        var index = 0;
        foreach(var b in Biases) {
            index = SetDNA(b, dna, index);
        }
        foreach(var w in Weights) {
            index = SetDNA(w, dna, index);
        }
    }
    // 行列の値をdnaにする。
    public double[] ToDNA() {
        var dna = new List<double>();
        foreach(var b in Biases) {
            dna.AddRange(b.ToArray());
        }
        foreach(var w in Weights) {
            dna.AddRange(w.ToArray());
        }
        return dna.ToArray();
    }
    // dnaを行列の値としてセットする。
    private int SetDNA(Matrix m, double[] dna, int index) {
        for(int r = 0; r < m.Row; r++) {
            for(int c = 0; c < m.Column; c++) {
                m[r, c] = dna[index];
                index++;
            }
        }

        return index;
    }
    // 行列の初期化：ランダムにする。
    private void InitAllMatrix() {
        foreach(Matrix m in Biases) {
            InitMatrix(m);
        }
        foreach(Matrix m in Weights) {
            InitMatrix(m);
        }
    }

    private void InitMatrix(Matrix m) {
        for(int r = 0; r < m.Row; r++) {
            for(int c = 0; c < m.Column; c++) {
                m[r, c] = UnityEngine.Random.Range(RandomMin, RandomMax);
            }
        }
    }
    // 変異を行う：一定の確率で丸々ランダム化して、一定の確率で乱数で変わる。
    public NNBrain Mutate(int generation) {
        var c = new NNBrain(this);
        for(int i = 0; i < c.HiddenLayers + 1; i++) {
            c.Biases[i] = MutateLayer(Biases[i], generation);
            c.Weights[i] = MutateLayer(Weights[i], generation);
        }
        return c;
    }

    private Matrix MutateLayer(Matrix m, int generation) {
        var newM = m.Copy();
        float mutRate = MutRate(generation) + 0.1f;
        var mutSize = MutRate(generation) * 0.2f + 0.02f;
        for(int r = 0; r < m.Row; r++) {
            for(int c = 0; c < m.Column; c++) {
                var mut = UnityEngine.Random.value;
                if(mut < mutRate * 0.05) {
                    var X = UnityEngine.Random.value;
                    var Y = UnityEngine.Random.value;
                    var Z1 = (float)Math.Sqrt(-2 * Math.Log(X)) * (float)Math.Cos(2 * Math.PI * Y);
                    newM[r, c] = Z1;
                }
                else if(mut < mutRate) {
                    var X = UnityEngine.Random.value;
                    var Y = UnityEngine.Random.value;
                    var Z1 = (float)Math.Sqrt(-2 * Math.Log(X)) * (float)Math.Cos(2 * Math.PI * Y);
                    newM[r, c] = m[r, c] + Z1 * mutSize;
                }
            }
        }
        return newM;
    }
    // 世代を追うごとに変異確率を下げていく
    private float MutRate(int generation) {
        return 0.2f * (float)Math.Max(0, 1 - generation / 100);
    }

    public override void Save(string path) {
        var json = JsonUtility.ToJson(this);
        File.WriteAllText(path, json);
    }

    public static NNBrain Load(TextAsset asset) {
        return JsonUtility.FromJson<NNBrain>(asset.text);
    }
}
