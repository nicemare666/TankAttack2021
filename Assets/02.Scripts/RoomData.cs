using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomData : MonoBehaviour
{
    private TMP_Text roomInfoText;
    private RoomInfo _roomInfo;

    public RoomInfo RoomInfo
    {
        get
        {
            return _roomInfo;
        }
        set
        {
            _roomInfo = value;
            roomInfoText.text = $"{_roomInfo.Name} ({_roomInfo.PlayerCount}/{_roomInfo.MaxPlayers})";
            // 버튼의 클릭 이벤트에 함수 연결
            // GetComponent<UnityEngine.UI.Button>().onClick.AddListener( () => OnEnterRoom(_roomInfo.Name) );
        }
    }


    void Awake()
    {
        roomInfoText = GetComponentInChildren<TMP_Text>();
    }

    public void OnEnterRoom()
    {
        // Room option setting
        RoomOptions ro = new RoomOptions();
        // Room 생성 시 Open 상태로 변경(Photon Server 상에서)
        ro.IsOpen = true;
        // Room 목록에서 확인 가능하게 변경(Photon Server 상에서)
        ro.IsVisible = true;
        // Room의 최대 플레이어 설정
        ro.MaxPlayers = 30;

        // PhotonNetwork.JoinRoom(_roomInfo.Name);
        PhotonNetwork.JoinOrCreateRoom(_roomInfo.Name, ro, TypedLobby.Default);

    }
    // public void OnEnterRoom(string roomName)
    // {
    //     PhotonNetwork.JoinRoom(roomName);
    // }

}
