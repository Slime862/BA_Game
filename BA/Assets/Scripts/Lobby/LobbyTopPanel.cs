using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������� ��Ҫ������ʾ����״̬
/// </summary>
public class LobbyTopPanel : MonoBehaviour
{
    private readonly string connectionStatusMessage = "����״̬: ";

    [Header("UI")]
    public Text ConnectionStatusText;
    public Text GameTitleText;

    #region UNITY

    private void Start()
    {
        GameTitleText.text = "ε������ǹս��˵";//��ʱ���ı���ʾ
    }
    public void Update()
    {
        //��������״̬�ı�
        ConnectionStatusText.text = connectionStatusMessage + PhotonNetwork.NetworkClientState;
    }

    #endregion


}
