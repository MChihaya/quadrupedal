using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopOnContact : MonoBehaviour
{
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void OnCollisionEnter(Collision collision){
        if (collision.gameObject.CompareTag("Plane"))
        {
            // Rigidbodyの動きを止める
            rb.velocity = Vector3.zero;   // 移動速度をゼロに
            rb.angularVelocity = Vector3.zero; // 回転速度をゼロに
            rb.isKinematic = true;       // オブジェクトを静止状態にする

        }
    }
}
