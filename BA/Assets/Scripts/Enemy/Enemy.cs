using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ba
{
    /// <summary>
    /// 所有敌人都用这个类，TODO：之后可以用SO决定敌人的行动方式，不同的ai逻辑就装载不同的SO，改成外边统一调用Move方法即可
    /// </summary>
    public class Enemy : MonoBehaviour, IDamagable
    {

        private GameObject target;
        public Rigidbody rb;
        public ChaState chaState;
        public virtual void TakeDamage(DamageData damageData)
        {
            float damage = damageData.BaseDamage;
            if(Random.Range(0,1)<damageData.CriticalChange)//暴击了
            {
                damage *= (1 + damageData.CriticalMultiplier);
            }
            chaState.resource.hp = Mathf.Max(0, chaState.resource.hp - (int)damage);
            GetComponent<HealthBarUI>().UpdateHealthBar(chaState.resource.hp, chaState.property.hp);
        }
        
        /// <summary>
        /// 暂时先这么写，就是简单地向着距离最近的玩家移动,
        /// </summary>
        public virtual void Move()
        {
            
            GetClosestPlayer();

            if (target != null) // 确保玩家存在
            {
                //Debug.Log("11");
                Vector3 direction = (target.transform.position - transform.position).normalized; // 获取朝向玩家的单位向量
                //transform.Translate(direction *chaState.property. moveSpeed * Time.deltaTime); // 沿着朝向玩家的方向移动
                rb.velocity = direction * chaState.property.moveSpeed;

                
                direction.y = 0f; // 将 y 轴置为 0，保持在水平面上

                // 将人物的方向设置为计算得到的方向
                transform.rotation = Quaternion.LookRotation(direction);
            }


        }
        // 获取与自己距离最近的玩家
        public GameObject GetClosestPlayer()
        {
            List<GameObject> characters = CharacterManager.Instance.characters;
            GameObject closestPlayer = null;
            float closestDistance = Mathf.Infinity;

            foreach (var character in characters)
            {
                if (character.CompareTag("Enemy")) continue;
                float distanceToPlayer = Vector3.Distance(transform.position, character.transform.position);
                if (distanceToPlayer < closestDistance)
                {
                    closestDistance = distanceToPlayer;
                    closestPlayer = character;
                }
                /*if (distanceToPlayer < chaState.property.attackRadius)
                {
                    character.GetComponent<GamePlayerController>().TakeDamage(chaState.damageData);
                }*/
            }
            target = closestPlayer;

            return closestPlayer;
        }
        

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Character"))
            {
                collision.gameObject.GetComponent<GamePlayerController>().TakeDamage(chaState.damageData);
            }

        }

    } 

}
