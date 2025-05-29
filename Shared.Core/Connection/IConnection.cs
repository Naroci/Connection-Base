namespace ClassLibrary1.Connection;

public interface IConnection
{
    void Send(object obj);
    
    void Send(byte[] objBytes);
    
    void Send(string obj);
    
    T Receive<T>();
    
    byte[] ReceiveBytes();
    
    string ReceiveString();
}