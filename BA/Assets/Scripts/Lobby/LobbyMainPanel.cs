using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;

public class LobbyMainPanel : MonoBehaviourPunCallbacks//�̳�����������д�ӿ�
{
    [Header("��¼���")]
    public GameObject LoginPanel;//��¼���

    public InputField PlayerNameInput;//��������������

    [Header("ѡ��������")]
    public GameObject SelectionPanel;//ѡ��������

    [Header("�����������")]
    public GameObject CreateRoomPanel;//�����������

    public InputField RoomNameInputField;//�������������
    public InputField MaxPlayersInputField;//�����������

    [Header("����������������")]
    public GameObject JoinRandomRoomPanel;//����������������

    [Header("�����б����")]
    public GameObject RoomListPanel;//�����б����

    public GameObject RoomListContent;//�����б�����
    public GameObject RoomListEntryPrefab;//����������ʾԤ����

    [Header("�����ڲ����")]
    public GameObject InsideRoomPanel;//�����ڲ����
    public Button StartGameButton;//��ʼ��Ϸ��ť
    public GameObject PlayerListEntryPrefab;//��Ҹ���Ԥ����
    public GameObject PlayerListParent;

    private Dictionary<string, RoomInfo> _cachedRoomList;//RoomInfo ��Photon�Դ��ķ�����Ϣ��
    private Dictionary<string, GameObject> _roomListEntries;
    private Dictionary<int, GameObject> _playerListEntries;



    #region UNITY
    private void Awake()
    {
        //�������пͻ����Ƿ�Ӧ�ü��غͷ���һ���ĳ���
        PhotonNetwork.AutomaticallySyncScene = true;
        //��ʼ�����淿��ͷ�����Ϣ�б��ֵ�
        _cachedRoomList = new Dictionary<string, RoomInfo>();
        _roomListEntries = new Dictionary<string, GameObject>();
        //����ҳ�ʼ��һ������
        PlayerNameInput.text = "��� " + Random.Range(1000, 10000);
    }


    #endregion


    #region PUN CALLBACKS

    //���ӵ�����������
    public override void OnConnectedToMaster()
    {
        Debug.Log(1);
        SetActivePanel(SelectionPanel.name);
    }

    //�����б���µ���
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();
        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    //�����������
    public override void OnJoinedLobby()
    {
        //�������ķ���������һ��
        _cachedRoomList.Clear();
        ClearRoomListView();
    }


    // note: ���ͻ��˼���/��������ʱ����ʹ�ͻ���֮ǰ�ڴ����У�Ҳ������� OnLeftLobby

    //�뿪��������
    public override void OnLeftLobby()
    {
        _cachedRoomList.Clear();
        ClearRoomListView();
    }

