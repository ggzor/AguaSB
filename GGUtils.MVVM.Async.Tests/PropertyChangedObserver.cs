using System.Collections.Generic;
using System.ComponentModel;

namespace GGUtils.MVVM.Async.Tests
{
    public class PropertyChangedObserver
    {
        public List<string> ChangedProperties { get; } = new List<string>();

        public PropertyChangedEventHandler EventHandler { get; }

        public PropertyChangedObserver() =>
            EventHandler = (src, args) => ChangedProperties.Add(args.PropertyName);
    }
}
