using UnityEngine;
using System;
using System.Collections;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class TCPClient : MonoBehaviour
{
    public string TCP_SERVER_HOST = "127.0.0.1";
    public int TCP_SERVER_PORT = 8081;

    private TcpClient client;
    private NetworkStream stream;
    private Thread clientThread;
    
    public FacialExpression curFacialExpression;

    private volatile bool isConnected = false;

    private object dataLock = new object();
    private string receivedData = "";

    void Start()
    {
        clientThread = new Thread(new ThreadStart(ConnectToServer));
        clientThread.IsBackground = true;
        clientThread.Start();
    }

    void Update()
    {
        string dataToProcess = "";

        lock (dataLock)
        {
            dataToProcess = receivedData;
            receivedData = "";
        }

        if (!string.IsNullOrEmpty(dataToProcess))
        {
            try
            {
                FacialExpressions facialExpressions = JsonUtility.FromJson<FacialExpressions>(dataToProcess);

                if (facialExpressions.emotions == null || facialExpressions.emotions.Count == 0)
                {
                    Debug.Log("No emotions detected");
                    curFacialExpression = null;
                }
                else
                {
                    curFacialExpression = facialExpressions.emotions[facialExpressions.emotions.Count - 1];
                }

            }
            catch (Exception e)
            {
                Debug.LogError("Error: " + e.Message);
            }
        }
    }

    void ConnectToServer()
    {
        try
        {
            client = new TcpClient(TCP_SERVER_HOST, TCP_SERVER_PORT);
            stream = client.GetStream();
            isConnected = true;
            Debug.Log("Connected to server");

            while (isConnected)
            {
                byte[] lengthBuffer = new byte[4];
                int bytesRead = stream.Read(lengthBuffer, 0, lengthBuffer.Length);

                if (bytesRead == 0)
                {
                    Debug.Log("Connection closed by the server");
                    break;
                }

                // Convert the length buffer to an integer (assuming little-endian byte order)
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(lengthBuffer);
                }

                int dataLength = BitConverter.ToInt32(lengthBuffer, 0);

                // Log the data length in decimal format
                byte[] data = ReceiveAll(dataLength);

                if (data == null)
                {
                    Debug.Log("Connection closed by the server");
                    break;
                }

                string dataString = Encoding.UTF8.GetString(data);

                lock (dataLock)
                {
                    receivedData = dataString;
                }
            }
        }
        catch (IOException ioEx)
        {
            Debug.LogError("IO Exception: " + ioEx.Message);
        }
        catch (SocketException sockEx)
        {
            Debug.LogError("Socket Exception: " + sockEx.Message);
        }
        catch (Exception e)
        {
            Debug.LogError("Exception: " + e.Message);
        }
        finally
        {
            isConnected = false;
            if (stream != null) stream.Close();
            if (client != null) client.Close();
        }
    }

    byte[] ReceiveAll(int length)
    {
        byte[] data = new byte[length];
        int totalReceived = 0;

        while (totalReceived < length)
        {
            int received = stream.Read(data, totalReceived, length - totalReceived);

            if (received == 0)
            {
                return null;
            }

            totalReceived += received;
        }

        return data;
    }

    void OnApplicationQuit()
    {
        isConnected = false;

        if (client != null)
        {
            client.Close();
        }

        if (clientThread != null && clientThread.IsAlive)
        {
            clientThread.Join();
        }
    }
}
[Serializable]
public class FacialExpressions
{
    public List<FacialExpression> emotions;

    public override string ToString()
    {
        string result = "";

        foreach (FacialExpression emotion in emotions)
        {
            result += "Angry: " + emotion.angry + "\n";
            result += "Disgust: " + emotion.disgust + "\n";
            result += "Fear: " + emotion.fear + "\n";
            result += "Happy: " + emotion.happy + "\n";
            result += "Sad: " + emotion.sad + "\n";
            result += "Surprise: " + emotion.surprise + "\n";
            result += "Neutral: " + emotion.neutral + "\n";
        }

        return result;
    }
}

[Serializable]
public class FacialExpression
{
    public float angry;
    public float disgust;
    public float fear;
    public float happy;
    public float sad;
    public float surprise;
    public float neutral;
}