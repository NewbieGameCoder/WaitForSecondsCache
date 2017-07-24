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
