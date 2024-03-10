using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//游戏需要的一些设置参数
public class BaGameSetting
{
    public const string PLAYER_READY = "IsPlayerReady";//玩家是否准备
    public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";//玩家是否加载到游戏场景里
    public const string PLAYER_CHARACTER = "Midori";//玩家选择的角色 默认是小绿


    public const int CURRENT_CHARACTER_COUNT = 2;
    public static List<string> CHARACTER_LIST = new List<string>()
        {
            "Midori",
            "Momoi",
        };
}
