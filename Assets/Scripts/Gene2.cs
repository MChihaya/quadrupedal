using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gene2 {
    public List<float> angles;
    public List<float> legSizes;
    public List<float> bodySizes;
    public int name;
    public List<float> springs;
    public List<float> dumpers;


    public Gene2(int numAngles, int numLegSizes, int numSprings, int numDumpers) {
        angles = new List<float>(numAngles);
        for (int i = 0; i < numAngles; i++) {
            angles.Add(Random.Range(-90.0f, 90.0f));
        }
        springs = new List<float>(numSprings);
        for (int i = 0; i < numSprings; i++) {
            springs.Add(Random.Range(0.0f, 100.0f));
        }
        dumpers = new List<float>(numDumpers);
        for (int i = 0; i < numDumpers; i++) {
            dumpers.Add(Random.Range(0.0f, 10.0f));
        }
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