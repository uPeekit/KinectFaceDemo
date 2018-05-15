using System;
using System.Windows.Controls;

namespace KinectDemo
{
    public interface Parameter
    {
        string Name { get; set; }
        void CreateElement(UIElementCollection collectionToAdd);
        void ProcessValue();
        void Clear();
    }

    public abstract class Parameter<PT> : Parameter where PT : Parameter
    {
        public abstract string Name { get; set; }
        public abstract Action<PT> ElementProcessor { get; set; }
        public Parameter(string name)
        {
            Name = name;
        }
        public abstract void CreateElement(UIElementCollection collectionToAdd);
        public abstract void Clear();

        public void ProcessValue() => ElementProcessor.Invoke((PT)Convert.ChangeType(this, typeof(PT)));
    }
}