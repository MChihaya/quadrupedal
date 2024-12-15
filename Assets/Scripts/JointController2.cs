using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JointController2 : MonoBehaviour {
    public List<HingeJoint> joints;
    public List<GameObject> legParts;
    public GameObject body;
    public Movement[] movements;
    public Gene2 gene;
    public GameObject goal;
    public int jointPhase = 10;
    public float timePerGene = 0.1f;
    private float timer = 0.0f;
    private int nextAction;
    private int phase = 0;

    private void Awake() {
        if (gene.robotBrain.InputSize != 6) {
            int[] hiddenLayers = {16};
            gene = new Gene2(6, hiddenLayers, 8, legParts.Count * 3);
        }
        MoveJoint();
    }

    private void FixedUpdate() {
         timer += Time.deltaTime;// timePerGene[s]ごとに足を動かす。
         if (timer >= timePerGene) {
            timer -= timePerGene;
            // gene.reward += 2000 / (body.transform.position - goal.transform.position).sqrMagnitude;
            gene.reward += body.transform.position.sqrMagnitude;
            MoveJoint();
        }
    }
    // 実際に足を動かしているところ
    public void MoveJoint() {
        
        if(phase == 0){
            var observation = new List<double>(); 
            for (int i = 0; i < joints.Count; i++) {
                var joint = joints[i];
                observation.Add(joint.angle / 180f);
            }
            Vector3 distance = goal.transform.position - body.transform.position;
            float distanceToGoal = distance.sqrMagnitude;
            observation.Add(distance.x / distanceToGoal);
            observation.Add(distance.z / distanceToGoal);
            observation.Add(body.transform.rotation.x);
            observation.Add(body.transform.rotation.y);
            observation.Add(body.transform.rotation.z);
            observation.Add(body.transform.rotation.w);
            //observation.Add(GetComponent<Rigidbody>().velocity.x / 10f);
            //observation.Add(GetComponent<Rigidbody>().velocity.z / 10f);
            nextAction = gene.robotBrain.GetAction(observation);
            ChangeSizes();
        }

        if(phase == 10){
            for (int i = 0; i < joints.Count; i++){
                var joint = joints[i];
                var spring = joint.spring;
                spring.targetPosition = 0f;
                spring.spring = 100f;  // 強度を十分に高く設定
                spring.damper = 10f;  // ダンピングを適用して安定化
                joint.spring = spring;
                joint.useSpring = true;  // Springを有効にする
            }
        }else{
            for (int i = 0; i < joints.Count; i++){
                var joint = joints[i];
                var spring = joint.spring;
                spring.targetPosition = (float) movements[nextAction].angles[i + phase * joints.Count];
                spring.spring = 100f;  // 強度を十分に高く設定
                spring.damper = 10f;  // ダンピングを適用して安定化
                joint.spring = spring;
                joint.useSpring = true;  // Springを有効にする
            }
        
        }
        phase++;
        if(phase == jointPhase + 1){
            phase = 0;
        }
    }
    public void ChangeSizes(){
        for (int i = 0; i < legParts.Count; i++){
            var legPartR = legParts[i];
            var legPartL = legParts[i + 1];
            var legSizeX = movements[nextAction].legSizes[3*i];
            var legSizeY = movements[nextAction].legSizes[3*i+1];
            var legSizeZ = movements[nextAction].legSizes[3*i+2];
            legPartR.transform.localScale = new Vector3(legSizeX, legSizeY, legSizeZ);
            legPartL.transform.localScale = new Vector3(legSizeX, legSizeY, legSizeZ);
        }
        for (int i = 0; i < 3; i++){
            var bodySizeX = movements[nextAction].bodySizes[3*i];
            var bodySizeY = movements[nextAction].bodySizes[3*i+1];
            var bodySizeZ = movements[nextAction].bodySizes[3*i+2];
            body.transform.localScale = new Vector3(bodySizeX, bodySizeY, bodySizeZ);
        }
    }
}