    //��������ʧ�ܵ���
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetActivePanel(SelectionPanel.name);
        Debug.Log("��������ʧ��");
    }


    //���뷿��ʧ��
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        SetActivePanel(SelectionPanel.name);
    }

    //�����������ʧ��
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //�Լ�����һ������
        string roomName = "���� " + Random.Range(1000, 10000);
        RoomOptions options = new RoomOptions { MaxPlayers = 4 };
        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    //�Լ����뷿��
    public override void OnJoinedRoom()
    {
        // joining (or entering) a room invalidates any cached lobby room list (even if LeaveLobby was not called due to just joining a room)
        _cachedRoomList.Clear();

        //���������Ϣ��壨������ҵ����� id ׼��״̬ɶ�ģ�
        SetActivePanel(InsideRoomPanel.name);

        if (_playerListEntries == null)
        {
            _playerListEntries = new Dictionary<int, GameObject>();
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject entry = Instantiate(PlayerListEntryPrefab);
            entry.transform.SetParent(PlayerListParent.transform);
            entry.transform.localScale = Vector3.one;
            //��ʼ������ڷ����е���Ϣ��
            entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, p.NickName);//ActorNumber���й���PlayerNumbering����ʵ������

            object isPlayerReady;
            //����������ҵ�׼��״̬���ڱ�������ʾ��
            if (p.CustomProperties.TryGetValue(BaGameSetting.PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
            }

            _playerListEntries.Add(p.ActorNumber, entry);
        }

        StartGameButton.gameObject.SetActive(CheckPlayersReady());//�ж��Ƿ���ʾ��ʼ��Ϸ�İ�ť

        //���뷿���˾�Ҫ���Լ���Ϣ���ú� ���������ͻ���ȡ���ж�
        Hashtable props = new Hashtable
            {
                {BaGameSetting.PLAYER_LOADED_LEVEL, false}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    //�Լ��뿪����
    public override void OnLeftRoom()
    {
        SetActivePanel(SelectionPanel.name);//�����������

        foreach (GameObject entry in _playerListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        _playerListEntries.Clear();
        _playerListEntries = null;
    }


    //������ҽ��뷿��
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject entry = Instantiate(PlayerListEntryPrefab);
        entry.transform.SetParent(PlayerListParent.transform);
        entry.transform.localScale = Vector3.one;
        entry.GetComponent<PlayerListEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);
        //��ӵ�����б�
        _playerListEntries.Add(newPlayer.ActorNumber, entry);
        //�ж��Ƿ���Կ�ʼ��Ϸ����ʵ�ս���һ��Ҳ���ܿ� ��Ϊû׼����
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }


    //��������뿪����
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //���������ҵ���Ϣ��ʾ
        Destroy(_playerListEntries[otherPlayer.ActorNumber].gameObject);
        //���б����Ƴ�������
        _playerListEntries.Remove(otherPlayer.ActorNumber);
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    //���ͻ��˸���
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        //����Լ���Ϊ�����ͻ���
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }
    }

    //����������Ը��� ��׼��״̬�ĸ��� ѡ���ɫ�ĸ���
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (_playerListEntries == null)
        {
            _playerListEntries = new Dictionary<int, GameObject>();
        }

        GameObject entry;
        if (_playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))//out �������
        {
            object isPlayerReady;
            if (changedProps.TryGetValue(BaGameSetting.PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);//���������Ǹ���ҵ�׼��״̬���ڱ�����ʾ�ϣ�
            }
            object character;
            if (changedProps.TryGetValue(BaGameSetting.PLAYER_CHARACTER, out character))
            {
                entry.GetComponent<PlayerListEntry>().SetCharacterImage((string)character);//���������Ǹ���ҵĽ�ɫͼƬ���ڱ�����ʾ�ϣ�
            }
        }

        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    #endregion


    #region UI CALLBACKS

    public void OnBackButtonClicked()//�뿪������ť
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        SetActivePanel(SelectionPanel.name);
    }

    public void OnCreateRoomButtonClicked()//�������䰴ť
    {
        string roomName = RoomNameInputField.text;
        //��ֹ��������Ϊ��
        roomName = (roomName.Equals(string.Empty)) ? "���� " + Random.Range(1000, 10000) : roomName;


        byte maxPlayers;
        byte.TryParse(MaxPlayersInputField.text, out maxPlayers);
        maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 4);

        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers, PlayerTtl = 10000 };//PlayerTtl����뿪���䵽�Ƴ��ļ�� ���岻��

        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    public void OnJoinRandomRoomButtonClicked()//����������䰴ť
    {
        SetActivePanel(JoinRandomRoomPanel.name);

        PhotonNetwork.JoinRandomRoom();
    }

    public void OnLeaveGameButtonClicked()//�뿪���䰴ť
    {
        PhotonNetwork.LeaveRoom();
    }
    public void OnReturnSelectPanel()//����ѡ�����
    {
        SetActivePanel(SelectionPanel.name);
    }
    public void OnLoginButtonClicked()//��¼��ť
    {
        string playerName = PlayerNameInput.text;

        if (!playerName.Equals(""))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();//ʹ�����PUN����
        }
        else
        {
            Debug.LogError("���������Ч��");
        }
    }

    public void OnRoomListButtonClicked()//չʾ�����б�ť
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        SetActivePanel(RoomListPanel.name);
    }

    public void OnStartGameButtonClicked()//��ʼ��Ϸ��ť�����ͻ��˲��ܰ���
    {
        //��ʼ��Ϸ�󷿼䲻�ܽ���Ҳ������ʾ
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        PhotonNetwork.LoadLevel("Game");
    }

    #endregion

    #region CUSTOMED METHODS


    /// <summary>
    /// Ҫ�������������
    /// </summary>
    /// <param name="activePanel"></param>
    public void SetActivePanel(string activePanel)
    {
        LoginPanel.SetActive(activePanel.Equals(LoginPanel.name));
        SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
        CreateRoomPanel.SetActive(activePanel.Equals(CreateRoomPanel.name));
        JoinRandomRoomPanel.SetActive(activePanel.Equals(JoinRandomRoomPanel.name));
        RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name));    // UI should call OnRoomListButtonClicked() to activate this
        InsideRoomPanel.SetActive(activePanel.Equals(InsideRoomPanel.name));
    }

    //��������������ã���ʵ��������������Ƿ���ʾ��ʼ��Ϸ��ť���������ֻ�����ͻ��˲Ż����
    public void LocalPlayerPropertiesUpdated()
    {
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    /// <summary>
    /// ��������б���ʾ
    /// </summary>
    private void ClearRoomListView()
    {
        foreach (GameObject entry in _roomListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        _roomListEntries.Clear();
    }


    /// <summary>
    /// ���»���ķ����б���Ϣ
    /// </summary>
    /// <param name="roomList"></param>
    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            // �Ƴ������� ����ʾ �������б��Ƴ� ����Щ����
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (_cachedRoomList.ContainsKey(info.Name))
                {
                    _cachedRoomList.Remove(info.Name);
                }

                continue;//��������ѭ��
            }

            //���·�����Ϣ
            if (_cachedRoomList.ContainsKey(info.Name))
            {
                _cachedRoomList[info.Name] = info;
            }
            //����µķ�����Ϣ������
            else
            {
                _cachedRoomList.Add(info.Name, info);
            }
        }
    }


    /// <summary>
    /// ���·����б���ʾ(ÿ�ε���ǰ�������һ�鵱ǰ��ʾ ������ʵ������������һ�飩
    /// </summary>
    private void UpdateRoomListView()
    {
        foreach (RoomInfo info in _cachedRoomList.Values)
        {
            GameObject entry = Instantiate(RoomListEntryPrefab);
            entry.transform.SetParent(RoomListContent.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<RoomListEntry>().Initialize(info.Name, (byte)info.PlayerCount, (byte)info.MaxPlayers);

            _roomListEntries.Add(info.Name, entry);
        }
    }


    /// <summary>
    /// �������Ƿ��Ѿ�׼������
    /// </summary>
    /// <returns></returns>
    private bool CheckPlayersReady()
    {
        //�������ͻ��˲���Ҫ�������
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(BaGameSetting.PLAYER_READY, out isPlayerReady))
            {
                //����һ�����û��׼���þͲ���
                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }
    #endregion
}
