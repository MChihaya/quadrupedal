using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GeneData2 {
    public List<double> robotdna;
    public List<float> legSizes;
    public List<float> bodySizes;
    public int name;
    public int generation;
    public float distance;
    public float reward;
    public GeneData2(Gene2 gene) {
        robotdna = gene.robotBrain.ToDNA().ToList<double>();
        legSizes = new List<float>(gene.legSizes);
        bodySizes = new List<float>(gene.bodySizes);
        name = gene.name;
    }
}

[System.Serializable]
public class GeneDataList2 {
    public List<GeneData2> geneDatas;

    public GeneDataList2(List<GeneData2> geneDatas) {
        this.geneDatas = geneDatas;
    }
}

public class MovementData {
    public List<float> angles;
    public List<float> bodySizes;
    public List<float> legSizes;
    //これより下は余計
    public List<float> springs;
    public List<float> dampers;
    public int name;
    public int generation;
    public float distance;
    public float reward;
}

public class MovementDataList {
    public List<MovementData> geneDatas;

    public MovementDataList(List<MovementData> movementDatas) {
        this.geneDatas = movementDatas;
    }
}