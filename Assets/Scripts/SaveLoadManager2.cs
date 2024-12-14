using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class SaveLoadManager2 : MonoBehaviour {

    private static SaveLoadManager2 instance;

    public static SaveLoadManager2 Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<SaveLoadManager2>();
                if (instance == null) {
                    GameObject obj = new GameObject("SaveLoadManager");
                    instance = obj.AddComponent<SaveLoadManager2>();
                }
            }
            return instance;
        }
    }

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    private String directory = "backwardright";
    public void SaveRobotData(List<GeneData2> geneDataList, int generation) {
        DateTime now = DateTime.Now;
        string formatedNow = now.ToString("yyyyMMddHHmmss");
        string jsonData = JsonUtility.ToJson(new GeneDataList2(geneDataList));
        //string filePath = System.IO.Path.Combine("Assets", "robots_save_data.json");
        string filePathRecord = "SaveData/" + directory + string.Format("/robots_save_data_{0}_{1}.json", generation, formatedNow);
        Debug.Log(filePathRecord);
        //System.IO.File.WriteAllText(filePath, jsonData);
        System.IO.File.WriteAllText(filePathRecord, jsonData);
        Debug.Log("Persistent Data Path: " + Application.persistentDataPath);
    }
        public void SaveBestRobotData(List<GeneData2> geneDataList, int generation) {
        DateTime now = DateTime.Now;
        string formatedNow = now.ToString("yyyyMMddHHmmss");
        string jsonData = JsonUtility.ToJson(new GeneDataList2(geneDataList));
        //string filePath = System.IO.Path.Combine("Assets", "robots_save_data.json");
        string filePathRecord = "SaveData/" + directory + string.Format("/best_robots_save_data_{0}_{1}.json", generation, formatedNow);
        Debug.Log(filePathRecord);
        //System.IO.File.WriteAllText(filePath, jsonData);
        System.IO.File.WriteAllText(filePathRecord, jsonData);
        Debug.Log("Persistent Data Path: " + Application.persistentDataPath);
    }
    public void NewestRecordRobotData(List<GeneData2> geneDataList){
        string jsonData = JsonUtility.ToJson(new GeneDataList2(geneDataList));
        string filePath = "SaveData/" + directory + "/robots_save_data.json";
        Debug.Log(filePath);
        System.IO.File.WriteAllText(filePath, jsonData);
    }

    public GeneDataList2 LoadRobotData() {
        string filePath = "SaveData/" + directory + "/robots_save_data.json";
        if (System.IO.File.Exists(filePath)) {
            string jsonData = System.IO.File.ReadAllText(filePath);
            GeneDataList2 geneDataList = JsonUtility.FromJson<GeneDataList2>(jsonData);
            Debug.Log("Loaded " + geneDataList.geneDatas.Count + " robots from " + filePath);
            return geneDataList;
        } else {
            Debug.Log("Save data file not found in " + filePath);
            return null;
        }
    }
}
