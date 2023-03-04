using System.Collections;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace InjectedLocalizations.Models
{
    public class FakeServiceCollection : IServiceCollection
    {
        private readonly List<ServiceDescriptor> descriptors;

        public FakeServiceCollection()
        {
            this.descriptors = new List<ServiceDescriptor>();
        }

        public ServiceDescriptor this[int index]
        {
            get
            {
                index.Should().BeInRange(0, this.descriptors.Count);
                return this.descriptors[index];
            }

            set
            {
                value.Should().NotBeNull();
                this.descriptors[index] = value;
            }
        }

        public int Count => this.descriptors.Count;
        public bool IsReadOnly => false;

        public void Add(ServiceDescriptor item) => this.descriptors.Add(item);

        public void Clear() => this.descriptors.Clear();

        public bool Contains(ServiceDescriptor item) => this.descriptors.Contains(item);

        public void CopyTo(ServiceDescriptor[] array, int arrayIndex) => this.descriptors.CopyTo(array, arrayIndex);

        public IEnumerator<ServiceDescriptor> GetEnumerator() => this.descriptors.GetEnumerator();

        public int IndexOf(ServiceDescriptor item) => this.descriptors.IndexOf(item);

        public void Insert(int index, ServiceDescriptor item) => this.descriptors.Insert(index, item);

        public bool Remove(ServiceDescriptor item) => this.descriptors.Remove(item);

        public void RemoveAt(int index) => this.descriptors.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => this.descriptors.GetEnumerator();
    }
}
