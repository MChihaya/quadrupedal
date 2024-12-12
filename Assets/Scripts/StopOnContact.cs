using Unity.VisualScripting;
using UnityEngine;

public class StopOnContact : MonoBehaviour
{
    public float timer;
    private float startTime;
    private Rigidbody[] rbs;
    // Start is called before the first frame update
    void Start()
    {
        rbs = GetComponentsInChildren<Rigidbody>();
    }

    public void StartTimer(){
        startTime = Time.time;
    }

    // Update is called once per frame
    void OnCollisionEnter(Collision collision){
        rbs = GetComponentsInChildren<Rigidbody>();
        if (collision.gameObject.CompareTag("Plane"))
        {
            timer = Time.time - startTime;
            GetComponent<JointController2>().gene.reward -= (10.0f  - timer) * 30f;
            foreach (var rb in rbs)
            {   
                rb.velocity = Vector3.zero;         // 移動速度をゼロに
                rb.angularVelocity = Vector3.zero; // 回転速度をゼロに
                rb.isKinematic = true;             // 動きを完全に停止
            }

        }
        if (collision.gameObject.CompareTag("Goal")){
            timer = Time.time - startTime;
            GetComponent<JointController2>().gene.reward += (10.0f - timer) * 30f;
            foreach (var rb in rbs)
            {   
                rb.velocity = Vector3.zero;         // 移動速度をゼロに
                rb.angularVelocity = Vector3.zero; // 回転速度をゼロに
                rb.isKinematic = true;             // 動きを完全に停止
            }
        }
    }
}
