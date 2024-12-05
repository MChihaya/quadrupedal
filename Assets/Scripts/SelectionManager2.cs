using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager2 : MonoBehaviour {
    public GameObject robotPrefab;
    public Slider populationSizeSlider;
    public Slider survivalRateSlider;
    public Slider timeScaleSlider;
    public GameObject goal;
    public int populationSize = 50;
    public float generationTime = 60.0f;
    public float survivalRate = 0.3f; // 新しい変数: 生存率（上位何%を保持するか）
    private List<GameObject> robots;
    private float generationTimer = 0.0f;
    private int robotVersion = 0;
    private int generation = 0;

    void Start() {
        //セーブデータがあればロードなければ初期化
        Load();
        if (robots == null) {
            robots = new List<GameObject>();
            for (int i = 0; i < populationSize; i++) {
                GameObject robot = Instantiate(robotPrefab, new Vector3(0, 3, 0), Quaternion.Euler(0, 0, 60));
                robots.Add(robot);
                robot.name = "" + robotVersion;
                robot.GetComponent<DisplayName>().SetName();
                robotVersion++;
            }
        }
        // サイズ遺伝子の適応
        // ApplyGene();
        foreach(var robot in robots){
            robot.GetComponent<StopOnContact>().StartTimer();
        }
        populationSizeSlider.value = robots.Count;
        populationSize = (int)populationSizeSlider.value;
        survivalRate = survivalRateSlider.value;
        Time.timeScale = timeScaleSlider.value;
        
    }

    void FixedUpdate() {
        generationTimer += Time.fixedDeltaTime;
        if (generationTimer >= generationTime || IsAllRobotStop()) {
            // ロボットのサイズを遺伝子に適用
            ApplyGene();
            SortRobotByReward();
            Save();
            // 適応度によって選別
            SelectAndReproduce();
            ResetRobots();
            Load(); // Load the robots again to update the gene values
            ChangePopulationSize();
            generationTimer = 0.0f; // Reset timer after reproduction
            generation++;
        }
    }
    bool IsAllRobotStop(){
        bool isAllRobotStopBool = true;
        foreach(var robot in robots){
            isAllRobotStopBool = robot.GetComponent<Rigidbody>().isKinematic && isAllRobotStopBool;
        }
        return isAllRobotStopBool;
    }

    void SortRobotByReward(){
        // 報酬値でソート
        robots.Sort((a, b) => b.GetComponent<JointController2>().gene.reward.CompareTo(a.GetComponent<JointController2>().gene.reward));
    }
    void SelectAndReproduce() {
        

        // Display the best gene and distance
        DisplayBestGeneAndDistance();

        // Calculate the number of robots that will survive and be replaced
        int survivalCount = (int)(robots.Count * survivalRate);
        int replacementCount = robots.Count - survivalCount;

        for (int i = 0; i < replacementCount; i++) {
            // Crossover between two selected parents from the surviving robots
            var parent1 = robots[UnityEngine.Random.Range(0, survivalCount)].GetComponent<JointController2>().gene;
            var parent2 = robots[UnityEngine.Random.Range(0, survivalCount)].GetComponent<JointController2>().gene;
            var childGene = Crossover(parent1, parent2);

            // Apply mutation with a certain probability
            Mutate(childGene);

            // Replace the genes of the robots to be replaced with the new child genes
            robots[i + survivalCount].GetComponent<JointController2>().gene = childGene;

            robots[i + survivalCount].transform.localScale = new Vector3(
                childGene.bodySizes[0],
                childGene.bodySizes[1],
                childGene.bodySizes[2]
            );

            // 名前を変更
            robots[i + survivalCount].name = "" + robotVersion;
            robots[i + survivalCount].GetComponent<DisplayName>().SetName();
            robotVersion++;
        }
    }

    // Display the best gene and distance of the current generation
    void DisplayBestGeneAndDistance() {
        var bestGene = robots[0].GetComponent<JointController2>().gene;
        string geneString = "";
        foreach (var angle in bestGene.angles) {
            geneString += angle.ToString() + ", ";
        }
        foreach (var legSize in bestGene.legSizes) {
            geneString += legSize.ToString() + ", ";
        }
        foreach (var bodySize in bestGene.bodySizes) {
            geneString += bodySize.ToString() + ", ";
        }
        Debug.Log("Generation: " + generation + ",Best distance: " + robots[0].GetComponent<JointController2>().gene.reward);
    }

    // Crossover function to mix genes of two parents
    Gene2 Crossover(Gene2 parent1, Gene2 parent2) {
        Gene2 child = new Gene2(parent1.angles.Count, parent1.legSizes.Count, parent1.springs.Count, parent1.dumpers.Count);
        
        // Decide the crossover point for angles
        int crossoverPointAngles = UnityEngine.Random.Range(0, parent1.angles.Count);
        for (int i = 0; i < parent1.angles.Count; i++) {
            child.angles[i] = i < crossoverPointAngles ? parent1.angles[i] : parent2.angles[i];
        }

        for (int i = 0; i < parent1.springs.Count; i++) {
            child.springs[i] = i < crossoverPointAngles ? parent1.springs[i] : parent2.springs[i];
        }

        for (int i = 0; i < parent1.dumpers.Count; i++) {
            child.dumpers[i] = i < crossoverPointAngles ? parent1.dumpers[i] : parent2.dumpers[i];
        }
        // Decide the crossover point for legSizes
        int crossoverPointLegSizes = UnityEngine.Random.Range(0, parent1.legSizes.Count);
        for (int i = 0; i < parent1.legSizes.Count; i++) {
            child.legSizes[i] = i < crossoverPointLegSizes ? parent1.legSizes[i] : parent2.legSizes[i];
        }

        // Decide the crossover point for bodySizes
        int crossoverPointBodySizes = UnityEngine.Random.Range(0, parent1.bodySizes.Count);
        for (int i = 0; i < parent1.bodySizes.Count; i++) {
            child.bodySizes[i] = i < crossoverPointBodySizes ? parent1.bodySizes[i] : parent2.bodySizes[i];
        }

        // child.bodysizesの積を8.0にする
        float volume = child.bodySizes[0] * child.bodySizes[1] * child.bodySizes[2];
        float ratio = Mathf.Pow(8.0f / volume, 1.0f / 3.0f);
        child.bodySizes[0] *= ratio;
        child.bodySizes[1] *= ratio;
        child.bodySizes[2] *= ratio;

        return child;
    }


    // Mutate function to introduce random changes
    void Mutate(Gene2 gene) {
        // Mutation logic for angles
        for (int i = 0; i < gene.angles.Count; i++) {
            if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.1f) {
                gene.angles[i] = UnityEngine.Random.Range(-60.0f, 60.0f);
            }
        }
        
        for (int i = 0; i < gene.springs.Count; i++) {
            if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.1f) {
                gene.springs[i] = UnityEngine.Random.Range(0f, 100f);
            }
        }
        
        for (int i = 0; i < gene.dumpers.Count; i++) {
            if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.1f) {
                gene.dumpers[i] = UnityEngine.Random.Range(0f, 10f);
            }
        }
        // Mutation logic for legSizes
        for (int i = 0; i < gene.legSizes.Count; i++) {
            if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.1f) {
                gene.legSizes[i] = UnityEngine.Random.Range(0.1f, 0.5f);
            }
        }
        
        // Ensure the volume of the body remains constant after mutation
        if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.1f) {
            float volume = gene.bodySizes[0] * gene.bodySizes[1] * gene.bodySizes[2];
            gene.bodySizes[0] = UnityEngine.Random.Range(1.5f, 3.0f);
            gene.bodySizes[1] = UnityEngine.Random.Range(1.5f, 3.0f);
            gene.bodySizes[2] = volume / (gene.bodySizes[0] * gene.bodySizes[1]);
        }
    }

    // Change the size of the robot
    void ChangeRobotSize() {
        // 足のサイズをそれぞれ遺伝子から設定
        foreach (var robot in robots) {
            for (int i = 0; i < robot.GetComponent<JointController2>().legParts.Count; i = i + 2) {
                var legPartR = robot.GetComponent<JointController2>().legParts[i];
                var legPartL = robot.GetComponent<JointController2>().legParts[i+1];
                var legSizeX = robot.GetComponent<JointController2>().gene.legSizes[3*i];
                var legSizeY = robot.GetComponent<JointController2>().gene.legSizes[3*i+1];
                var legSizeZ = robot.GetComponent<JointController2>().gene.legSizes[3*i+2];

                // legSizeを適用
                legPartR.transform.localScale = new Vector3(
                    legSizeX,
                    legSizeY,
                    legSizeZ
                );

                legPartL.transform.localScale = new Vector3(
                    legSizeX,
                    legSizeY,
                    legSizeZ
                );        
            }
        }

        // 体のサイズをそれぞれ遺伝子から設定
        foreach (var robot in robots) {
            var body = robot.GetComponent<JointController2>().body;
            var bodySizeX = robot.GetComponent<JointController2>().gene.bodySizes[0];
            var bodySizeY = robot.GetComponent<JointController2>().gene.bodySizes[1];
            var bodySizeZ = robot.GetComponent<JointController2>().gene.bodySizes[2];

            // bodySizeを適用
            body.transform.localScale = new Vector3(
                bodySizeX,
                bodySizeY,
                bodySizeZ
            );
        }
    }

    void ChangePopulationSize() {
        // populationSize が0以下の場合は何もしない
        if (populationSize <= 0) {
            return;
        }

        int currentPopulation = robots.Count;

        // ロボットの数が目標より少ない場合、追加する
        while (currentPopulation < populationSize) {
            AddRobot();
            currentPopulation++;
        }

        // ロボットの数が目標より多い場合、削除する
        while (currentPopulation > populationSize) {
            RemoveRobot(currentPopulation - 1);
            currentPopulation--;
        }
    }

    void AddRobot() {
        GameObject robot = Instantiate(robotPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 60));
        robots.Add(robot);
        robot.name = "" + robotVersion;
        robot.GetComponent<DisplayName>().SetName();
        robotVersion++;
    }

    void RemoveRobot(int index) {
        if (index >= 0 && index < robots.Count) {
            GameObject robotToRemove = robots[index];
            robots.RemoveAt(index);
            Destroy(robotToRemove);
        }
    }


    void ResetRobots() {
        // シーン上のロボットをすべて削除
        foreach (var robot in robots) {
            Destroy(robot);
        }
    }

    // ロボットのサイズを遺伝子に適用する。報酬値をリセットする
    public void ApplyGene() {
        for (int i = 0; i < robots.Count; i++) {
            // 胴体のサイズを遺伝子に適用
            robots[i].GetComponent<JointController2>().gene.bodySizes[0] = robots[i].transform.localScale.x;
            robots[i].GetComponent<JointController2>().gene.bodySizes[1] = robots[i].transform.localScale.y;
            robots[i].GetComponent<JointController2>().gene.bodySizes[2] = robots[i].transform.localScale.z;

            // 足のサイズをそれぞれ遺伝子に適用
            for (int j = 0; j < robots[i].GetComponent<JointController2>().legParts.Count; j = j + 2) {
                var legPartR = robots[i].GetComponent<JointController2>().legParts[j];
                var legPartL = robots[i].GetComponent<JointController2>().legParts[j+1];
                robots[i].GetComponent<JointController2>().gene.legSizes[3*j] = legPartR.transform.localScale.x;
                robots[i].GetComponent<JointController2>().gene.legSizes[3*j+1] = legPartR.transform.localScale.y;
                robots[i].GetComponent<JointController2>().gene.legSizes[3*j+2] = legPartR.transform.localScale.z;
            }
            // 報酬値=距離の逆数＋(-20)×倒れたか
            float reward = 0.0f;
            reward += 1 / (robots[i].transform.position - goal.transform.position).sqrMagnitude;
            if(robots[i].GetComponent<Rigidbody>().isKinematic){
                reward -= (60.0f - robots[i].GetComponent<StopOnContact>().timer) * 0.1f;
            }
            robots[i].GetComponent<JointController2>().gene.reward = reward;
        }
    }

    public void SetPopulationSize() {
        populationSize = (int)populationSizeSlider.value;
    }

    public void SetSurvivalRate() {
        survivalRate = survivalRateSlider.value;
    }

    public void SetTimeScale() {
        Time.timeScale = timeScaleSlider.value;
    }
    // Saveロジックの例
    public void Save() {
        List<GeneData2> geneDataList = new List<GeneData2>();
        foreach (var robot in robots) {
            Gene2 gene = robot.GetComponent<JointController2>().gene;
            GeneData2 geneData = new GeneData2(gene);
            geneData.angles = gene.angles;
            geneData.springs = gene.springs;
            geneData.dumpers = gene.dumpers;
            geneData.legSizes = gene.legSizes;
            geneData.bodySizes = gene.bodySizes;
            geneData.name = int.Parse(robot.name);
            geneData.distance = (goal.transform.position - robot.transform.position).sqrMagnitude;
            geneData.reward = gene.reward;
            geneData.generation = generation;
            geneDataList.Add(geneData);
        }
        SaveLoadManager2.Instance.SaveRobotData(geneDataList, generation);
    }

    // Loadロジックの例
    public void Load() {
        GeneDataList2 geneDataList = SaveLoadManager2.Instance.LoadRobotData();
        if (geneDataList != null) {
            robots = new List<GameObject>();
            foreach (var geneData in geneDataList.geneDatas) {
                GameObject robot = Instantiate(robotPrefab, new Vector3(0, 3, 0), Quaternion.Euler(0, 0, 60));
                //  public Gene(int numAngles, int numLegSizes)
                robot.GetComponent<JointController2>().gene = new Gene2(geneData.angles.Count, geneData.legSizes.Count, geneData.springs.Count, geneData.dumpers.Count);
                for (int i = 0; i < geneData.angles.Count; i++) {
                    robot.GetComponent<JointController2>().gene.angles[i] = geneData.angles[i];
                    robot.GetComponent<JointController2>().gene.springs[i] = geneData.springs[i];
                    robot.GetComponent<JointController2>().gene.dumpers[i] = geneData.dumpers[i];
                }
                robot.GetComponent<JointController2>().gene.legSizes = geneData.legSizes;
                robot.GetComponent<JointController2>().gene.bodySizes = geneData.bodySizes;
                robot.name = geneData.name.ToString();
                // robotVersionとgeneData.nameの大きい方をrobotVersionにする
                robotVersion = Mathf.Max(robotVersion, geneData.name);
                robots.Add(robot);
                robot.GetComponent<DisplayName>().SetName();
                generation = geneData.generation;
            }

            ChangeRobotSize();
        }
    }

}
