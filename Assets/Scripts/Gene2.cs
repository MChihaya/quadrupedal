using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Gene2 {
    public NNBrain robotBrain;
    public List<float> legSizes;
    public List<float> bodySizes;
    public int name;
    public float reward = 0f;


    public Gene2(int inputSize, int[] hiddenLayers, int outputSize, int numLegSizes) {
        robotBrain = new NNBrain(inputSize, hiddenLayers, outputSize);
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

[System.Serializable]
public class Movement {
    public List<float> angles;
    public List<float> bodySizes;
    public List<float> legSizes;
}