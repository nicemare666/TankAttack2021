using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Utility;

public class TankCtrl : MonoBehaviour
{
    private Transform tr;
    public float speed = 10.0f;
    private PhotonView pv;

    void Start()
    {
        tr = GetComponent<Transform>();
        pv = GetComponent<PhotonView>();    //PhotonView Component 취득

        // 자신의 탱크일 시
        if (pv.IsMine)
        {
            // 카메라 이동 -> 자신의 캐릭터.CamPivot
            Camera.main.GetComponent<SmoothFollow>().target = tr.Find("CamPivot").transform;
            // Rigidbody의 무게중심 변경
            GetComponent<Rigidbody>().centerOfMass = new Vector3(0, -5.0f, 0);
        }
        else
        {
            // 타 유저의 운동역학 true (자신의 오브젝트 외의 운동역학 계산은 하지 않는다.)
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    void Update()
    {
        // 자신(타 플레이어를 제외한)의 캐릭터인지 확인
        if (pv.IsMine)
        {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");

            tr.Translate(Vector3.forward * Time.deltaTime * speed * v);
            tr.Rotate(Vector3.up * Time.deltaTime * 100.0f * h);
        }
    }
}
