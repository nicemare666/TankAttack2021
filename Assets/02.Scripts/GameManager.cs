using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Room Info")]
    public TMP_Text roomNameText;
    public TMP_Text connectInfoText;
    public TMP_Text msgText;
    public Button exitButton;

    [Header("Chatting UI")]
    public TMP_Text chatListText;
    public TMP_InputField msgIF;

    private PhotonView pv;

    // Singleton 변수
    public static GameManager instance = null;

    void Awake()
    {
        instance = this;

        Vector3 pos = new Vector3(Random.Range(-200.0f, 200.0f),
                                  30.0f,
                                  Random.Range(-200.0f, 200.0f));
        // 통신이 가능한 주인공 캐릭터(탱크) 생성
        PhotonNetwork.Instantiate("Tank", 
                                  pos,
                                  Quaternion.identity);
    }

    void Start()
    {
        // pv = GetComponent<PhotonView>();
        pv = photonView;
        SetRoomInfo();
    }

    void SetRoomInfo()
    {
        Room currentRoom = PhotonNetwork.CurrentRoom;
        roomNameText.text = currentRoom.Name;
        connectInfoText.text = $"{currentRoom.PlayerCount}/{currentRoom.MaxPlayers}";
    }

    public void OnExitClick()
    {
        // Game Room에서 Exit 하기 전 PhotonView의 삭제를 진행
        // Clean Up 작업
        PhotonNetwork.LeaveRoom();
    }

    // CleanUp이 완료된 후 호출되는 Call Back Function
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }

    // 타 Player가 Room에 추가되었을 시 Call Back Function
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        SetRoomInfo();

        string msg = $"\r\n<color=#00ff00>{newPlayer.NickName}</color> is Joined Room";
        

        msgText.text += msg;
    }
    // 타 Player가 Room에서 나갔을 시 Call Back Function
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        SetRoomInfo();
        string msg =$"\r\n<color=#ff0000>{otherPlayer.NickName}</color> is Left Room";
        msgText.text += msg;
    }

    public void OnSendClick()
    {
        string _msg = $"<color=#00ff00>[{PhotonNetwork.NickName}]</color>{msgIF.text}";

        pv.RPC("SendChatMessage", RpcTarget.AllBufferedViaServer, _msg);
    }

    [PunRPC]
    void SendChatMessage(string msg)
    {
        chatListText.text += $"\r\n{msg}";
    }
}
