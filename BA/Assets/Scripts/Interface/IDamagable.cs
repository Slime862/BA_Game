using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ba
{
    public interface  IDamagable//受伤接口 不仅是敌人和主角 可破坏的物品也可以 这样子弹发出去就只要获取这个接口调用TakeDamage
    {
        public abstract void TakeDamage(DamageData damageData);

    }
}
