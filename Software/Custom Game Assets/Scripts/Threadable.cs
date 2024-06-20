using System;
using System.Collections.Generic;
using UnityEngine;

public class Threadable : MonoBehaviour {
    private bool onlyRunLatestQueuedFunction = false; // by default will run all of the functions in the queue
    public volatile Queue<Action> queuedFunctions = new Queue<Action>(); 

    public void QueueFunction(Action function) {
        lock (queuedFunctions) {
            queuedFunctions.Enqueue(function);
        }
    }

    public void runQueuedFunctions() {
        lock (queuedFunctions) { // locks the queued functions so it can't be added to while running them
            if (queuedFunctions.Count > 0) {
                if (onlyRunLatestQueuedFunction) {
                    Debug.Log("Applying latest force");
                    Action function = queuedFunctions.Dequeue();
                    function.Invoke();
                    queuedFunctions.Clear();
                } else { 
                    while (queuedFunctions.Count > 0) {
                        Debug.Log("Applying force");
                        Action function = queuedFunctions.Dequeue();
                        function.Invoke();
                    }
                }
            }
        }
    }

    public void RunOnlyLatestQueuedFunc(bool input) {
        onlyRunLatestQueuedFunction = input;
    }
}
