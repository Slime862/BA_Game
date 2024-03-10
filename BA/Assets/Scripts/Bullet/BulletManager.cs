using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Photon.Pun;
using System.Linq;
using Ba;

public class BulletManager : SingletonPun<BulletManager>
{
    //Ĭ�ϵĿ��ӵ�����
    public GameObject defaultBullet;
    public ObjectPool<BulletState> pool;

    public List<GameObject> bullets;//���ϵ�ǰ���ڵ��ӵ�

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
            //����Ǹմ����ģ���ô��Ҫ����մ���������
            if (bs.timeElapsed <= 0 && bs.model.OnCreate != null)
            {
                bs.model.OnCreate.Apply(bs);
            }

            //�����ӵ�������Ч������Ϣ
            if (bs.hitTimer <= 0/* > bs.model.hitTimes*/)
            {
                bs.model.OnRemove?.Apply(bs);
                //***Test����
                pool.Release(bs);

                // bs.gameObject.SetActive(false);
            }





            //�����ӵ����ƶ���Ϣ
            // ����ÿ֡�ƶ��ľ���
            float distance = bs.speed * Time.fixedDeltaTime;

            // ���ݷ���;������λ��
            bs.transform.position += bs.createDirection * distance;

            //�����ӵ�����ײ��Ϣ������ӵ�������ײ���Ż�ִ����ײ�߼�
            
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

                //TODO:������ҹ�����ҵ�Ч�����˺�0������TakeDamage��
                
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

                    Debug.Log("������" + character[j].gameObject.name);
                    //������ 
                    bs.hitTimer -= 1;

                    character[j].GetComponent<IDamagable>().TakeDamage(bullets[i].GetComponent<BulletState>().caster.GetComponent<ChaState>().damageData);
                    if (bs.model.OnHit != null)
                    {
                        bs.model.OnHit.Apply(bs);
                    }

                    //��ǽ���ٵĴ��룬��ʱ�Ȳ���

                    //else
                    //{
                    //    Destroy(bullet[i]);
                    //    continue;
                    //}
                }
            }
            

            ///�������ڵĽ���
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
