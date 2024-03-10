using Ba;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ChaState : MonoBehaviour
{

    public ChaProperty property = new ChaProperty();
    ///<summary>
    ///角色现有的资源，比如hp之类的
    ///</summary>

    public ChaResource resource = new ChaResource(1);

    public BulletLauncher bulletLauncher;

    public DamageData damageData;
    ///<summary>
    ///角色是否已经死了，这不由我这个系统判断，其他系统应该告诉我
    ///</summary>
    public bool dead = false;
    /// <summary>
    /// 角色现在能否移动
    /// </summary>
    public bool canMove = true;
    ///<summary>
    ///角色的无敌状态持续时间，如果在无敌状态中，子弹不会碰撞，DamageInfo处理无效化
    ///单位：秒
    ///</summary>
    public float immuneTime
    {
        get
        {
            return _immuneTime;
        }
        set
        {
            _immuneTime = Mathf.Max(_immuneTime, value);
        }
    }
    private float _immuneTime = 0.00f;

   





}
