using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public float speed = 2000.0f;
    public GameObject expEffect;
    public string shooter;

    void Update()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * speed); 
    }

    void OnCollisionEnter(Collision coll)
    {
        GameObject obj = Instantiate(expEffect,
                                     transform.position,
                                     transform.rotation);
        Destroy(obj, 4.0f);
        Destroy(this.gameObject);
    }

}
