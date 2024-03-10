using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ba
{
    /// <summary>
    /// ���е��˶�������࣬TODO��֮�������SO�������˵��ж���ʽ����ͬ��ai�߼���װ�ز�ͬ��SO���ĳ����ͳһ����Move��������
    /// </summary>
    public class Enemy : MonoBehaviour, IDamagable
    {

        private GameObject target;
        public Rigidbody rb;
        public ChaState chaState;
        public virtual void TakeDamage(DamageData damageData)
        {
            float damage = damageData.BaseDamage;
            if(Random.Range(0,1)<damageData.CriticalChange)//������
            {
                damage *= (1 + damageData.CriticalMultiplier);
            }
            chaState.resource.hp = Mathf.Max(0, chaState.resource.hp - (int)damage);
            GetComponent<HealthBarUI>().UpdateHealthBar(chaState.resource.hp, chaState.property.hp);
        }
        
        /// <summary>
        /// ��ʱ����ôд�����Ǽ򵥵����ž������������ƶ�,
        /// </summary>
        public virtual void Move()
        {
            
            GetClosestPlayer();

            if (target != null) // ȷ����Ҵ���
            {
                //Debug.Log("11");
                Vector3 direction = (target.transform.position - transform.position).normalized; // ��ȡ������ҵĵ�λ����
                //transform.Translate(direction *chaState.property. moveSpeed * Time.deltaTime); // ���ų�����ҵķ����ƶ�
                rb.velocity = direction * chaState.property.moveSpeed;

                
                direction.y = 0f; // �� y ����Ϊ 0��������ˮƽ����

                // ������ķ�������Ϊ����õ��ķ���
                transform.rotation = Quaternion.LookRotation(direction);
            }


        }
        // ��ȡ���Լ�������������
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
