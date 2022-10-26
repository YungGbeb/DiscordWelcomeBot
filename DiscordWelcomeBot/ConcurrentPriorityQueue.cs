using DotNetty.Common.Utilities;
using System;
using System.Linq;
using System.Threading;

namespace WelcomeBot
{
    public class Pair<T, K> : IComparable where K: IComparable
    {
        public T Key;
        public K Value;
        public Pair(T Key, K Value)
        {
            this.Key = Key;
            this.Value = Value;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            Pair<T, K> pair = obj as Pair<T, K>;
            return pair.Value.CompareTo(this.Value);
        }
    }
    public class ConcurrentPriorityQueue<T> where T : class
    {
        Mutex mutex;

        PriorityQueue<T> queue;
        public ConcurrentPriorityQueue()
        {
            mutex = new Mutex();
            queue = new PriorityQueue<T>();
        }

        public void Enqueue(T value)
        {
            mutex.WaitOne();
            queue.Enqueue(value);
            mutex.ReleaseMutex();
        }

        public void Dequeue()
        {
            mutex.WaitOne();
            queue.Dequeue();
            mutex.ReleaseMutex();
        }

        public T First()
        {
            mutex.WaitOne();
            var firstValue = queue.First();
            mutex.ReleaseMutex();
            return firstValue;
        }

        public int Count()
        {
            mutex.WaitOne();
            var valueCount = queue.Count;
            mutex.ReleaseMutex();
            return valueCount;
        }

        
    }
}
