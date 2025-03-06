using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class SmokeInteractions : Threadable {
    [SerializeField] public float maxParticleHeight; 
    public ParticleSystem particleSystem;
    public float interactionRadius = 1f;
    public float forceStrength = 1f;

    private ParticleSystem.Particle[] particles;
    private ParticleSystem.Particle[] particlesWForces;
    private Thread particleInteractionsThread;

    private bool running;
    private int particleCount;

    void Start() {
        RunOnlyLatestQueuedFunc(true); // we only want the lastest particle velocities to be applied

        particleSystem = GetComponent<ParticleSystem>();

        particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
        particlesWForces = new ParticleSystem.Particle[particleSystem.main.maxParticles];

        running = true;
        particleInteractionsThread = new Thread(ParticleColliderThread); // only have one thread for all particles
        particleInteractionsThread.Start();
    }

    void FixedUpdate() {
        lock (particles) {
            runQueuedFunctions();
            particleCount = particleSystem.GetParticles(particles);
        }
    }

    void OnDestroy() {
        running = false;
        if (particleInteractionsThread != null && particleInteractionsThread.IsAlive) {
            particleInteractionsThread.Join();
        }
    }

    private void ParticleColliderThread() {
        // Need to use c# random because Unity's random is not thread safe
        System.Random random = new System.Random();
        while (running) {
            lock (particles) {
                Array.Copy(particles, particlesWForces, particles.Length);
                
                int particleCount = particlesWForces.Length;
                for (int i = 0; i < particleCount; i++) {
                    if (particlesWForces[i].position.y > maxParticleHeight) {
                        particlesWForces[i].remainingLifetime = 0f;
                        continue;
                    }
                    

                    for (int j = i + 1; j < particleCount; j++) {
                        Vector3 direction = particlesWForces[j].position - particlesWForces[i].position;
                        float distance = direction.magnitude;

                        if (distance < interactionRadius) {
                            Vector3 force = (direction.normalized + Vector3.down/3f) * forceStrength * (interactionRadius - distance) * random.Next(3, 17)/10; // *0.3 -> 1.7
                            particlesWForces[i].velocity += force;
                            particlesWForces[j].velocity -= force;
                            particlesWForces[i].velocity += new Vector3(0, -0.002f, 0);
                            particlesWForces[j].velocity -= new Vector3(0, -0.002f, 0);
                        }
                    }
                }

                // Array.Copy(particlesWForces, particles, particles.Length);
                // Update particles array in the main thread
                Action setParticleVelocities = () => {
                    particleSystem.SetParticles(particlesWForces, particleCount);
                };

                QueueFunction(setParticleVelocities);
            }
            

            Thread.Sleep(100); // Adjust sleep time as needed
        }
    }
}
