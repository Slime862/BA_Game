using Photon.Pun.UtilityScripts;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using static UnityEngine.GraphicsBuffer;
using System.Runtime.InteropServices;
using System.Linq;

namespace Ba
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        private static GameManager instance = null;

        public static GameManager Instance => instance;

        public Text InfoText;//提示文本

        public List<Transform> BornTransforms = new List<Transform>();//出生位置



        #region UNITY
        public void Awake()
        {
            instance = this;
        }

        public override void OnEnable()
        {
            base.OnEnable();

            CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
        }

        public void Start()
        {
            Hashtable props = new Hashtable
            {
                {BaGameSetting.PLAYER_LOADED_LEVEL, true}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);//给当前玩家传递属性 可以方便取用
        }

        public override void OnDisable()
        {
            base.OnDisable();

            CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
        }




        #endregion




        #region PUN CALLBACK

        //自己断开连接
        public override void OnDisconnected(DisconnectCause cause)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
        }

        //自己离开房间
        public override void OnLeftRoom()
        {
            PhotonNetwork.Disconnect();
        }

        ////当游戏房间内主客户端更换时触发调用
        //public override void OnMasterClientSwitched(Player newMasterClient)
        //{
        //    //如果自己是主客户端
        //    if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        //    {
        //        //
        //    }
        //}

        //其他玩家离开房间
        //public override void OnPlayerLeftRoom(Player otherPlayer)
        //{
        //    CheckEndOfGame();
        //}


        //玩家属性更改调用
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {

            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }


            // 如果还没有倒计时，主客户端（这个客户端）会等到每个人都加载关卡并设置计时器开始
            int startTimestamp;//开始时间戳
            bool startTimeIsSet = CountdownTimer.TryGetStartTime(out startTimestamp);

            if (changedProps.ContainsKey(BaGameSetting.PLAYER_LOADED_LEVEL))
            {
                if (CheckAllPlayerLoadedLevel())
                {
                    if (!startTimeIsSet)
                    {
                        CountdownTimer.SetStartTime();
                    }
                }
                else
                {
                    //设置提示文本信息
                    InfoText.text = "正在等待其他玩家加入......";
                }
            }

        }


        #endregion


        #region CUSTOMED METHODS
        //开始游戏
        private void StartGame()
        {
            object character;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(BaGameSetting.PLAYER_CHARACTER, out character))
            {
                //生成角色
                GameObject obj = PhotonNetwork.Instantiate((string)character,
                    BornTransforms[PhotonNetwork.LocalPlayer.GetPlayerNumber()].position, Quaternion.identity, 0);
                //获取一些参数
                FindObjectOfType<CameraController>().target = obj.transform;//
                //FindObjectOfType<CameraController>().offset = FindObjectOfType<CameraController>().target.transform.position - FindObjectOfType<CameraController>().transform.position;
                FindObjectOfType<AnimController>().currentPlayerAnimator = obj.GetComponent<Animator>();//
                //CharacterManager.Instance.characters.Add(obj);
            }
            Debug.Log("Start");

        }



        //判断游戏是否结束
        private void CheckEndOfGame()
        {

        }

        //判断所有玩家是否已经加入到游戏场景中
        private bool CheckAllPlayerLoadedLevel()
        {
            foreach (var player in PhotonNetwork.PlayerList)
            {
                object isLoaded;
                //获取玩家的加载属性
                if (player.CustomProperties.TryGetValue(BaGameSetting.PLAYER_LOADED_LEVEL, out isLoaded))
                {
                    if ((bool)isLoaded)
                    {
                        continue;
                    }
                }
                return false;
            }
            return true;
        }

        private void OnCountdownTimerIsExpired()//倒计时结束调用的方法 利用委托的形式进行调用
        {
            StartGame();
        }

        #endregion

    }
}
