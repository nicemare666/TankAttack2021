using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Photon.Pun;

public class Turret : MonoBehaviour
{
    // private PhotonView pv;

    void Start()
    {
        // pv = GetComponent<PhotonView>();
        // 자신의 오브젝트가 아닐 시 스크립트 비활성화
        // this.enabled = pv.IsMine;
    }

    void Update()
    {
        // float mouseX = Input.GetAxis("Mouse X");
        // transform.Rotate(Vector3.up * Time.deltaTime * 200.0f * mouseX);
        
        // float mouseY = Input.GetAxis("Mouse Y");
        // transform.Rotate(Vector3.right * Time.deltaTime * 200.0f * mouseY * -1.0f);
    }
}
