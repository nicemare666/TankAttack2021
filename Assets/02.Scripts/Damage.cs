using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Damage : MonoBehaviour
{
    private List<MeshRenderer> renderers = new List<MeshRenderer>();
    public int hp = 100;
    private PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        // MeshRenderer Component 취득
        GetComponentsInChildren<MeshRenderer>(renderers);
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("CANNON"))
        {

            string shooter = coll.gameObject.GetComponent<Cannon>().shooter;
            hp -= 10;

            if (hp <= 0)
            {
                StartCoroutine("TankDestroy", shooter);
            }
        }
    }

    IEnumerator TankDestroy(string shooter)
    {
        string msg = $"\r\n<color=#00ff00>{pv.Owner.NickName}</color>"
                     + $" is killed by <color=#ff0000>{shooter}</color>";
        GameManager.instance.msgText.text += msg;

        // 발사 로직을 정지
        // 렌더러/콜리전 컴포넌트 비활성화
        foreach (MeshRenderer mesh in renderers) mesh.enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        if (pv.IsMine) GetComponent<Rigidbody>().isKinematic = true;

        // 5초 Waitting
        yield return new WaitForSeconds(5.0f);

        // 생명력 초기화
        hp = 100;

        // Random한 위치로 변경
        Vector3 pos = new Vector3(Random.Range(-200.0f, 200.0f),
                            30.0f,
                            Random.Range(-200.0f, 200.0f));
        transform.position = pos;

        // 렌더러/콜리전 컴포넌트 활성화
        foreach (MeshRenderer mesh in renderers) mesh.enabled = true;
        GetComponent<BoxCollider>().enabled = true;
        if (pv.IsMine) GetComponent<Rigidbody>().isKinematic = false;

    }
}
