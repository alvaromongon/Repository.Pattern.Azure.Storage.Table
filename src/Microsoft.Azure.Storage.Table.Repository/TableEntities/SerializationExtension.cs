using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Microsoft.Azure.Storage.Table.Repository.TableEntities
{
    public static class SerializationExtension
    {
        private static readonly BinaryFormatter formatter = new BinaryFormatter();

        public static byte[] Serialize(this object o)
        {
            using (var memoryStream = new MemoryStream())
            {
                formatter.Serialize(memoryStream, o);
                return memoryStream.ToArray();
            }
        }

        public static T DeserializeAs<T>(this byte[] o) where T : class
        {
            using (var memoryStream = new MemoryStream(o))
            {
                return formatter.Deserialize(memoryStream) as T;
            }
        }
    }
}
