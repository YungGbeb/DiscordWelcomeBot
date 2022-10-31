using DotNetty.Common.Utilities;
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Discord;

namespace WelcomeBot
{
    public enum Type
    {
        BUTTON,
        GIF
    }

    public interface IMessageType
    {
        Type GetMessageType();
    }

    public class Message<T, K> : IComparable, IMessageType where K : IComparable 
    {


        public T key;
        public K value;
        public Type type;

        public Message(T key, K value, Type type = Type.GIF)
        {
            this.key = key;
            this.value = value;
            this.type = type;
        }

        public Type GetMessageType()
        {
            return type;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            Message<T, K> pair = obj as Message<T, K>;
            return pair.value.CompareTo(value);
        }
    }

    public class Comparer<T> : IComparer<T> where T:IMessageType
    {
        public int Compare(T first, T second)
        {
            if (first.GetMessageType() == second.GetMessageType())
            {
                Console.WriteLine("second = first");
                return ((new CaseInsensitiveComparer()).Compare(second, first));
            }
            if (first.GetMessageType() == Type.BUTTON)
            {
                Console.WriteLine("-1");
                return 1;
            }
            if (first.GetMessageType() == Type.GIF)
            {
                Console.WriteLine("1");
                return -1;
            }

            return ((new CaseInsensitiveComparer()).Compare(second, first));
        }
    }

    public class ConcurrentPriorityQueue<T> where T : class, IMessageType
    {
        Mutex mutex;

        PriorityQueue<T> queue;
        public ConcurrentPriorityQueue()
        {
            mutex = new Mutex();
            queue = new PriorityQueue<T>(new Comparer<T>());
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
