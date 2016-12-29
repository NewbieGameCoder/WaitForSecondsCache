using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaitForSecondsCache
{
    const int cacheSize = 64;
    const int fractionalPartMultiplicand = 100;

    static LinkedList<int> cacheQueue = new LinkedList<int>();
    static Dictionary<int, WaitForSeconds> cache = new Dictionary<int, WaitForSeconds>(cacheSize);

    public static WaitForSeconds Wait(float waitingTime)
    {
        WaitForSeconds unit = null;
        int clippedNumber = (int)(fractionalPartMultiplicand * waitingTime);

        cache.TryGetValue(clippedNumber, out unit);
        if (unit == null)
        {
            if (cache.Count >= cacheSize)
            {
                cache.Remove(cacheQueue.Last.Value);
                cacheQueue.RemoveLast();
            }
            unit = new WaitForSeconds(waitingTime);
            cache.Add(clippedNumber, unit);
        }

        if (cacheQueue.Count == 0) cacheQueue.AddFirst(clippedNumber);
        else MoveToTop(clippedNumber, cacheQueue);

        return unit;
    }

#if UNITY_5_4_OR_NEWER
    static LinkedList<int> realTimeCacheQueue = new LinkedList<int>();
    static Dictionary<int, WaitForSecondsRealtime> realTimeCache = new Dictionary<int, WaitForSecondsRealtime>(cacheSize);

    public static WaitForSecondsRealtime WaitForRealTime(float waitingTime)
    {
        WaitForSecondsRealtime unit = null;
        int clippedNumber = (int)(fractionalPartMultiplicand * waitingTime);

        realTimeCache.TryGetValue(clippedNumber, out unit);
        if (unit == null)
        {
            if (realTimeCache.Count >= cacheSize)
            {
                realTimeCache.Remove(realTimeCacheQueue.Last.Value);
                realTimeCacheQueue.RemoveLast();
            }
            unit = new WaitForSecondsRealtime(waitingTime);
            realTimeCache.Add(clippedNumber, unit);
        }

        if (realTimeCacheQueue.Count == 0) realTimeCacheQueue.AddFirst(clippedNumber);
        else MoveToTop(clippedNumber, realTimeCacheQueue);

        return unit;
    }
#endif

    static void MoveToTop(int elementValue, LinkedList<int> cacheQueue)
    {
        LinkedListNode<int> existElement = null;
        existElement = cacheQueue.First;

        do
        {
            if (existElement.Value == elementValue)
                break;
            existElement = existElement.Next;
        }
        while (existElement != null);

        if (existElement != null)
        {
            cacheQueue.Remove(existElement);
            cacheQueue.AddFirst(existElement);
        }
        else cacheQueue.AddFirst(elementValue);
    }
}
