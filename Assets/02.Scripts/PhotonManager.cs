using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

// MonoBehaviour대신 MonoBehaviourPunCallbacks를 상속받아 사용
public class PhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "v1.0";
    private string UserId = "ROK";

    void Awake()
    {
        // 게임 버전 지정
        PhotonNetwork.GameVersion = gameVersion;
        // 유저명 지정
        PhotonNetwork.NickName = UserId;

        // 서버 접속
        PhotonNetwork.ConnectUsingSettings();
    }

    // Photon에 접속했을 시 Photon에서 호출해주는 Call Back Function
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Server!!");
        // 현재 존재하는 Network의 무작위 방에 Join
        PhotonNetwork.JoinRandomRoom();

        // base.OnConnectedToMaster();
    }

    // Random Room에 Join Fail했을 시 호출 Call Back Function
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"code = {returnCode}, msg = {message}");

        // Room을 생성
        PhotonNetwork.CreateRoom($"Room_No_{PhotonNetwork.CountOfRooms + 1}");

        // Room option setting
        RoomOptions ro = new RoomOptions();
        // Room 생성 시 Open 상태로 변경(Photon Server 상에서)
        ro.IsOpen = true;
        // Room 목록에서 확인 가능하게 변경(Photon Server 상에서)
        ro.IsVisible = true;
        // Room의 최대 플레이어 설정
        ro.MaxPlayers = 30;

        // base.OnJoinRandomFailed(returnCode, message);
    }

    // Room 생성 시 호출 Call Back Function
    public override void OnCreatedRoom()
    {
        // base.OnCreatedRoom();
    }

    // Room에 Join 했을 시 호출 Call Back Function
    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 완료");
        Debug.Log(PhotonNetwork.CurrentRoom.Name);

        // 통신이 가능한 주인공 캐릭터(탱크) 생성
        PhotonNetwork.Instantiate("Tank", 
                                  new Vector3(0, 20.0f, 0),
                                  Quaternion.identity);

        // base.OnJoinedRoom();
    }

}
