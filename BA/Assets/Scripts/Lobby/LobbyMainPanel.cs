using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;

public class LobbyMainPanel : MonoBehaviourPunCallbacks//继承这个类可以重写接口
{
    [Header("登录面板")]
    public GameObject LoginPanel;//登录面板

    public InputField PlayerNameInput;//玩家名字输入面板

    [Header("选择操作面板")]
    public GameObject SelectionPanel;//选择操作面板

    [Header("创建房间面板")]
    public GameObject CreateRoomPanel;//创建房间面板

    public InputField RoomNameInputField;//房间名字输入框
    public InputField MaxPlayersInputField;//最大玩家输入框

    [Header("加入随机房间输入框")]
    public GameObject JoinRandomRoomPanel;//加入随机房间输入框

    [Header("房间列表面板")]
    public GameObject RoomListPanel;//房间列表面板

    public GameObject RoomListContent;//房间列表父物体
    public GameObject RoomListEntryPrefab;//单个房间显示预制体

    [Header("房间内部面板")]
    public GameObject InsideRoomPanel;//房间内部面板
    public Button StartGameButton;//开始游戏按钮
    public GameObject PlayerListEntryPrefab;//玩家概述预制体
    public GameObject PlayerListParent;

    private Dictionary<string, RoomInfo> _cachedRoomList;//RoomInfo 是Photon自带的房间信息类
    private Dictionary<string, GameObject> _roomListEntries;
    private Dictionary<int, GameObject> _playerListEntries;



    #region UNITY
    private void Awake()
    {
        //设置所有客户端是否应该加载和房主一样的场景
        PhotonNetwork.AutomaticallySyncScene = true;
        //初始化缓存房间和房间信息列表字典
        _cachedRoomList = new Dictionary<string, RoomInfo>();
        _roomListEntries = new Dictionary<string, GameObject>();
        //给玩家初始化一个名字
        PlayerNameInput.text = "玩家 " + Random.Range(1000, 10000);
    }


    #endregion


    #region PUN CALLBACKS

    //连接到服务器调用
    public override void OnConnectedToMaster()
    {
        Debug.Log(1);
        SetActivePanel(SelectionPanel.name);
    }

    //房间列表更新调用
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();
        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    //加入大厅调用
    public override void OnJoinedLobby()
    {
        //情况缓存的房间重新找一遍
        _cachedRoomList.Clear();
        ClearRoomListView();
    }


    // note: 当客户端加入/创建房间时，即使客户端之前在大厅中，也不会调用 OnLeftLobby

    //离开大厅调用
    public override void OnLeftLobby()
    {
        _cachedRoomList.Clear();
        ClearRoomListView();
    }

