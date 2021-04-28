using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

// MonoBehaviour대신 MonoBehaviourPunCallbacks를 상속받아 사용
public class PhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "v1.0";
    private string userId = "ROK";

    public TMP_InputField userIdText;
    public TMP_InputField roomNameText;


    void Awake()
    {
        // true 상태일 때 host가 Scene을 호출하면 Client도 Scene이 호출된다.
        PhotonNetwork.AutomaticallySyncScene = true;

        // 게임 버전 지정
        PhotonNetwork.GameVersion = gameVersion;
        // 유저명 지정
        // PhotonNetwork.NickName = userId;

        // 서버 접속
        PhotonNetwork.ConnectUsingSettings();
    }

    void Start()
    {
        // Local에 저장되어 있는 UserId를 취득하여 UserId InputBox에 삽입
        userId = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(0, 100):00}");
        userIdText.text = userId;
        PhotonNetwork.NickName = userId;
    }

    // Photon에 접속했을 시 Photon에서 호출해주는 Call Back Function
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Server!!");
        // 현재 존재하는 Network의 무작위 방에 접속 시도
        // PhotonNetwork.JoinRandomRoom();

        // Lobby에 입장
        PhotonNetwork.JoinLobby();

        // base.OnConnectedToMaster();
    }

    // Lobby 입장 시 Call Back Function
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined to Lobby");
        // base.OnJoinedLobby();
    }

    // Random Room에 Join Fail했을 시 호출 Call Back Function
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"code = {returnCode}, msg = {message}");

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

        // 유니티 SceneManager를 사용하여 Load하는 방법
        // 통신을 잠시 끊음
        // PhotonNetwork.IsMessageQueueRunning = false;
        // SceneManagerHelper.LoadLevel("BattleField");

        // 본인이 Host Client일 때
        if (PhotonNetwork.IsMasterClient)
        {
            // 전투 Scene을 로드한다.
            PhotonNetwork.LoadLevel("BattleField");
        }

        // base.OnJoinedRoom();
    }

#region UI_BUTTON_CALLBACK
    // Login Button Click Function
    public void OnLoginClick()
    {
        // UserId Input Box Null Check
        if (string.IsNullOrEmpty(userIdText.text))
        {
            // UserId Input Box가 Null이면 UserId를 랜덤 생성하여 삽입
            userId = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(0, 100):00}");
            userIdText.text = userId;
        }
        else 
        {
            userId = userIdText.text;
        }
        // UserId를 Local영역에 저장
        PlayerPrefs.SetString("USER_ID", userId);
        // UserId를 Photon UserId로 설정
        PhotonNetwork.NickName = userId;
        // Random Room에 join
        PhotonNetwork.JoinRandomRoom();
    }

    // Create Room Button Click Function
    public void OnMakeRoomClick()
    {
        // Room option setting
        RoomOptions ro = new RoomOptions();
        // Room 생성 시 Open 상태로 변경(Photon Server 상에서)
        ro.IsOpen = true;
        // Room 목록에서 확인 가능하게 변경(Photon Server 상에서)
        ro.IsVisible = true;
        // Room의 최대 플레이어 설정
        ro.MaxPlayers = 30;

        // Room Name Input Box가 Null일 시 Random Room Name 지정
        if (string.IsNullOrEmpty(roomNameText.text))
        {
            roomNameText.text = $"ROOM_{Random.Range(0, 100):000}";
            
        }
        // Room을 생성
        PhotonNetwork.CreateRoom(roomNameText.text, ro);
    }

    // Room List 수신 (룸 목록이 갱신 될 때마다 호출되는 Call Back Function)
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            Debug.Log($"Room Name : {room.Name}, {room.PlayerCount}/{room.MaxPlayers}");
        }
    }

#endregion

}
