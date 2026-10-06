namespace Nodeheim.Persistence.Json;

public sealed class JsonDocumentStorage
{
    private readonly string _path;

    public JsonDocumentStorage(string path)
    {
        _path = path;
    }

    public void Load()
    {
        byte[] bytes = File.ReadAllBytes(_path);
    }
}
