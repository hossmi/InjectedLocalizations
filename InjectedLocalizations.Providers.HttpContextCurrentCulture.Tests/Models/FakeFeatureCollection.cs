using System.Collections;
using Microsoft.AspNetCore.Http.Features;

namespace InjectedLocalizations.Models
{
    public class FakeFeatureCollection : IFeatureCollection
    {
        private readonly IDictionary<Type, object> features;

        public FakeFeatureCollection()
        {
            this.features = new Dictionary<Type, object>();
        }

        public object this[Type key]
        {
            get
            {
                if (this.features.TryGetValue(key, out object feature))
                    return feature;
                else
                    return default;
            }

            set => this.features[key] = value;
        }

        public bool IsReadOnly => true;
        public int Revision => 1;

        public TFeature Get<TFeature>()
        {
            if (this.features.TryGetValue(typeof(TFeature), out object feature))
                return (TFeature)feature;
            else
                return default;
        }

        public IEnumerator<KeyValuePair<Type, object>> GetEnumerator() => this.features.GetEnumerator();

        public void Set<TFeature>(TFeature instance) => this.features[typeof(TFeature)] = instance;

        IEnumerator IEnumerable.GetEnumerator() => this.features.GetEnumerator();
    }
}
