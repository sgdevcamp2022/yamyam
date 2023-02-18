using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public Text StatusText;
    public InputField roomInput, NickNameInput;

    void Awake() => Screen.SetResolution(960, 540, false); // PC���� ���带 �ϱ� ���� ���� 960, ���� 540���� �ߴ�?

    void Update() => StatusText.text = PhotonNetwork.NetworkClientState.ToString(); //���� �ؽ�Ʈ�� � ���·� ����Ǿ��ִ��� ǥ�� ( ����Ǿ��ִ���, �濡�ִ���, �κ��ִ���)

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        print("�������ӿϷ�");
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
    }

    public void Disconnect() => PhotonNetwork.Disconnect();
    public override void OnDisconnected(DisconnectCause cause) => print("�������");
    public void JoinLobby() => PhotonNetwork.JoinLobby();
    public override void OnJoinedLobby() => print("�κ����ӿϷ�");
    public void CreateRoom() => PhotonNetwork.CreateRoom(roomInput.text, new RoomOptions { MaxPlayers = 4 });
    public void JoinRoom() => PhotonNetwork.JoinRoom(roomInput.text);
    public void JoinOrCreateRoom() => PhotonNetwork.JoinOrCreateRoom(roomInput.text, new RoomOptions { MaxPlayers = 4 }, null);
    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();
    public void LeaveRoom() => PhotonNetwork.LeaveRoom();
    public override void OnCreatedRoom() => print("�游���Ϸ�");
    public override void OnJoinedRoom() => print("�������Ϸ�");
    public override void OnCreateRoomFailed(short returnCode, string message) => print("�游������");
    public override void OnJoinRoomFailed(short returnCode, string message) => print("����������");
    public override void OnJoinRandomFailed(short returnCode, string message) => print("�淣����������");

    [ContextMenu("����")]
    void Info()
    {
        if (PhotonNetwork.InRoom) // �濡 ������ ǥ��
        {
            print("���� �� �̸� : " + PhotonNetwork.CurrentRoom.Name);
            print("���� �� �ο��� : " + PhotonNetwork.CurrentRoom.PlayerCount);
            print("���� �� �ִ��ο��� : " + PhotonNetwork.CurrentRoom.MaxPlayers);

            string playerStr = "�濡 �ִ� �÷��̾� ��� : ";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) playerStr += PhotonNetwork.PlayerList[i].NickName + ", ";
            print(playerStr);
        }
        else // �κ� �����Ϳ� ������ ǥ��
        {
            print("������ �ο� �� : " + PhotonNetwork.CountOfPlayers);
            print("�� ���� : " + PhotonNetwork.CountOfRooms);
            print("��� �濡 �ִ� �ο� �� : " + PhotonNetwork.CountOfPlayersInRooms);
            print("�κ� �ִ���? : " + PhotonNetwork.InLobby);
            print("����ƴ���? : " + PhotonNetwork.IsConnected);
        }
    }
}