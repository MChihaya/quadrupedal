using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class SaveLoadManager : MonoBehaviour {

    private static SaveLoadManager instance;

    public static SaveLoadManager Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<SaveLoadManager>();
                if (instance == null) {
                    GameObject obj = new GameObject("SaveLoadManager");
                    instance = obj.AddComponent<SaveLoadManager>();
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

    public void SaveRobotData(List<GeneData2> geneDataList, int generation) {
        DateTime now = DateTime.Now;
        string formatedNow = now.ToString("yyyyMMddHHmmss");
        string jsonData = JsonUtility.ToJson(new GeneDataList2(geneDataList));
        string filePath = System.IO.Path.Combine("Assets", "robots_save_data.json");
        string filePathRecord = string.Format("Assets/robots_save_data_{0}_{1}.json", generation, formatedNow);
        Debug.Log(filePathRecord);
        System.IO.File.WriteAllText(filePath, jsonData);
        System.IO.File.WriteAllText(filePathRecord, jsonData);
        Debug.Log("Persistent Data Path: " + Application.persistentDataPath);
    }

    public GeneDataList2 LoadRobotData() {
        string filePath = System.IO.Path.Combine("Assets", "robots_save_data.json");
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
