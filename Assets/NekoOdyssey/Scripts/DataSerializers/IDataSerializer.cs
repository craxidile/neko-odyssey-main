namespace NekoOdyssey.Scripts.DataSerializers
{
    public interface IDataSerializer<T>
    {
        public string Serialize(T data);
        public string[] DeserializeHeadColumns(string text);
        public T DeserializeLines(string text, int columnIndex = 0);
    }
}