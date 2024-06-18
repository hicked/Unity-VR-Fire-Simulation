using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class ParticleInteractions : MonoBehaviour {
    public ParticleSystem particleSystem;
    public float interactionRadius = 1f;
    public float forceStrength = 1f;

    private ParticleSystem.Particle[] particles;
    private ParticleSystem.Particle[] particlesWForces;
    private Thread particleInteractionsThread;
    private Queue<Action> queuedFunctions = new Queue<Action>(); // Use Queue instead of List

    void Start() {
        particleSystem = GetComponent<ParticleSystem>();

        particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
        particlesWForces = new ParticleSystem.Particle[particleSystem.main.maxParticles];

        particleInteractionsThread = new Thread(ParticleColliderThread);
        particleInteractionsThread.Start();
    }

    void Update() {
        int particleCount = particleSystem.GetParticles(particles);

        // Process queued functions in the main thread
        lock (queuedFunctions) {
            while (queuedFunctions.Count > 0) {
                Debug.Log("applying force");
                Action function = queuedFunctions.Dequeue();
                function.Invoke();
            }
        }
    }

    void OnDestroy() {
        if (particleInteractionsThread != null && particleInteractionsThread.IsAlive) {
            particleInteractionsThread.Join();
        }
    }

    private void ParticleColliderThread() {
        while (true) {
            Array.Copy(particles, particlesWForces, particles.Length);

            int particleCount = particlesWForces.Length;
            for (int i = 0; i < particleCount; i++) {
                for (int j = i + 1; j < particleCount; j++) {
                    Vector3 direction = particlesWForces[j].position - particlesWForces[i].position;
                    float distance = direction.magnitude;

                    if (distance < interactionRadius) {
                        Vector3 force = direction.normalized * forceStrength * (interactionRadius - distance);
                        particlesWForces[i].position -= force;
                        particlesWForces[j].position += force;
                    }
                }
            }

            // Update particles array in the main thread
            Action setParticleVelocities = () => {
                Array.Copy(particlesWForces, particles, particles.Length);
            };
            QueueFunction(setParticleVelocities);

            Thread.Sleep(10); // Adjust sleep time as needed
        }
    }

    private void QueueFunction(Action function) {
        lock (queuedFunctions) {
            Debug.Log("qed function");
            queuedFunctions.Enqueue(function);
        }
    }
}
