using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JointController2 : MonoBehaviour {
    public List<HingeJoint> joints;
    public List<GameObject> legParts;
    public GameObject body;
    public Gene2 gene;
    public int jointPhase = 10;
    public float timePerGene = 0.1f;

    private int currentGeneIndex = 0;
    private float timer = 0.0f;

    private void Awake() {
        if (gene.angles.Count != joints.Count*jointPhase) {
            gene = new Gene2(joints.Count*jointPhase, legParts.Count * 3, joints.Count*jointPhase, joints.Count*jointPhase);
        }
        ApplyGene();
    }

    private void FixedUpdate() {
        timer += Time.deltaTime;// timePerGene[s]ごとに足を動かす。
        if (timer >= timePerGene) {
            timer -= timePerGene;
            // currentGeneIndexは今何段階目の遺伝子かを表す
            // jointPhase回やってループする。
            currentGeneIndex = (currentGeneIndex + 1) % jointPhase;
            ApplyGene();
        }
    }
    // 実際に足を動かしているところ
    public void ApplyGene() {
        for (int i = 0; i < joints.Count; i++) {
            var joint = joints[i];
            var angle = gene.angles[i + currentGeneIndex * joints.Count];
            var strong = gene.springs[i + currentGeneIndex * joints.Count];
            var damper = gene.dumpers[i + currentGeneIndex * joints.Count];

            var spring = joint.spring;
            spring.targetPosition = angle;
            spring.spring = strong;  // 強度を十分に高く設定
            spring.damper = damper;  // ダンピングを適用して安定化
            joint.spring = spring;
            joint.useSpring = true;  // Springを有効にする
        }
    }
}
