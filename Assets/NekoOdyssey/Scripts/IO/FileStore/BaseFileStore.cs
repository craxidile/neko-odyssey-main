namespace NekoOdyssey.Scripts.IO.FileStore
{
    public interface IBaseFileStore
    {
        void CreateNew(string fileName);
        void Load<T>(string fileName);
        void Save<T>(string fileName, object data);
        void Remove(string fileName);
    }
}