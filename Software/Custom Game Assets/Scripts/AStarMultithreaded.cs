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
    [SerializeField] public static int maxPathfindingThreads = 8;
    private static List<PathfindingRequest> activePathfindingRequests = new List<PathfindingRequest>();
    private static Queue<PathfindingRequest> queuedPathfindingRequests = new Queue<PathfindingRequest>();
    private ConcurrentQueue<PhysicsRequest> physicsRequests = new ConcurrentQueue<PhysicsRequest>();
    private ConcurrentQueue<PhysicsResult> physicsResults = new ConcurrentQueue<PhysicsResult>();

    public class PathfindingRequest {
        public Thread Thread { get; set; }
        public NPCManager NPC { get; set; }
    }

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
    [SerializeField] public LayerMask layer;
    [SerializeField] public NPCManager NPC;
    [SerializeField] private int maxTilesAllowedToCheck = 15000; // note that most is regulated by other scripts like NPCManager
    // This is wost case scenario, it will quit after this many tiles are checked
    private float NPCHeight;
    private float NPCRadius;
    public bool isPathfinding;
    public Vector3 lookatVector;

    private List<Location> openList = new List<Location>();
    private HashSet<Location> closedList = new HashSet<Location>();
    private Dictionary<Location, Location> cameFrom = new Dictionary<Location, Location>();
    private List<Location> validNeighbors = new List<Location>();
    public List<Location> path = null;
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

    bool PerformCapsuleCast(PhysicsRequest request, out RaycastHit hitInfo) {
        return Physics.CapsuleCast(
            new Vector3(request.Location.x, request.Location.y + NPCHeight / 2f, request.Location.z),
            new Vector3(request.Location.x, request.Location.y - NPCHeight / 2f, request.Location.z),
            NPCRadius * 1.2f,
            request.Direction,
            out hitInfo,
            request.Distance,
            request.Layer
        );
    }

    private void Update() {
        runQueuedFunctions();

        List<PathfindingRequest> requestsToRemove = new List<PathfindingRequest>();

        // Process pathfinding threads and manage thread pools
        for (int i = 0; i < activePathfindingRequests.Count; i++) {
            if (!activePathfindingRequests[i].Thread.IsAlive) {
                requestsToRemove.Add(activePathfindingRequests[i]);
                activePathfindingRequests.RemoveAt(i);
            }
        }
        foreach (PathfindingRequest request in requestsToRemove) {
            DestroyThread(request.Thread);
        }

        if (activePathfindingRequests.Count < maxPathfindingThreads && queuedPathfindingRequests.Count > 0) {
            while ((maxPathfindingThreads - activePathfindingRequests.Count) >= 0 && queuedPathfindingRequests.Count != 0) {
                PathfindingRequest requestToStart = queuedPathfindingRequests.Dequeue();
                requestToStart.Thread.Start();
                activePathfindingRequests.Add(requestToStart);
            }
        }

        // Process physics requests
        while (physicsRequests.TryDequeue(out PhysicsRequest request)) {
            RaycastHit hitInfo;
            
            bool hit = PerformCapsuleCast(request, out hitInfo);

            // Check for NPC and Doors layers and handle recasting
            if (hit) {
                bool needsRecast = false;
                int newLayer = request.Layer;

                if (LayerMask.LayerToName(hitInfo.collider.gameObject.layer) == "NPC") {
                    NPCManager NPC = hitInfo.collider.gameObject.GetComponent<NPCManager>();
                    if (NPC.isIdle) {
                        needsRecast = true;
                        newLayer &= ~(1 << LayerMask.NameToLayer("NPC"));
                    }
                }
                else if (LayerMask.LayerToName(hitInfo.collider.gameObject.layer) == "Doors") {
                    Doors door = hitInfo.collider.gameObject.GetComponent<Doors>();
                    DoorHandle handle = door.doorHandle.GetComponent<DoorHandle>();

                    if (!handle.isLocked) {
                        needsRecast = true;
                        newLayer &= ~(1 << LayerMask.NameToLayer("Doors"));
                    }
                }

                if (needsRecast) {
                    hit = PerformCapsuleCast(new PhysicsRequest
                    {
                        Location = request.Location,
                        Direction = request.Direction,
                        Distance = request.Distance,
                        Layer = newLayer,
                        Callback = request.Callback
                    }, out hitInfo);
                }
            }

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
        PathfindingRequest request = new PathfindingRequest { Thread = pathfindingThread, NPC = NPC };
        if (activePathfindingRequests.Count < maxPathfindingThreads) {
            pathfindingThread.Start();
            activePathfindingRequests.Add(request);
        } else {
            queuedPathfindingRequests.Enqueue(request);
        }
    }

    private void FindPathThread(Vector3 start, Vector3 end) {
        //Debug.Log($"Finding path from {start} to {end}");

        if (NPC == null) {
            Debug.LogError("NPC is not initialized.");
            return;
        }

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
                Action debugTooFar = () => {
                    Debug.Log($"Checked too many tiles, assuming no path possible to get to location {end} from {start} with NPC {NPC}");
                    isPathfinding = false;
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
            lock (openList) {
                openList.Remove(currentLocation);
            }
            lock (closedList) {
                closedList.Add(currentLocation);
            }

            if (Vector3.Distance(currentLocation.vector, end) <= tileSize &&
                !IsBlocked(currentLocation.vector, currentLocation.vector - end, (currentLocation.vector - end).magnitude, layer)) {

                if (cameFrom == null) {
                    Debug.LogError("cameFrom dictionary is not initialized.");
                    return;
                }

                cameFrom[endLocation] = currentLocation;
                currentLocation = endLocation;

                lookatVector = Vector3.zero;
                foreach (Vector3 direction in directions) {
                    if (!IsBlocked(end, direction, 2f, layer)) {
                        lookatVector += direction;
                    }
                }
                
                NPC.ChangeLocationStatus();

                Action setPathAndLookat = () => {
                    isPathfinding = false;
                    NPC.path.AddRange(ReconstructPath(cameFrom, currentLocation));
                    
                    if (lookatVector == Vector3.zero) {
                        NPC.lookatVector = NPC.gameObject.transform.forward;
                    }
                    else {
                        NPC.lookatVector = lookatVector;
                    }

                    if (NPC.path == null) {
                        Debug.LogError("Reconstructed path is null.");
                    }
                };

                QueueFunction(setPathAndLookat);
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
        
        Action debugNoPath = () => {
            Debug.Log($"No path could be found from {start} to {end}");
            isPathfinding = false;
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
        return hitResult;
    }

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
        pathfindingThread.Join();
        pathfindingThread.Abort();
    }

    public List<Location> GetPath() {
        return path;
    }

    public void SetPath(List<Location> paramPath) {
        this.path = paramPath;
    }
}