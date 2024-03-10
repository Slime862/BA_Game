using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletState : MonoBehaviour
{
    ///<summary>
    ///这是一颗怎样的子弹
    ///</summary>
    public BulletData model;

    ///<summary>
    ///要发射子弹的这个人的gameObject，这里就认角色（拥有ChaState的）
    ///当然可以是null发射的，但是写效果逻辑的时候得小心caster是null的情况
    ///</summary>
    public GameObject caster;

    ///<summary>
    ///子弹的初速度，单位：米/秒，跟Tween结合获得Tween得到当前移动速度
    ///</summary>
    public float speed;

    ///<summary>
    ///子弹的生命周期，单位：秒
    ///</summary>
    public float duration;

    ///<summary>
    ///子弹已经存在了多久了，单位：秒
    ///毕竟duration是可以被重设的，比如经过一个aoe，生命周期减半了
    ///</summary>
    public float timeElapsed = 0;
    /// <summary>
    /// 子弹触碰有效目标的次数
    /// </summary>
    public int hitTimer;           
    /// <summary>
    /// 子弹目前击中的人，如果没有有效击中，就设成null
    /// </summary>
    public GameObject currentTouch=null;
    /// <summary>
    /// 生成子弹时角色的朝向
    /// </summary>
    public Vector3 createDirection;

    /// <summary>
    /// 可以攻击的物体的tag名字
    /// </summary>
    public bool isPlayerBullet;


    public void Set(BulletData model,Vector3 createDirection,GameObject caster= null, float speed = 2f, float duration = 5f,bool isPlayerBullet= true,int hitTimer=1 )
    {
        this.model=model;
        this.caster = caster;
        this.speed = speed;
        this.duration = duration;
        this.createDirection=createDirection;
        this.isPlayerBullet = isPlayerBullet;
        this.hitTimer = hitTimer;


    }
    

}
