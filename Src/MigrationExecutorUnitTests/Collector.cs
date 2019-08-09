namespace MigrationExecutorUnitTests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Azure.WebJobs;

    public class Collector<T> : ICollector<T>
    {
        public readonly List<T> Items = new List<T>();

        public void Add(T item)
        {
            this.Items.Add(item);
        }

        public int Count()
        {
            return this.Items.Count;
        }

        public T GetElement(int i)
        {
            if (this.Items.Count > i && i >= 0)
            {
                return this.Items[i];
            }
            else
            {
                throw new ArgumentException("Index out of range");
            }
        }
    }
}
