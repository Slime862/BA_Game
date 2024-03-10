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

        public Text InfoText;//��ʾ�ı�

        public List<Transform> BornTransforms = new List<Transform>();//����λ��



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
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);//����ǰ��Ҵ������� ���Է���ȡ��
        }

        public override void OnDisable()
        {
            base.OnDisable();

            CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
        }




        #endregion




        #region PUN CALLBACK

        //�Լ��Ͽ�����
        public override void OnDisconnected(DisconnectCause cause)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
        }

        //�Լ��뿪����
        public override void OnLeftRoom()
        {
            PhotonNetwork.Disconnect();
        }

        ////����Ϸ���������ͻ��˸���ʱ��������
        //public override void OnMasterClientSwitched(Player newMasterClient)
        //{
        //    //����Լ������ͻ���
        //    if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        //    {
        //        //
        //    }
        //}

        //��������뿪����
        //public override void OnPlayerLeftRoom(Player otherPlayer)
        //{
        //    CheckEndOfGame();
        //}


        //������Ը��ĵ���
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {

            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }


            // �����û�е���ʱ�����ͻ��ˣ�����ͻ��ˣ���ȵ�ÿ���˶����عؿ������ü�ʱ����ʼ
            int startTimestamp;//��ʼʱ���
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
                    //������ʾ�ı���Ϣ
                    InfoText.text = "���ڵȴ�������Ҽ���......";
                }
            }

        }


        #endregion


        #region CUSTOMED METHODS
        //��ʼ��Ϸ
        private void StartGame()
        {
            object character;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(BaGameSetting.PLAYER_CHARACTER, out character))
            {
                //���ɽ�ɫ
                GameObject obj = PhotonNetwork.Instantiate((string)character,
                    BornTransforms[PhotonNetwork.LocalPlayer.GetPlayerNumber()].position, Quaternion.identity, 0);
                //��ȡһЩ����
                FindObjectOfType<CameraController>().target = obj.transform;//
                //FindObjectOfType<CameraController>().offset = FindObjectOfType<CameraController>().target.transform.position - FindObjectOfType<CameraController>().transform.position;
                FindObjectOfType<AnimController>().currentPlayerAnimator = obj.GetComponent<Animator>();//
                //CharacterManager.Instance.characters.Add(obj);
            }
            Debug.Log("Start");

        }



        //�ж���Ϸ�Ƿ����
        private void CheckEndOfGame()
        {

        }

        //�ж���������Ƿ��Ѿ����뵽��Ϸ������
        private bool CheckAllPlayerLoadedLevel()
        {
            foreach (var player in PhotonNetwork.PlayerList)
            {
                object isLoaded;
                //��ȡ��ҵļ�������
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

        private void OnCountdownTimerIsExpired()//����ʱ�������õķ��� ����ί�е���ʽ���е���
        {
            StartGame();
        }

        #endregion

    }
}
