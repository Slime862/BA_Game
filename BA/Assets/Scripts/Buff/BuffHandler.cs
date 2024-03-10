using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffHandler : MonoBehaviour
{
    public LinkedList<BuffInfo> buffList = new();
    private List<BuffInfo> buffListForRemove = new();
    private void Update()
    {
        BuffTickAndRemove();
        

    }
    
    public void AddBuff(BuffInfo buffInfo)
    {
        BuffInfo findBuffInfo = FindBuff(buffInfo.buffData.id);
        if (findBuffInfo != null)
        {
            //buff存在
            if (findBuffInfo.curStack < findBuffInfo.buffData.maxStack)
            {
                findBuffInfo.curStack++;
                switch (findBuffInfo.buffData.buffUpdateTime)
                {
                    case BuffUpdateTimeEnum.Add:
                        findBuffInfo.duraTimer += findBuffInfo.buffData.duration;
                        break;
                    case BuffUpdateTimeEnum.Replace:
                        findBuffInfo.duraTimer = findBuffInfo.buffData.duration;
                        break;
                    case BuffUpdateTimeEnum.Keep:
                        break;
                }

                findBuffInfo.buffData.OnCreate.Apply(findBuffInfo);
            }
            else
            {
                buffInfo.curStack = 1;
                buffInfo.duraTimer = buffInfo.buffData.duration;
                buffInfo.tickTimer = buffInfo.buffData.tickTime;
                buffList.AddLast(buffInfo);
                //对list进行排序
                InsertionSort(buffList);
            }
        }


    }
    /// <summary>
    /// 减少buff层数
    /// </summary>
    /// <param name="buffInfo"></param>
    public void RemoveBuff(BuffInfo buffInfo) 
    {
        switch (buffInfo.buffData.buffRemoveStackUpdate)
        {
            case BuffRemoveStackUpdateEnum.Clear:
                buffInfo.buffData.OnRemove.Apply(buffInfo);
                buffList.Remove(buffInfo);
                break;
            case BuffRemoveStackUpdateEnum.Reduce:
                buffInfo.curStack--;
                buffInfo.buffData.OnRemove.Apply(buffInfo);
                if (buffInfo.curStack == 0)
                {
                    
                    buffList.Remove(buffInfo);
                }
                else
                {
                    buffInfo.duraTimer = buffInfo.buffData.duration;
                }

                break;
        }

    }
    public void BuffTickAndRemove()
    {
        foreach(var buffInfo in buffList)
        {
            if (buffInfo.buffData.OnTick != null)
            {
                if (buffInfo.tickTimer < 0)
                {
                    buffInfo.buffData.OnTick.Apply(buffInfo);
                    buffInfo.tickTimer = buffInfo.buffData.tickTime;
                }
                else
                {
                    buffInfo.tickTimer -= Time.deltaTime;
                }


            }
            if (buffInfo.duraTimer < 0)
            {
                buffListForRemove.Add(buffInfo);
            }
            else
            {
                buffInfo.duraTimer -= Time.deltaTime;
            }
        }
        foreach(var buffinfo in buffListForRemove)
        {
            RemoveBuff(buffinfo);
        }
    }
   
    // 插入排序算法
    private void InsertionSort(LinkedList<BuffInfo> list)
    {
        LinkedListNode<BuffInfo> current = list.First?.Next;

        while (current != null)
        {
            BuffInfo currentValue = current.Value;
            LinkedListNode<BuffInfo> insertionNode = current.Previous;

            while (insertionNode != null && insertionNode.Value.buffData.priority > currentValue.buffData.priority)
            {
                insertionNode = insertionNode.Previous;
            }

            LinkedListNode<BuffInfo> next = current.Next;
            list.Remove(current);

            if (insertionNode == null)
            {
                list.AddFirst(current);
            }
            else
            {
                list.AddAfter(insertionNode, current);
            }

            current = next;
        }
    }

    private BuffInfo FindBuff(int buffDataID)
    {
        foreach (var buffInfo in buffList)
        {
            if(buffInfo.buffData.id == buffDataID)
            {
                return buffInfo;
            }
        }
        return default;
    }



}
