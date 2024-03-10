using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 存储子弹的固定的信息
/// </summary>
[CreateAssetMenu(fileName ="BulletData",menuName ="BulletSystem/BulletData",order =1)]
public class BulletData : ScriptableObject
{

    public string id;

    ///<summary>
    ///子弹需要用的prefab，默认是Resources/Prefabs/Bullet/下的，所以这个string需要省略前半部分
    ///比如是BlueRocket0，就会创建自Resources/Prefabs/Bullet/BlueRocket0这个prefab
    ///</summary>
    public GameObject prefab;

    ///<summary>
    ///子弹的碰撞半径，单位：米。这个游戏里子弹在逻辑世界都是圆形的，当然是这个游戏设定如此，实际策划的需求未必只能是圆形。
    ///</summary>
    public float radius;

    ///<summary>
    ///子弹可以碰触的次数，每次碰到合理目标-1，到0的时候子弹就结束了。
    ///</summary>
    public int hitTimes;

    ///<summary>
    ///子弹碰触同一个目标的延迟，单位：秒，最小值是Time.fixedDeltaTime（每帧发生一次）
    ///</summary>
    public float sameTargetDelay;

    public BaseBulletModel OnCreate;

    public BaseBulletModel OnHit;

    public BaseBulletModel OnRemove;





}
