using System.Runtime.InteropServices;
using System.Text;

namespace Giphy.Application.Threading;

public class MurmurHash2
{
    public static uint Hash(byte[] data)
    {
        return Hash(data, 0xc58f1a7b);
    }

    const UInt32 m = 0x5bd1e995;
    const Int32 r = 24;

    [StructLayout(LayoutKind.Explicit)]
    struct BytetoUInt32Converter
    {
        [FieldOffset(0)] public Byte[] Bytes;

        [FieldOffset(0)] public UInt32[] UInts;
    }

    public static UInt32 Hash(Byte[] data, UInt32 seed)
    {
        var length = data.Length;
        if (length == 0)
            return 0;
        var h = seed ^ (uint)length;
        var currentIndex = 0;
        // array will be length of Bytes but contains Uints
        // therefore the currentIndex will jump with +1 while length will jump with +4
        uint[] hackArray = new BytetoUInt32Converter { Bytes = data }.UInts;
        while (length >= 4)
        {
            var k = hackArray[currentIndex++];
            k *= m;
            k ^= k >> r;
            k *= m;

            h *= m;
            h ^= k;
            length -= 4;
        }

        currentIndex *= 4; // fix the length
        switch (length)
        {
            case 3:
                h ^= (ushort)(data[currentIndex++] | data[currentIndex++] << 8);
                h ^= (uint)data[currentIndex] << 16;
                h *= m;
                break;
            case 2:
                h ^= (ushort)(data[currentIndex++] | data[currentIndex] << 8);
                h *= m;
                break;
            case 1:
                h ^= data[currentIndex];
                h *= m;
                break;
        }

        // Do a few final mixes of the hash to ensure the last few
        // bytes are well-incorporated.

        h ^= h >> 13;
        h *= m;
        h ^= h >> 15;

        return h;
    }
}

public class ConsistentHash<T> where T : new()
{
    SortedDictionary<int, T> circle = new();
    private int _replicate = 100; //default _replicate count
    private int[] ayKeys = null; //cache the ordered keys for better performance

    public ConsistentHash(int i)
    {
        Init(i);
    }

    //it's better you override the GetHashCode() of T.
    //we will use GetHashCode() to identify different node.
    public void Init(IEnumerable<T> nodes)
    {
        Init(nodes, _replicate);
    }

    public void Init(int count)
    {
        var nodes = new T[count];
        for (var i = 0; i < count; i++)
        {
            nodes[i] = new T();
        }

        Init(nodes);
    }

    public void Init(IEnumerable<T> nodes, int replicate)
    {
        _replicate = replicate;

        foreach (var node in nodes)
        {
            Add(node, false);
        }

        ayKeys = circle.Keys.ToArray();
    }

    public void Add(T node)
    {
        Add(node, true);
    }

    private void Add(T node, bool updateKeyArray)
    {
        for (var i = 0; i < _replicate; i++)
        {
            var hash = BetterHash(node.GetHashCode().ToString() + i);
            circle[hash] = node;
        }

        if (updateKeyArray)
        {
            ayKeys = circle.Keys.ToArray();
        }
    }

    public void Remove(T node)
    {
        for (var i = 0; i < _replicate; i++)
        {
            var hash = BetterHash(node.GetHashCode().ToString() + i);
            if (!circle.Remove(hash))
            {
                throw new Exception("can not remove a node that not added");
            }
        }

        ayKeys = circle.Keys.ToArray();
    }


    //return the index of first item that >= val.
    //if not exist, return 0;
    //ay should be ordered array.
    int First_ge(IReadOnlyList<int> ay, int val)
    {
        var begin = 0;
        var end = ay.Count - 1;

        if (ay[end] < val || ay[0] > val)
        {
            return 0;
        }

        while (end - begin > 1)
        {
            var mid = (end + begin) / 2;
            if (ay[mid] >= val)
            {
                end = mid;
            }
            else
            {
                begin = mid;
            }
        }

        if (ay[begin] > val || ay[end] < val)
        {
            throw new Exception("should not happen");
        }

        return end;
    }

    public T GetNode(string key)
    {
        //return GetNode_slow(key);

        int hash = BetterHash(key);

        int first = First_ge(ayKeys, hash);

        //int diff = circle.Keys[first] - hash;

        return circle[ayKeys[first]];
    }

    //default String.GetHashCode() can't well spread strings like "1", "2", "3"
    public static int BetterHash(String key)
    {
        uint hash = MurmurHash2.Hash(Encoding.ASCII.GetBytes(key));
        return (int)hash;
    }
}