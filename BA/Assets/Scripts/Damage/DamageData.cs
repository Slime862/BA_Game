using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ba
{
    //这个每个人物挂一个 可以根据武器/词条/Buff/等级 等属性变化 一方对另一方造成伤害时要把这个数据块传到计算伤害的函数里面
    [CreateAssetMenu(fileName = "DamageData", menuName = "DamageSystem/DamageData", order = 1)]
    public class DamageData : ScriptableObject
    {
        public int BaseDamage;//基础伤害

        public float CriticalChange;//暴击率

        public float CriticalMultiplier;//暴击倍数


       


    }
}
