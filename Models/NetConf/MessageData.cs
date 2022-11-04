namespace CalixManager.Models.NetConf;

public struct MessageData
{
    int id = 0;
    int? session = null;

    public int messageId
    {
        get
        {
            id++;
            return id;
        }
    }

    public int sessionId
    {
        get
        {
            ArgumentNullException.ThrowIfNull(session);
            return session.Value;
        }
        set
        {
            if (value > -1)
                session = value;
        }
    }

    public string username = "rootgod";
    public string password = "root";
    public int timeout = 35000;

    public MessageData(string username = "rootgod", string password = "root", int timeout = 35000)
    {
        this.username = username;
        this.password = password;
        this.timeout = timeout;
    }
}
