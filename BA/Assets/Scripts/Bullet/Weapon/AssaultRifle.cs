using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 突击步枪：独特点子，小桃用
/// </summary>
[CreateAssetMenu(fileName = "AssaultRifle",menuName ="BulletSystem/AssaultRifle",order =2)]
public class AssaultRifle : BulletLauncher
{
    

    public override void Shoot(ChaState chaState)
    {
        //***TODO  要加上射击的间隔

        if (Time.time - lastShootTime < shootInterval) return;

        lastShootTime = Time.time;
        // 在玩家当前位置生成子弹??枪口
        Vector3 position = chaState.transform.position + chaState.transform.forward * 0.25f + chaState.transform.up * 0.3f;


        BulletState bullet = BulletManager.Instance.pool.Get();
        //把子弹的一些数值初始化
        bullet.gameObject.transform.SetPositionAndRotation(position, Quaternion.identity);

        Debug.Log("Shoot!!!!!!");

        //GameObject bullet = Instantiate(BulletManager.Instance.defaultBullet, position, Quaternion.identity);

        // 将子弹的方向设置为朝向玩家的 Z 方向
        Vector3 direction = chaState.transform.forward;
        bullet.transform.forward = GetRandomDirectionOffset(direction);
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
    // 获取玩家当前方向并随机让此方向向左或右偏离几度
    public static Vector3 GetRandomDirectionOffset(Vector3 currentDirection)
    {
        

        // 生成一个随机的偏移角度（假设最大偏移角度为3度）
        float randomAngle = Random.Range(-15f, 15f);

        // 将当前方向向左或右偏转随机角度
        Quaternion newRotation = Quaternion.AngleAxis(randomAngle, Vector3.up);
        Vector3 newDirection = newRotation * currentDirection;

        return newDirection;
    }



}
