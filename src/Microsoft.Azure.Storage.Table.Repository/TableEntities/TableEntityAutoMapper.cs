using FastMember;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Storage.Table.Repository.TableEntities
{
    internal class TableEntityAutoMapper<T> where T : class, new()
    {
        internal T ToDomainModel(string partitionKey, string rowKey, DateTimeOffset timestamp, IDictionary<string, EntityProperty> properties, string etag)
        {
            dynamic entity = new T();

            var objectAccessor = ObjectAccessor.Create(entity);

            foreach (var keyValuePair in properties)
            {
                objectAccessor[keyValuePair.Key] = GetEntityPropertyValue(keyValuePair.Value);
            }

            return entity;
        }

        internal ITableEntity ToTableEntity(T domainModel, string parititonKey, string rowKey)
        {
            IDictionary<string, EntityProperty> properties = new Dictionary<string, EntityProperty>();

            var typeAccessor = TypeAccessor.Create(typeof(T));
            var objectAccessor = ObjectAccessor.Create(domainModel);

            foreach (var member in typeAccessor.GetMembers())
            {
                properties.Add(member.Name, BuildEntityProperty(member, objectAccessor[member.Name]));
            }

            return new DynamicTableEntity(parititonKey, rowKey, null, properties);
        }

        private static dynamic GetEntityPropertyValue(EntityProperty entityProperty)
        {
            switch (entityProperty.PropertyType)
            {
                case EdmType.String:
                default:
                    return entityProperty.StringValue;
                case EdmType.Binary:
                    return entityProperty.BinaryValue;
                case EdmType.Boolean:
                    return entityProperty.BooleanValue.Value;
                case EdmType.DateTime:
                    return entityProperty.DateTimeOffsetValue.Value;
                case EdmType.Double:
                    return entityProperty.DoubleValue.Value;
                case EdmType.Guid:
                    return entityProperty.GuidValue.Value;
                case EdmType.Int32:
                    return entityProperty.Int32Value.Value;
                case EdmType.Int64:
                    return entityProperty.Int64Value.Value;
            }
        }        

        private static EntityProperty BuildEntityProperty(Member member, object value)
        {
            EntityProperty result = null;

            var memberType = member.Type;

            if (memberType.Equals(typeof(string)))
            {
                result = new EntityProperty(value as string);
            }
            else if (memberType.Equals(typeof(byte[])))
            {
                result = new EntityProperty(value as byte[]);
            }
            else if (memberType.Equals(typeof(bool)) || memberType.Equals(typeof(bool?)))
            {
                result = new EntityProperty(value as bool?);
            }
            else if (memberType.Equals(typeof(DateTimeOffset)) || memberType.Equals(typeof(DateTimeOffset?)))
            {
                result = new EntityProperty(value as DateTimeOffset?);
            }
            else if (memberType.Equals(typeof(double)) || memberType.Equals(typeof(double?)))
            {
                result = new EntityProperty(value as double?);
            }
            else if (memberType.Equals(typeof(Guid)) || memberType.Equals(typeof(Guid?)))
            {
                result = new EntityProperty(value as Guid?);
            }
            else if (memberType.Equals(typeof(int)) || memberType.Equals(typeof(int?)))
            {
                result = new EntityProperty(value as int?);
            }
            else if (memberType.Equals(typeof(long)) || memberType.Equals(typeof(long?)))
            {
                result = new EntityProperty(value as long?);
            }
            else
            {
                result = new EntityProperty(value.ToString());
            }

            return result;
        }
    }
}
