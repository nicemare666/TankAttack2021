using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Utility;

// IPunObservable 인터페이스를 사용하면 Custom으로 데이터 송/수신 가능
public class TankCtrl : MonoBehaviour, IPunObservable
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

    // 네트워크를 통해 수신받을 position, rotation
    Vector3 receivePos    = Vector3.zero;
    Quaternion receiveRot = Quaternion.identity;


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
        else
        {
            // 수신받은 데이터를 이용하여 탱크를 이동/회전시킴
            // 데이터가 부족할 수 있기 때문에 Lerp(선형보간), Slerp(원형보간)으로 보간하여 사용
            // 현재 위치와 수신받은 위치를 비교하여 3.0f이상 벌어지게 되면 수신받은 위치로 이동
            if ( (tr.position - receivePos).sqrMagnitude > 3.0f * 3.0f)
            {
                tr.position = receivePos;
            }
            else
            {
                // 현재 위치와 수신받은 위치가 3.0f이상 벌어지지 않으면 선형보간을 이용하여 위치 이동
                tr.position = Vector3.Lerp(tr.position, receivePos, Time.deltaTime * 10.0f);        // position
            }
            tr.rotation = Quaternion.Slerp(tr.rotation, receiveRot, Time.deltaTime * 10.0f);    // rotation
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


    // PhotonView를 이용하여 데이터를 송수신 하기 위한 Call Back Function
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)   // PhotonStream으로 데이터를 보낼 때(자신)
        {
            stream.SendNext(tr.position);   // position 송신
            stream.SendNext(tr.rotation);   // rotation 송신
        }
        else
        {
            receivePos = (Vector3) stream.ReceiveNext();    //position 송신
            receiveRot = (Quaternion) stream.ReceiveNext(); //rosition 송신
        }
    }
}
