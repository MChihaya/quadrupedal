using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gene2 {
    public NNBrain robotBrain;
    public List<float> legSizes;
    public List<float> bodySizes;
    public int name;
    public float reward;


    public Gene2(int inputSize, int hiddenSize, int hiddenLayers, int outputSize, int numLegSizes) {
        robotBrain = new NNBrain(inputSize, hiddenSize, hiddenLayers, outputSize);
        legSizes = new List<float>(numLegSizes);
        for (int i = 0; i < numLegSizes; i++) {
            legSizes.Add(0.25f);
        }
        bodySizes = new List<float>(3);
        for (int i = 0; i < 3; i++) {
            bodySizes.Add(2.0f);
        }
    }
}