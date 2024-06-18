//-----------------------------------------------------------------------
// <copyright file="ExampleScript.cs" company="Quill18 Productions">
//     Copyright (c) Quill18 Productions. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class ExampleScript : MonoBehaviour {
    void Start() {
        Debug.Log("Start() :: Starting.");

        foo = "Computer Science";

        someVector = new Vector3(1, 1, 1);


        // SlowJob is a variable that contains some function code
        // When you add the () at the end of the SlowJob variable
        // name, that's a shortcut to tell C# "execute the contents of variable SlowJob"

        myThread = new Thread(SlowJob);

        // Now run SlowJob in a new thread
        myThread.Start();

        Debug.Log("Start() :: Done.");
    }

    Thread myThread;

    void Update() {
        if (myThread.IsAlive) {
            Debug.Log("SlowJob isRunning");
        }
        else {
            // The thread is done, so copy the pixels back to the texture
            someTexture.SetPixels(pixels);
        }

        someVector = new Vector3(2, 2, 2);

        Vector3 pos = this.transform.position;
        pos.x = 3;
        this.transform.position = pos;

        //this.transform.Translate( Vector3.up * Time.deltaTime );

        pixels = someTexture.GetPixels();
    }

    Texture2D someTexture;
    Color[] pixels;

    void PrintStudentID() {
        // This send "foo" to the printer
        // This should print out    "Computer Science"
        // Or maybe it'll print out "English Literature"
        // Either is legit
        // What isn't legit is if in the MIDDDLE of printing, the
        // other thread makes a change and suddenly we get a 
        // student ID that says:    "Compute Literature"

        // Let's make sure no one is changing our data mid-way through
        // our printing operation.

        lock (FrontDoor) {
            // Print out the student ID here, safe in the
            // knowledge that nothing is messing with our data.
        }

    }

    void FixedUpdate() {

    }

    string foo;

    Vector3 someVector;

    object FrontDoor = new object();

    void SlowJob() {
        Debug.Log("ExampleScript::SlowJob() -- Doing 1000 things, each taking 2ms.");

        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        someVector = new Vector3(0, 0, 0);

        for (int i = 0; i < 1000; i++) {

            Thread.Sleep(2);    // Sleep for 2ms

        }

        sw.Stop();

        // NOTE: Because of various overheads and context-switches, elapsed time
        // will be slightly more than 2 seconds.
        Debug.Log("ExampleScript::SlowJob() -- Done! Elapsed time: " + sw.ElapsedMilliseconds / 1000f);

    }
}
