using Ba;
using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Pool;
/// <summary>
/// 统一管理所有敌人/角色的方法
/// </summary>
public class CharacterManager : SingletonPun<CharacterManager>/*,IPunObservable*/
{
    public List<GameObject> characters = new List<GameObject>();
    
    public List<GameObject> enemies = new();
    [Header("用于敌人生成的对象池")]
    public GameObject defaultEnemyBase;
    public ObjectPool<Enemy> pool;
    protected override void Awake()
    {
        base.Awake();
        pool = new(CreatFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, true, 20, 100);

    }
    Enemy CreatFunc()
    {
        GameObject enemyBase = Instantiate(defaultEnemyBase, Vector3.zero, Quaternion.identity);
        Enemy enemy = enemyBase.GetComponent<Enemy>();



        return enemy;
    }

    void ActionOnGet(Enemy obj)
    {
        obj.gameObject.SetActive(true);
    }
    void ActionOnRelease(Enemy obj)
    {
        obj.gameObject.SetActive(false);
    }
    void ActionOnDestroy(Enemy obj)
    {
        Destroy(obj.gameObject);
    }
    private void FixedUpdate()
    {

       characters = GameObject.FindGameObjectsWithTag("Character").ToList();

       enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();

       characters.AddRange(enemies);


       for(int i = 0; i < enemies.Count; i++)
        {
            
            enemies[i].GetComponent<Enemy>().Move();
        }




    }





  

}
