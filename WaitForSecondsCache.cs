using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaitForSecondsCache
{
    const int unitSize = 8;
    const int cacheSize = 64;
    const int fractionalPartMultiplicand = 100;

    static LinkedList<int> cacheQueue = new LinkedList<int>();
    static Dictionary<int, Stack<WaitForSeconds>> cache = new Dictionary<int, Stack<WaitForSeconds>>(cacheSize);

    public static IEnumerator Wait(float waitingTime)
    {
        WaitForSeconds unit = null;
        Stack<WaitForSeconds> unitList = null;
        int clippedNumber = (int)(fractionalPartMultiplicand * waitingTime);

        cache.TryGetValue(clippedNumber, out unitList);
        if (unitList == null)
        {
            if (cache.Count >= cacheSize)
            {
                cache.TryGetValue(cacheQueue.Last.Value, out unitList);
                cache.Remove(cacheQueue.Last.Value);
                cacheQueue.RemoveLast();
                unitList.Clear();
            }
            else unitList = new Stack<WaitForSeconds>(unitSize);

            cache.Add(clippedNumber, unitList);
        }

        if (unitList.Count == 0) unit = new WaitForSeconds(waitingTime);
        else unit = unitList.Pop();
        if (cacheQueue.Count == 0) cacheQueue.AddFirst(clippedNumber);
        else MoveToTop(clippedNumber);

        yield return unit;

        cache.TryGetValue(clippedNumber, out unitList);
        if (unitList != null && unitList.Count < unitSize) unitList.Push(unit);
    }

    static void MoveToTop(int elementValue)
    {
        var existElement = cacheQueue.Find(elementValue);

        if (existElement != null)
        {
            cacheQueue.Remove(existElement);
            cacheQueue.AddFirst(existElement);
        }
        else cacheQueue.AddFirst(elementValue);
    }
}
