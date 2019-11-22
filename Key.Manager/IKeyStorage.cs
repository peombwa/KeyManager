namespace Key.Manager
{
    public interface IKeyStorage
    {
        byte[] ReadContent();

        void WriteContent(byte[] content);

        void ClearContent();
    }
}