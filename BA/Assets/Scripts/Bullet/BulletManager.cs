using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Photon.Pun;
using System.Linq;
using Ba;

public class BulletManager : SingletonPun<BulletManager>
{
    //默认的空子弹物体
    public GameObject defaultBullet;
    public ObjectPool<BulletState> pool;

    public List<GameObject> bullets;//场上当前存在的子弹

    public DamageData zeroDamage;
    protected override void Awake()
    {
        base.Awake();
        pool = new(CreatFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, true, 1000, 10000);

    }
    BulletState CreatFunc()
    {
        GameObject bullet = Instantiate(defaultBullet, Vector3.zero, Quaternion.identity);
        BulletState bulletState = bullet.GetComponent<BulletState>();



        return bulletState;
    }

    void ActionOnGet(BulletState obj)
    {
        obj.gameObject.SetActive(true);





    }
    void ActionOnRelease(BulletState obj)
    {
        obj.gameObject.SetActive(false);
    }
    void ActionOnDestroy(BulletState obj)
    {
        Destroy(obj.gameObject);
    }



    private void FixedUpdate()
    {



        float timePassed = Time.fixedDeltaTime;
        bullets = GameObject.FindGameObjectsWithTag("Bullet").ToList();

        //Debug.Log(bullet.Length);
        if (bullets.Count <= 0 || CharacterManager.Instance.characters.Count <= 0) return;


        for (int i = 0; i < bullets.Count; i++)
        {
            if (bullets[i].activeInHierarchy != true) continue;

            BulletState bs = bullets[i].GetComponent<BulletState>();
            if (!bs) continue;
            //如果是刚创建的，那么就要处理刚创建的事情
            if (bs.timeElapsed <= 0 && bs.model.OnCreate != null)
            {
                bs.model.OnCreate.Apply(bs);
            }

            //处理子弹命中有效次数信息
            if (bs.hitTimer <= 0/* > bs.model.hitTimes*/)
            {
                bs.model.OnRemove?.Apply(bs);
                //***Test销毁
                pool.Release(bs);

                // bs.gameObject.SetActive(false);
            }





            //处理子弹的移动信息
            // 计算每帧移动的距离
            float distance = bs.speed * Time.fixedDeltaTime;

            // 根据方向和距离更新位置
            bs.transform.position += bs.createDirection * distance;

            //处理子弹的碰撞信息，如果子弹可以碰撞，才会执行碰撞逻辑
            
            List<ChaState> character = new List<ChaState>();
            foreach (var c in CharacterManager.Instance.characters)
            {
                character.Add(c.GetComponent<ChaState>());
            }
            float bRadius = bs.model.radius;

            if (bs.caster)
            {
                ChaState bcs = bs.caster.GetComponent<ChaState>();

            }

            for (int j = 0; j < character.Count; j++)
            {

                //TODO:设置玩家攻击玩家的效果（伤害0，但是TakeDamage）
                
                if (/*(bs.isPlayerBullet && character[j].tag == "Character") || */(!bs.isPlayerBullet && character[j].tag == "Enemy")) continue;
                ChaState cs = character[j].GetComponent<ChaState>();
                if (!cs || cs.dead == true || cs.immuneTime > 0) continue;

                //if (
                //    (bs.model.hitAlly == false && bSide == cs.side) ||
                //    (bs.model.hitFoe == false && bSide != cs.side)
                //) continue;

                float cRadius = cs.property.hitRadius;
                Vector3 dis = bullets[i].transform.position - character[j].transform.position;
                
                if (Mathf.Pow(dis.x, 2) + Mathf.Pow(dis.z, 2) <= Mathf.Pow(bRadius + cRadius, 2))
                {

                    Debug.Log("击中了" + character[j].gameObject.name);
                    //命中了 
                    bs.hitTimer -= 1;

                    character[j].GetComponent<IDamagable>().TakeDamage(bullets[i].GetComponent<BulletState>().caster.GetComponent<ChaState>().damageData);
                    if (bs.model.OnHit != null)
                    {
                        bs.model.OnHit.Apply(bs);
                    }

                    //碰墙销毁的代码，暂时先不搞

                    //else
                    //{
                    //    Destroy(bullet[i]);
                    //    continue;
                    //}
                }
            }
            

            ///生命周期的结算
            bs.duration -= timePassed;
            bs.timeElapsed += timePassed;
            if (bs.duration <= 0 /*|| bs.HitObstacle() == true*/)
            {
                if (bs.model.OnRemove != null)
                {
                    bs.model.OnRemove.Apply(bs);
                }
                //Destroy(bullet[i]);
                pool.Release(bs);
                continue;
            }
        }



    }
}
