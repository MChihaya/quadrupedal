using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GeneData2 {
    public List<double> robotdna;
    public List<float> legSizes; // 不要
    public List<float> bodySizes; //不要
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
[System.Serializable]
public class MovementData {
    public List<float> angles; // 複数の角度データ
    public List<float> legSizes; // 複数の脚サイズ
    public List<float> bodySizes; // 複数の体サイズ
    public List<float> springs;
    public List<float> dumpers;
    public int name; // 識別用
    public int generation; // 世代
    public float distance; // 距離
    public float reward; // 報酬
}
[System.Serializable]
public class MovementDataList {
    public List<MovementData> geneDatas;

    public MovementDataList(List<MovementData> movementDatas) {
        this.geneDatas = movementDatas;
    }
}