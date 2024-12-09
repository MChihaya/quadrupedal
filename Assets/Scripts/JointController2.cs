using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JointController2 : MonoBehaviour {
    public List<HingeJoint> joints;
    public List<GameObject> legParts;
    public GameObject body;
    public Gene2 gene;
    public GameObject goal;
    public int jointPhase = 10;
    public float timePerGene = 0.1f;
    private float timer = 0.0f;

    private void Awake() {
        if (gene.robotBrain.InputSize != joints.Count + 5) {
            gene = new Gene2(joints.Count + 5, 1, 2 * joints.Count, joints.Count, legParts.Count * 3);
        }
        MoveJoint();
    }

    private void FixedUpdate() {
        timer += Time.deltaTime;// timePerGene[s]ごとに足を動かす。
        if (timer >= timePerGene) {
            timer -= timePerGene;
            MoveJoint();
        }
    }
    // 実際に足を動かしているところ
    public void MoveJoint() {
        var observation = new List<double>(); 
        for (int i = 0; i < joints.Count; i++) {
            var joint = joints[i];
            observation.Add(joint.angle);
        }
        Vector3 distance = body.transform.position - goal.transform.position;
        observation.Add(distance.sqrMagnitude);
        observation.Add(Vector3.Angle(new Vector3(0f, 0f, 0f), distance));
        observation.Add(body.transform.rotation.x);
        observation.Add(body.transform.rotation.y);
        observation.Add(body.transform.rotation.z);
        double[] nextAction = gene.robotBrain.GetAction(observation);

        for (int i = 0; i < joints.Count; i++){
            var joint = joints[i];
            var spring = joint.spring;
            spring.targetPosition = (float) nextAction[i];
            spring.spring = 100f;  // 強度を十分に高く設定
            spring.damper = 10f;  // ダンピングを適用して安定化
            joint.spring = spring;
            joint.useSpring = true;  // Springを有効にする
        }
    }
}
