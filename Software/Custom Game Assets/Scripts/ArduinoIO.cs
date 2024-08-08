using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class ArduinoIO : MonoBehaviour
{
    [SerializeField] private string arduinoPort = "COM8"; // Update COM port as necessary
    [SerializeField] private int arduinoBaudRate = 115200; // Update baud rate as necessary

    // Add a method or property to access these values from a static context if needed
    public static string ArduinoPort { get; private set; }
    public static int ArduinoBaudRate { get; private set; }

    private Thread ArduinoThread;
    
    public static float leftSpeed = 0;
    public static float rightSpeed = 0;
    private static SerialPort port;
    public static readonly object lockObject = new object();

    private static void IOThread() {
        while (port.IsOpen && port != null) {
            if (port.BytesToRead > 0) {
                string receivedString = port.ReadLine();
                if (!string.IsNullOrEmpty(receivedString)) {
                    string[] strSpeeds = receivedString.Split(',');
                    if (strSpeeds.Length == 2) {
                        if (float.TryParse(strSpeeds[0], out float parsedLeftSpeed)) {
                            //lock (lockObject) {
                            leftSpeed = parsedLeftSpeed;
                            //}
                        }

                        if (float.TryParse(strSpeeds[1], out float parsedRightSpeed)) {
                            lock (lockObject) {
                                rightSpeed = parsedRightSpeed;
                            }
                        }
                    }
                }
            }
            Thread.Sleep(50);
        }
    }

    private void OnDestroy()
    {
        if (ArduinoThread != null && ArduinoThread.IsAlive) {
            ArduinoThread.Abort();
        }

        if (port != null && port.IsOpen) {
            port.Close();
            port.Dispose();
        }

        Debug.Log("ArduinoIO thread stopped and port closed");
    }

    // Start is called before the first frame update
    void Start() {
        port = new SerialPort(arduinoPort, arduinoBaudRate){
            WriteTimeout = 300,
            ReadTimeout = 5000,
            DtrEnable = true,
            RtsEnable = true
        };
        try {
            port.Open();
            Thread.Sleep(250);
        } catch {
            throw new UnityException("PORT IS ALREADY IN USE: Make sure to close SERIAL MONITOR");
        }
        
        if (port.IsOpen){
            ArduinoThread = new Thread(IOThread);
            ArduinoThread.Start();
            Debug.Log("ArduinoIO thread started");
        }
    }

    // Update is called once per frame
    void Update(){}
}
