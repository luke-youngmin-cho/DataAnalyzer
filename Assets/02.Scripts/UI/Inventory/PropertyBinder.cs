using System.Collections.Generic;
using System.ComponentModel;
using System;


public enum SourceTag
{
    InventoryData
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal class BindPropertyToAttribute : Attribute
{
    public string PropertyName
    {
        get;
        private set;
    }

    public SourceTag tag
    {
        get;
        private set;
    }

    public BindPropertyToAttribute(string propertyName, SourceTag sourceId)
    {
        PropertyName = propertyName;
        tag = sourceId;
    }
}

/// <summary>
/// Helps bind to depentent source
/// </summary>
/// <typeparam name="T">Type of object including source</typeparam>
internal class PropertyBinder<T>
{
    private readonly T _receiver;
    private readonly PropertyDescriptorCollection _sourceProperties;
    private readonly Dictionary<string, PropertyDescriptor> _receiverMappingProperties;

    public PropertyBinder(T receiver, INotifyPropertyChanged source, SourceTag tag)
    {
        _receiver = receiver;

        //Get a list of receiver properties
        PropertyDescriptorCollection receiverProperties = TypeDescriptor.GetProperties(receiver);

        //Get a list of source properties
        _sourceProperties = TypeDescriptor.GetProperties(source);

        //This is the source property to receiver property mapping
        _receiverMappingProperties = new Dictionary<string, PropertyDescriptor>();

        //listen for INotifyPropertyChanged event on the source
        source.PropertyChanged += SourcePropertyChanged;

        // Mapping
        foreach (PropertyDescriptor property in receiverProperties)
        {
            var attribute = (BindPropertyToAttribute)property.Attributes[typeof(BindPropertyToAttribute)];
            if (attribute != null && attribute.tag == tag)
            {
                _receiverMappingProperties[attribute.PropertyName] = property;
            }
        }
    }

    void SourcePropertyChanged(object sender, PropertyChangedEventArgs args)
    {
        if (_receiverMappingProperties.ContainsKey(args.PropertyName))
        {
            _receiverMappingProperties[args.PropertyName]
                .SetValue(_receiver, _sourceProperties[args.PropertyName]
                .GetValue(sender));
            // todo -> check setvalue argument and getvalue return value are same ( on multithread )
        }
    }
}