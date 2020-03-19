namespace QarnotCLI
{
    using System.Reflection;

    public interface IObjectTransformer
    {
        void Transform<T>(T objToTransform);
    }

    public class ConnectionTransformer : IObjectTransformer
    {

        private void DeleteConnection<T>(T obj, FieldInfo field)
        {
            if (field.FieldType == typeof(QarnotSDK.Connection))
            {
                field.SetValue(obj, null);
            }
        }

        public void Transform<T>(T obj)
        {
            BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (var field in obj.GetType().GetFields(FLAGS))
            {
                DeleteConnection(obj, field);
            }
        }
    }
}
