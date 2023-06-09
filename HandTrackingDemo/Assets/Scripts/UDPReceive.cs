using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPReceive : MonoBehaviour
{

    Thread receiveThread;
    UdpClient client;
    public int port = 5052;
    public bool startReceiving = true;
    public bool printToConsole = false;
    public string data;


    public void Start()
    {

        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        // so game can keep running even if packets are not sent
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }


    // receive thread
    private void ReceiveData()
    {

        client = new UdpClient(port);

        while (startReceiving)
        {

            try
            {
                // receive from any IP (very secure!!!)
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] dataByte = client.Receive(ref anyIP);

                // receive JSON data and format accordingly
                data = Encoding.UTF8.GetString(dataByte);

                if (printToConsole) {
                    print(data);
                }
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

}
