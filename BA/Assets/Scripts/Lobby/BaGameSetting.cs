using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��Ϸ��Ҫ��һЩ���ò���
public class BaGameSetting
{
    public const string PLAYER_READY = "IsPlayerReady";//����Ƿ�׼��
    public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";//����Ƿ���ص���Ϸ������
    public const string PLAYER_CHARACTER = "Midori";//���ѡ��Ľ�ɫ Ĭ����С��


    public const int CURRENT_CHARACTER_COUNT = 2;
    public static List<string> CHARACTER_LIST = new List<string>()
        {
            "Midori",
            "Momoi",
        };
}
