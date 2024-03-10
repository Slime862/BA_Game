using Photon.Chat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 武器模板的脚本，内含武器的数据和发射方法，可以被继承重写
/// </summary>
public class BulletLauncher : ScriptableObject
{
    ///<summary>
    ///要发射的子弹
    ///</summary>
    public BulletData model;

    ///<summary>
    ///要发射子弹的这个人的gameObject，这里就认角色（拥有ChaState的）
    ///当然可以是null发射的，但是写效果逻辑的时候得小心caster是null的情况
    ///</summary>
    public GameObject caster;

    /// <summary>
    /// 发射的方法
    /// </summary>
    //*******************************TODO改成在bulletmanager里的对象池生成

    public float bulletSpeed=3;

    public  float shootInterval = 0.3f;

    //这个只是默认的射击方法！具体武器射击的方式还是应该自己写
    public float lastShootTime;

    public virtual void Shoot(ChaState chaState)
    {
        //***TODO  要加上射击的间隔

        if (Time.time - lastShootTime < shootInterval) return;

        lastShootTime = Time.time;
        // 在玩家当前位置生成子弹??枪口
        Vector3 position = chaState.transform.position + chaState.transform.forward * 0.05f+chaState.transform.up*0.05f;


        BulletState bullet = BulletManager.Instance.pool.Get();
        //把子弹的一些数值初始化
        bullet.gameObject.transform.SetPositionAndRotation(position, Quaternion.identity);

        Debug.Log("Shoot!!!!!!");

        //GameObject bullet = Instantiate(BulletManager.Instance.defaultBullet, position, Quaternion.identity);

        // 将子弹的方向设置为朝向玩家的 Z 方向
        Vector3 direction = chaState.transform.forward;
        bullet.transform.forward = direction;
        //BulletState bulletState = bullet.AddComponent<BulletState>();


        if (chaState.gameObject.tag == "Character")//是玩家角色发射的子弹 就设置子弹不能攻击玩家角色
        {
            bullet.Set(model, direction, chaState.gameObject, bulletSpeed, 5, true);

        }
        else if (chaState.gameObject.tag == "Enemy")//是敌人角色发射的子弹 就设置子弹不能攻击敌人角色
        {
            bullet.Set(model, direction, chaState.gameObject, bulletSpeed, 5, false);

        }


        //BulletManager.Instance.bullet.Add(bulletState);
        //TODO:可以优化一下放manager里
        if (bullet.model.OnCreate != null)
            bullet.model.OnCreate.Apply(bullet);

    }



}