    //创建房间失败调用
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetActivePanel(SelectionPanel.name);
        Debug.Log("创建房间失败");
    }


    //加入房间失败
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        SetActivePanel(SelectionPanel.name);
    }

    //加入随机房间失败
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //自己创建一个房间
        string roomName = "房间 " + Random.Range(1000, 10000);
        RoomOptions options = new RoomOptions { MaxPlayers = 4 };
        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    //自己加入房间
    public override void OnJoinedRoom()
    {
        // joining (or entering) a room invalidates any cached lobby room list (even if LeaveLobby was not called due to just joining a room)
        _cachedRoomList.Clear();

        //激活房间内信息面板（包含玩家的名字 id 准备状态啥的）
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
            //初始化玩家在房间中的信息块
            entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, p.NickName);//ActorNumber是有挂载PlayerNumbering才有实际意义

            object isPlayerReady;
            //设置所有玩家的准备状态（在本地上显示）
            if (p.CustomProperties.TryGetValue(BaGameSetting.PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
            }

            _playerListEntries.Add(p.ActorNumber, entry);
        }

        StartGameButton.gameObject.SetActive(CheckPlayersReady());//判断是否显示开始游戏的按钮

        //加入房间了就要把自己信息设置好 方便其他客户端取用判断
        Hashtable props = new Hashtable
            {
                {BaGameSetting.PLAYER_LOADED_LEVEL, false}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    //自己离开房间
    public override void OnLeftRoom()
    {
        SetActivePanel(SelectionPanel.name);//激活操作界面

        foreach (GameObject entry in _playerListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        _playerListEntries.Clear();
        _playerListEntries = null;
    }


    //其他玩家进入房间
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject entry = Instantiate(PlayerListEntryPrefab);
        entry.transform.SetParent(PlayerListParent.transform);
        entry.transform.localScale = Vector3.one;
        entry.GetComponent<PlayerListEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);
        //添加到玩家列表
        _playerListEntries.Add(newPlayer.ActorNumber, entry);
        //判断是否可以开始游戏（其实刚进来一般也不能开 因为没准备）
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }


    //其他玩家离开房间
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //销毁这个玩家的信息显示
        Destroy(_playerListEntries[otherPlayer.ActorNumber].gameObject);
        //从列表中移除这个玩家
        _playerListEntries.Remove(otherPlayer.ActorNumber);
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    //主客户端更换
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        //如果自己成为了主客户端
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }
    }

    //其他玩家属性更换 如准备状态的更改 选择角色的更改
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (_playerListEntries == null)
        {
            _playerListEntries = new Dictionary<int, GameObject>();
        }

        GameObject entry;
        if (_playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))//out 是真好用
        {
            object isPlayerReady;
            if (changedProps.TryGetValue(BaGameSetting.PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);//重新设置那个玩家的准备状态（在本地显示上）
            }
            object character;
            if (changedProps.TryGetValue(BaGameSetting.PLAYER_CHARACTER, out character))
            {
                entry.GetComponent<PlayerListEntry>().SetCharacterImage((string)character);//重新设置那个玩家的角色图片（在本地显示上）
            }
        }

        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    #endregion


    #region UI CALLBACKS

    public void OnBackButtonClicked()//离开大厅按钮
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        SetActivePanel(SelectionPanel.name);
    }

    public void OnCreateRoomButtonClicked()//创建房间按钮
    {
        string roomName = RoomNameInputField.text;
        //防止房间名字为空
        roomName = (roomName.Equals(string.Empty)) ? "房间 " + Random.Range(1000, 10000) : roomName;


        byte maxPlayers;
        byte.TryParse(MaxPlayersInputField.text, out maxPlayers);
        maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 4);

        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers, PlayerTtl = 10000 };//PlayerTtl玩家离开房间到移除的间隔 意义不大

        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    public void OnJoinRandomRoomButtonClicked()//加入随机房间按钮
    {
        SetActivePanel(JoinRandomRoomPanel.name);

        PhotonNetwork.JoinRandomRoom();
    }

    public void OnLeaveGameButtonClicked()//离开房间按钮
    {
        PhotonNetwork.LeaveRoom();
    }
    public void OnReturnSelectPanel()//返回选择面板
    {
        SetActivePanel(SelectionPanel.name);
    }
    public void OnLoginButtonClicked()//登录按钮
    {
        string playerName = PlayerNameInput.text;

        if (!playerName.Equals(""))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();//使用你的PUN配置
        }
        else
        {
            Debug.LogError("玩家名字无效！");
        }
    }

    public void OnRoomListButtonClicked()//展示房间列表按钮
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        SetActivePanel(RoomListPanel.name);
    }

    public void OnStartGameButtonClicked()//开始游戏按钮（主客户端才能按）
    {
        //开始游戏后房间不能进入也不再显示
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        PhotonNetwork.LoadLevel("Game");
    }

    #endregion

    #region CUSTOMED METHODS


    /// <summary>
    /// 要激活的面板的名字
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

    //本地玩家属性设置（其实在这里就是设置是否显示开始游戏按钮）这个方法只有主客户端才会调用
    public void LocalPlayerPropertiesUpdated()
    {
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    /// <summary>
    /// 清除房间列表显示
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
    /// 更新缓存的房间列表信息
    /// </summary>
    /// <param name="roomList"></param>
    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            // 移除不开发 不显示 即将从列表移除 的那些房间
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (_cachedRoomList.ContainsKey(info.Name))
                {
                    _cachedRoomList.Remove(info.Name);
                }

                continue;//跳过当次循环
            }

            //更新房间信息
            if (_cachedRoomList.ContainsKey(info.Name))
            {
                _cachedRoomList[info.Name] = info;
            }
            //添加新的房价信息到缓存
            else
            {
                _cachedRoomList.Add(info.Name, info);
            }
        }
    }


    /// <summary>
    /// 更新房间列表显示(每次调用前都会清楚一遍当前显示 所以其实就是重新生成一遍）
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
    /// 检查玩家是否都已经准备好了
    /// </summary>
    /// <returns></returns>
    private bool CheckPlayersReady()
    {
        //不是主客户端不需要这个功能
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(BaGameSetting.PLAYER_READY, out isPlayerReady))
            {
                //存在一个玩家没有准备好就不行
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
