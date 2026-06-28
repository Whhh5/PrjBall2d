namespace Protocol
{
    public interface IServer_RSP
    { }

    public enum EnProtocol
    {
        NONE = 0,
        REQ_DOWNLOAD_FILE,
        RSP_DOWNLOAD_FILE,
        REQ_DOWNLOAD_BYTE,
        RSP_DOWNLOAD_BYTE,
    }
    public class REQ_DOWNLOAD_FILE
    {
        public int id;
        public string fileLocalPath;
    }
    public class RSP_DOWNLOAD_FILE : IServer_RSP
    {
        public int id;
        public string content;
    }
    public class REQ_DOWNLOAD_BYTE
    {
        public int id;
        public string fileLocalPath;
    }
    public class RSP_DOWNLOAD_BYTE : IServer_RSP
    {
        public int id;
        public byte[] content;
    }
    public class ProtocolInfo
    {
        public int id;
        public EnProtocol protocolID;
        public string content;
    }
}