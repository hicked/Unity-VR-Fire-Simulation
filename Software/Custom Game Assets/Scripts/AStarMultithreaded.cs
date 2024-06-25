using System.Collections.Concurrent;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class AStarMultithreaded : Threadable {
    [SerializeField] public static int maxPathfindingThreads = 3;
    private static List<Thread> activePathfindingThreads = new List<Thread>();
    private static Queue<Thread> queuedPathfindingThreads = new Queue<Thread>();
    
    private ConcurrentQueue<PhysicsRequest> physicsRequests = new ConcurrentQueue<PhysicsRequest>();
    private ConcurrentQueue<PhysicsResult> physicsResults = new ConcurrentQueue<PhysicsResult>();

    private class PhysicsRequest {
        public Vector3 Location;
        public Vector3 Direction;
        public float Distance;
        public int Layer;
        public Action<PhysicsResult> Callback;
    }

    private class PhysicsResult {
        public bool Hit;
        public RaycastHit HitInfo;
        public PhysicsRequest Request;
    }

    [SerializeField] public float tileSize = 1f;
    [SerializeField] private LayerMask layer;
    [SerializeField] public NPCManager NPC;
    [SerializeField] private int maxTilesAllowedToCheck = 5000;
    private float NPCHeight;
    private float NPCRadius;
    public bool isPathfinding;
    public Vector3 lookatVector;

    private List<Location> openList = new List<Location>();
    private HashSet<Location> closedList = new HashSet<Location>();
    private Dictionary<Location, Location> cameFrom = new Dictionary<Location, Location>();
    private List<Location> validNeighbors = new List<Location>();
    private List<Location> path = null;
    Location currentLocation;
    private Vector3[] directions = {
        new Vector3(1f, 0f, 0f),
        new Vector3(-1f, 0f, 0f),
        new Vector3(0f, 0f, 1f),
        new Vector3(0f, 0f, -1f),
        new Vector3(1f, 0f, 1f),
        new Vector3(-1f, 0f, 1f),
        new Vector3(-1f, 0f, -1f),
        new Vector3(1f, 0f, -1f)
    };

    private void Start() {
        NPC = GetComponent<NPCManager>();
        NPCHeight = NPC.NPCHeight;
        NPCRadius = NPC.NPCRadius;
    }

    private void Update() {
        runQueuedFunctions();

        List<Thread> threadsToRemove = new List<Thread>();

        // Process pathfinding threads and manage thread pools
        for (int i = 0; i < activePathfindingThreads.Count; i++) {
            if (!activePathfindingThreads[i].IsAlive) {
                Debug.Log("Thread finished running.");
                threadsToRemove.Add(activePathfindingThreads[i]);
                activePathfindingThreads.RemoveAt(i);
            }
        }
        foreach (Thread thread in threadsToRemove) {
            DestroyThread(thread);
            Debug.Log("Thread destroyed.");
        }

        if (activePathfindingThreads.Count < maxPathfindingThreads && queuedPathfindingThreads.Count > 0) {
            while ((maxPathfindingThreads - activePathfindingThreads.Count) > 0 && queuedPathfindingThreads.Count != 0) {
                Thread threadToStart = queuedPathfindingThreads.Dequeue();
                threadToStart.Start();
                Debug.Log("Spot found, starting thread.");
                activePathfindingThreads.Add(threadToStart);
            }
        }

        // Process physics requests
        while (physicsRequests.TryDequeue(out PhysicsRequest request)) {
            RaycastHit hitInfo;
            bool hit = Physics.CapsuleCast(
                new Vector3(request.Location.x, request.Location.y + NPCHeight / 2f, request.Location.z),
                new Vector3(request.Location.x, request.Location.y - NPCHeight / 2f, request.Location.z),
                NPCRadius * 1.2f,
                request.Direction,
                out hitInfo,
                request.Distance,
                request.Layer
            );

            // Queue the result back
            physicsResults.Enqueue(new PhysicsResult {
                Hit = hit,
                HitInfo = hitInfo,
                Request = request
            });
        }

        // Process results and invoke callbacks
        while (physicsResults.TryDequeue(out PhysicsResult result)) {
            result.Request.Callback(result);
        }
    }

    public void FindPath(Vector3 start, Vector3 end) {
        Thread pathfindingThread = new Thread(() => FindPathThread(start, end));
        if (activePathfindingThreads.Count < maxPathfindingThreads) {
            pathfindingThread.Start();
            activePathfindingThreads.Add(pathfindingThread);
        } else {
            queuedPathfindingThreads.Enqueue(pathfindingThread);
        }
    }

    private void FindPathThread(Vector3 start, Vector3 end) {
        Debug.Log("pathfinding started in thread");
        isPathfinding = true;

        path = null;
        lookatVector = Vector3.zero;
        openList.Clear();
        closedList.Clear();
        cameFrom.Clear();

        Location startLocation = new Location(start, 0, Heuristic(start, end));
        Location endLocation = new Location(end, 0, 0);

        openList.Add(startLocation);

        while (openList.Count > 0) {
            if (openList.Count > maxTilesAllowedToCheck) {
                isPathfinding = false;
                Action debugTooFar = () => {
                    Debug.Log($"Checked too many tiles, assuming no path possible to get to location {end} from {start} with NPC {NPC}");
                    NPC.moveToRandom();
                };
                QueueFunction(debugTooFar);
                return;
            }

            Location currentLocation = openList[0];
            for (int i = 1; i < openList.Count; i++) {
                if (openList[i].f < currentLocation.f) {
                    currentLocation = openList[i];
                }
            }
            openList.Remove(currentLocation);
            closedList.Add(currentLocation);

            if (Vector3.Distance(currentLocation.vector, end) <= tileSize &&
                !IsBlocked(currentLocation.vector, currentLocation.vector - end, (currentLocation.vector - end).magnitude, layer)) {

                cameFrom[endLocation] = currentLocation;
                currentLocation = endLocation;
                Action setPath = () => {
                    path = ReconstructPath(cameFrom, currentLocation);
                    Debug.Log(path);
                };

                QueueFunction(setPath);

                lookatVector = end;
                foreach (Vector3 direction in directions) {
                    if (!IsBlocked(end, direction, 2f, layer)) {
                        lookatVector += direction;
                    }
                }
                Action debugFoundPath = () => {
                    Debug.Log("Path found!");
                };
                QueueFunction(debugFoundPath);
                isPathfinding = false;
                NPC.ChangeLocationStatus();
                return;
            }

            foreach (Location neighbor in GetValidNeighbors(currentLocation, end, currentLocation.g)) {
                if (closedList.Contains(neighbor)) {
                    continue;
                }

                float tentativeG = currentLocation.g + Vector3.Distance(new Vector3(currentLocation.x, currentLocation.y, currentLocation.z), new Vector3(neighbor.x, neighbor.y, neighbor.z));

                if (!openList.Contains(neighbor)) {
                    openList.Add(neighbor);
                } else if (tentativeG >= neighbor.g) {
                    continue;
                }

                cameFrom[neighbor] = currentLocation;
                neighbor.g = tentativeG;
                neighbor.h = Heuristic(new Vector3(neighbor.x, neighbor.y, neighbor.z), end);
                neighbor.f = neighbor.g + neighbor.h;
            }
        }
        isPathfinding = false;
        Action debugNoPath = () => {
            Debug.Log($"No path could be found from {start} to {end}");
            NPC.moveToRandom();
        };
        QueueFunction(debugNoPath);
    }

    private bool IsBlocked(Vector3 location, Vector3 dir, float distance, int layer) {
        ManualResetEventSlim resetEvent = new ManualResetEventSlim(false);
        bool hitResult = false;

        Action<PhysicsResult> callback = (result) => {
            hitResult = result.Hit;
            resetEvent.Set();
        };

        physicsRequests.Enqueue(new PhysicsRequest {
            Location = location,
            Direction = dir,
            Distance = distance,
            Layer = layer,
            Callback = callback
        });

        resetEvent.Wait();
        Debug.Log("Waiting for physics callback to return...");
        Debug.Log($"Hit result: {hitResult}");
        return hitResult;
    }
    /*private List<Location> GetValidNeighbors(Location currentLocation, Vector3 end, float currentG) {
        validNeighbors.Clear();
        for (int i = 0; i < directions.Count(); i++) {
            if (!IsBlocked(new Vector3(currentLocation.x, currentLocation.y, currentLocation.z), directions[i], tileSize, layer)) {
                Vector3 neighborPos = currentLocation.vector + directions[i].normalized * tileSize;
                float tentativeG = currentG + Vector3.Distance(currentLocation.vector, neighborPos);
                validNeighbors.Add(new Location(neighborPos, tentativeG, Heuristic(neighborPos, end)));
            }
        }
        return validNeighbors;
    }*/

    private List<Location> GetValidNeighbors(Location currentLocation, Vector3 end, float currentG) {
        validNeighbors.Clear();
        int pendingRequests = directions.Length;

        for (int i = 0; i < directions.Length; i++) {
            Vector3 direction = directions[i];
            Vector3 neighborPos = currentLocation.vector + direction.normalized * tileSize;
            float tentativeG = currentG + Vector3.Distance(currentLocation.vector, neighborPos);

            PhysicsRequest request = new PhysicsRequest {
                Location = currentLocation.vector,
                Direction = direction,
                Distance = tileSize,
                Layer = layer,
                Callback = result => {
                    if (!result.Hit) {
                        lock (validNeighbors) {
                            validNeighbors.Add(new Location(neighborPos, tentativeG, Heuristic(neighborPos, end)));
                        }
                    }
                    Interlocked.Decrement(ref pendingRequests);
                }
            };

            physicsRequests.Enqueue(request);
        }

        // Wait until all physics requests are processed
        while (pendingRequests > 0) {
            Thread.Sleep(1); // Or use another method to wait
        }

        return new List<Location>(validNeighbors); // Return a new list to avoid modification during iteration elsewhere
    }



    private List<Location> ReconstructPath(Dictionary<Location, Location> cameFrom, Location current) {
        List<Location> totalPath = new List<Location> { current };
        while (cameFrom.ContainsKey(current)) {
            current = cameFrom[current];
            totalPath.Add(current);
        }
        totalPath.Reverse();
        return totalPath;
    }

    private float Heuristic(Vector3 current, Vector3 end) {
        return (Mathf.Abs(current.x - end.x) + Mathf.Abs(current.z - end.z)) / tileSize;
    }

    public void DestroyThread(Thread pathfindingThread) {
        activePathfindingThreads.Remove(pathfindingThread);
        pathfindingThread.Join();
        pathfindingThread.Abort();
    }

    public List<Location> GetPath() {
        return path;
    }

    public void SetPath(List<Location> paramPath) {
        this.path = paramPath;
    }

    // Other existing methods...
}  
