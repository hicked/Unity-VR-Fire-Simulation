using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class ArduinoIO : MonoBehaviour
{
    [SerializeField] public static string arduinoPort = "COM8"; // Update COM port as necessary
    private Thread ArduinoThread;
    public static float[] speeds = new float[2];
    public static float leftSpeed = 0;
    public static float rightSpeed = 0;
    private static SerialPort port;
    private static readonly object lockObject = new object();

    private static void IOThread()
    {
        try
        {
            while (port.IsOpen)
            {
                lock (lockObject)
                {
                    if (port.BytesToRead > 0)
                    {
                        string receivedString = port.ReadLine();
                        if (!string.IsNullOrEmpty(receivedString))
                        {
                            string[] strSpeeds = receivedString.Split(',');
                            if (strSpeeds.Length == 2)
                            {
                                if (float.TryParse(strSpeeds[0], out float parsedLeftSpeed))
                                {
                                    leftSpeed = parsedLeftSpeed;
                                    speeds[0] = leftSpeed;
                                }

                                if (float.TryParse(strSpeeds[1], out float parsedRightSpeed))
                                {
                                    rightSpeed = parsedRightSpeed;
                                    speeds[1] = rightSpeed;
                                }
                            }
                        }
                    }
                }
                Thread.Sleep(200);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error in IOThread: {ex.Message}");
        }
    }

    private void OnDestroy()
    {
        if (ArduinoThread != null && ArduinoThread.IsAlive)
        {
            ArduinoThread.Abort();
        }

        if (port != null && port.IsOpen)
        {
            port.Close();
            port.Dispose();
        }

        Debug.Log("ArduinoIO thread stopped and port closed");
    }

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            port = new SerialPort(arduinoPort, 9600)
            {
                WriteTimeout = 300,
                ReadTimeout = 5000,
                DtrEnable = true,
                RtsEnable = true
            };

            port.Open();

            if (port.IsOpen)
            {
                ArduinoThread = new Thread(IOThread);
                ArduinoThread.Start();
                Debug.Log("ArduinoIO thread started");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to open serial port: {ex.Message}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Example usage of the speed values
        Debug.Log($"Left Speed: {leftSpeed}, Right Speed: {rightSpeed}");
    }
}
