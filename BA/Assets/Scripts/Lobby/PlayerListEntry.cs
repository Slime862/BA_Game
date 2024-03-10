using Photon.Pun.UtilityScripts;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class PlayerListEntry : MonoBehaviour
{
    public Text PlayerNameText;
    public Button PlayerReadyButton;
    public Button ChangeCharacterButton;
    public Image PlayerReadyImage;
    public Image PlayerCharacterImage;

    public Image midoriImage;
    public Image momoiImage;
    private int ownerId;
    private bool isPlayerReady;
    private string character = "Midori";
    private int characterIndex = 0;

    #region UNITY

    public void OnEnable()
    {
        PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumberingChanged;
    }

    public void Start()
    {

        //��������Լ���id�Ͳ���ʾ׼����ť ��ֹ������׼��������ֵ�������
        if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
        {
            PlayerReadyButton.gameObject.SetActive(false);
            ChangeCharacterButton.gameObject.SetActive(false);
        }
        else
        {
            PhotonNetwork.LocalPlayer.SetScore(0);//Pun2��װ�õ����÷�����api ֱ�ӵ���
                                                  //��ʼ������ѡ��Ľ�ɫΪС��
            Hashtable props = new Hashtable() { { BaGameSetting.PLAYER_CHARACTER, character } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);//Ϊ������ó�ʼ����(׼��״̬�� ����ȡ������

            //Ϊ���׼����ťע���¼�
            PlayerReadyButton.onClick.AddListener(() =>
            {
                isPlayerReady = !isPlayerReady;
                SetPlayerReady(isPlayerReady);


                Hashtable props = new Hashtable() { { BaGameSetting.PLAYER_READY, isPlayerReady } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);//Ϊ������ó�ʼ����(׼��״̬�� ����ȡ������

                if (PhotonNetwork.IsMasterClient)
                {
                    FindObjectOfType<LobbyMainPanel>().LocalPlayerPropertiesUpdated();
                }
            });

            //Ϊ��Ҹı��ɫ��ťע���¼�
            ChangeCharacterButton.onClick.AddListener(() =>
            {
                characterIndex++;
                if (characterIndex == BaGameSetting.CURRENT_CHARACTER_COUNT)
                {
                    characterIndex = 0;
                }
                character = BaGameSetting.CHARACTER_LIST[characterIndex];
                //��ͼƬ
                PlayerCharacterImage.sprite = Resources.Load<Sprite>("UI/Sprite/" + character);
                Hashtable props = new Hashtable() { { BaGameSetting.PLAYER_CHARACTER, character } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);//Ϊ�������ѡ��Ľ�ɫ

            });


        }
    }

    public void OnDisable()
    {
        PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumberingChanged;
    }

    #endregion

    public void Initialize(int playerId, string playerName)
    {
        ownerId = playerId;
        PlayerNameText.text = playerName;
    }


    //�����˳����߼��뷿��ͻ����PlayerNumbering.OnPlayerNumberingChanged���ί�� ����Ƕ����˸�ί�еķ���
    private void OnPlayerNumberingChanged()
    {
        //foreach (Player p in PhotonNetwork.PlayerList)
        //{
        //    if (p.ActorNumber == ownerId)
        //    {
        //        //���������ɫ
        //        PlayerColorImage.color = AsteroidsGame.GetColor(p.GetPlayerNumber());
        //    }
        //}
    }

    public void SetPlayerReady(bool playerReady)
    {
        PlayerReadyButton.GetComponentInChildren<Text>().text = playerReady ? "��׼��" : "δ׼��";
        PlayerReadyImage.enabled = playerReady;
    }

    public void SetCharacterImage(string character)
    {
        PlayerCharacterImage.sprite = Resources.Load<Sprite>("UI/Sprite/" + character);
    }
}