using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ba
{
    public class EnemySpawner : SingletonPun<EnemySpawner>
    {


        public void SpawnEnemies(EnemyType enemyType , int amount, Vector3 pos , float radius)
        {
            for(int i =0;i<amount;i++)
            {
                Vector3 spawnPos = new Vector3(pos.x+Random.Range(-1,1)*radius,0, pos.z+Random.Range(-1, 1) * radius);
                GameObject enemy = PhotonNetwork.Instantiate(enemyType.ToString(), spawnPos, Quaternion.identity);
            }
        }
    }
}
