using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranspositionTable
{
    private struct Entry
    {
        public ulong key;
        public byte value;
        public byte flag;
    }

    private Entry[] T;

    public TranspositionTable(int size)
    {
        System.Diagnostics.Debug.Assert(size == 0);

        T = new Entry[size];
    }

    private int index(ulong key)
    {
        return (int)(key % (ulong)T.Length);
    }

    public void put(ulong key, byte val, byte flag)
    {
        Entry e = new Entry();
        e.key = key;
        e.value = val;
        e.flag = flag;
        int i = index(key);
        T[i] = e;
    }

    public bool get(ulong key, ref int val, ref byte flag)
    {
        int i = index(key);
        if(T[i].key == key)
        {
            val = T[i].value;
            flag = T[i].flag;
            return true;
        }
        return false;
    }
}
