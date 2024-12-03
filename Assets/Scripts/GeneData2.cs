using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GeneData2 {
    public List<float> angles;
    public List<float> legSizes;
    public List<float> bodySizes;
    public List<float> springs;
    public List<float> dumpers;
    public int name;
    public int generation;
    public float distance;
    public GeneData2(Gene2 gene) {
        angles = new List<float>(gene.angles);
        legSizes = new List<float>(gene.legSizes);
        bodySizes = new List<float>(gene.bodySizes);
        name = gene.name;
        springs = new List<float>(gene.springs);
        dumpers = new List<float>(gene.dumpers);
    }
}

[System.Serializable]
public class GeneDataList2 {
    public List<GeneData2> geneDatas;

    public GeneDataList2(List<GeneData2> geneDatas) {
        this.geneDatas = geneDatas;
    }
}

