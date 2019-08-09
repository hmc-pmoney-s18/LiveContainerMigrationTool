using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace UnitTestProject1
{
    public class Collector<T> : ICollector<T>
    {
        public readonly List<T> Items = new List<T>();

        public void Add(T item)
        {
            Items.Add(item);
        }

        public int Count()
        {
            return this.Items.Count;
        }

        public Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {

            return Task.FromResult(true);

        }

        public T GetElement(int i)
        {
            if(this.Items.Count > i && i >= 0)
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
