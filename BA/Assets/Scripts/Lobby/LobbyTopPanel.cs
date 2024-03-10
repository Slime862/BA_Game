using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 顶部面板 主要用作显示连接状态
/// </summary>
public class LobbyTopPanel : MonoBehaviour
{
    private readonly string connectionStatusMessage = "连接状态: ";

    [Header("UI")]
    public Text ConnectionStatusText;
    public Text GameTitleText;

    #region UNITY

    private void Start()
    {
        GameTitleText.text = "蔚蓝档案枪战传说";//暂时用文本显示
    }
    public void Update()
    {
        //设置连接状态文本
        ConnectionStatusText.text = connectionStatusMessage + PhotonNetwork.NetworkClientState;
    }

    #endregion


}
