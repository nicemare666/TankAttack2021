using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Utility;

public class TankCtrl : MonoBehaviour
{
    private Transform tr;
    public float speed = 30.0f;
    private PhotonView pv;

    public Transform firePos;
    public GameObject cannon;
    public Transform turretTr;
    public Transform cannonTr;
    public AudioClip fireSfx;
    private new AudioSource audio;

    public TMPro.TMP_Text userIdText;

    void Start()
    {
        tr = GetComponent<Transform>();
        pv = GetComponent<PhotonView>();    //PhotonView Component 취득
        audio = GetComponent<AudioSource>();

        userIdText.text = pv.Owner.NickName;

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
            float mouseX = Input.GetAxis("Mouse X");
            float mouseScroll = Input.GetAxis("Mouse ScrollWheel");

            tr.Translate(Vector3.forward * Time.deltaTime * speed * v);
            tr.Rotate(Vector3.up * Time.deltaTime * 100.0f * h);

            turretTr.Rotate(Vector3.up * Time.deltaTime * 200.0f * mouseX);
            cannonTr.Rotate(Vector3.right * Time.deltaTime * 500.0f * mouseScroll * -1.0f);

            // Mouse 왼쪽 버튼 클릭 시
            if (Input.GetMouseButtonDown(0))
            {
                // RPC로 호출 포탄 발사 함수 호출
                // pv.RPC("Fire", RpcTarget.All, null);                      // 본인 호출 후 타 유저는 Server경유 후 호출
                pv.RPC("Fire", RpcTarget.AllViaServer, pv.Owner.NickName);   // Server 경유 후 호출(본인도 포함)
            }
        }
    }

    // 포탄 발사 함수
    [PunRPC]    // RPC 호출용 함수로 선언
    void Fire(string shooterName)
    {
        audio?.PlayOneShot(fireSfx);
        // 포탄 생성
        GameObject _cannon = Instantiate(cannon, firePos.position, firePos.rotation);
        _cannon.GetComponent<Cannon>().shooter = shooterName;
    }
}
