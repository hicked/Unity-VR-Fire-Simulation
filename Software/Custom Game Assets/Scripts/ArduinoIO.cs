using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class ArduinoIO : MonoBehaviour {
    Thread ArduinoThread = new Thread(IOThread);
    [SerializeField] private static string arduinoPort; // COM5 as an example
    private static SerialPort port;
    private static string incomingData = "";
    private static string outgoingData = "";

    private static void IOThread() {
        port = new SerialPort(arduinoPort, 9600);
        port.Open();

        while (true) {
            if (outgoingData != "") {
                port.Write(outgoingData);
                outgoingData = "";
            }
            incomingData = port.ReadExisting();
            Thread.Sleep(200);
        }
    }

    private void OnDestroy() {
        ArduinoThread.Abort();
        port.Close();
    }

    // Start is called before the first frame update
    void Start() {
        ArduinoThread.Start(); 
    }

    // Update is called once per frame
    void Update() {
        if (incomingData != "") {
            Debug.Log(incomingData);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            outgoingData = "1";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0)) {
            outgoingData = "0";
        }
    }
}
