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

        //如果不是自己的id就不显示准备按钮 防止你帮别人准备这种奇怪的现象发生
        if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
        {
            PlayerReadyButton.gameObject.SetActive(false);
            ChangeCharacterButton.gameObject.SetActive(false);
        }
        else
        {
            PhotonNetwork.LocalPlayer.SetScore(0);//Pun2封装好的设置分数的api 直接调用
                                                  //初始化传递选择的角色为小绿
            Hashtable props = new Hashtable() { { BaGameSetting.PLAYER_CHARACTER, character } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);//为玩家设置初始属性(准备状态） 可以取出来用

            //为玩家准备按钮注册事件
            PlayerReadyButton.onClick.AddListener(() =>
            {
                isPlayerReady = !isPlayerReady;
                SetPlayerReady(isPlayerReady);


                Hashtable props = new Hashtable() { { BaGameSetting.PLAYER_READY, isPlayerReady } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);//为玩家设置初始属性(准备状态） 可以取出来用

                if (PhotonNetwork.IsMasterClient)
                {
                    FindObjectOfType<LobbyMainPanel>().LocalPlayerPropertiesUpdated();
                }
            });

            //为玩家改变角色按钮注册事件
            ChangeCharacterButton.onClick.AddListener(() =>
            {
                characterIndex++;
                if (characterIndex == BaGameSetting.CURRENT_CHARACTER_COUNT)
                {
                    characterIndex = 0;
                }
                character = BaGameSetting.CHARACTER_LIST[characterIndex];
                //换图片
                PlayerCharacterImage.sprite = Resources.Load<Sprite>("UI/Sprite/" + character);
                Hashtable props = new Hashtable() { { BaGameSetting.PLAYER_CHARACTER, character } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);//为玩家设置选择的角色

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


    //有人退出或者加入房间就会调用PlayerNumbering.OnPlayerNumberingChanged这个委托 这个是订阅了该委托的方法
    private void OnPlayerNumberingChanged()
    {
        //foreach (Player p in PhotonNetwork.PlayerList)
        //{
        //    if (p.ActorNumber == ownerId)
        //    {
        //        //设置玩家颜色
        //        PlayerColorImage.color = AsteroidsGame.GetColor(p.GetPlayerNumber());
        //    }
        //}
    }

    public void SetPlayerReady(bool playerReady)
    {
        PlayerReadyButton.GetComponentInChildren<Text>().text = playerReady ? "已准备" : "未准备";
        PlayerReadyImage.enabled = playerReady;
    }

    public void SetCharacterImage(string character)
    {
        PlayerCharacterImage.sprite = Resources.Load<Sprite>("UI/Sprite/" + character);
    }
}