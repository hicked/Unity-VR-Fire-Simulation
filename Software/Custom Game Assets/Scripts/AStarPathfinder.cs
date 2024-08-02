using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading;
using System;


public class AStarPathfinder : Threadable {
    [SerializeField] public float tileSize = 1f; // square meters
    [SerializeField] private LayerMask layer;
    [SerializeField] NPCManager NPC;
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
    Vector3[] directions = {
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
        if (path != null) {
            for (int i = 1; i < path.Count; i++) {
                Debug.DrawLine(path[i].vector + new Vector3(0, 0.5f, 0), path[i-1].vector + new Vector3(0, 0.5f, 0), Color.yellow);
            }
        }
    }
        
    public IEnumerator FindPathCoroutine(Vector3 start, Vector3 end) {
        isPathfinding = true;

        path = null;
        lookatVector = Vector3.zero;
        openList.Clear();
        closedList.Clear();
        cameFrom.Clear();

        Location startLocation = new Location(start, 0, Heuristic(start, end));
        Location endLocation = new Location(end, 0, 0);  // End location's heuristic is not needed here

        openList.Add(startLocation);

        while (openList.Count > 0) {
            // checks if too many tiles have been checked, and simply breaks
            if (openList.Count > maxTilesAllowedToCheck) { 
                Debug.Log($"Checked too many tiles, assuming no path possible to get to location {end} from {start} with NPC {NPC}");
                isPathfinding = false;
                NPC.moveToRandom(); // restart pathfinding
                yield break;
            }

            // sets random initial location, before checking all open/candidate tiles and finding the best one
            Location currentLocation = openList[0]; 
            for (int i = 1; i < openList.Count; i++) {
                if (openList[i].f < currentLocation.f) {
                    currentLocation = openList[i]; // makes the best location the current location
                }
            }
            // removes the location since it has been checked
            openList.Remove(currentLocation); 
            closedList.Add(currentLocation);


            // if its close enough to the end location and there isnt a wall in the way, simply make the final location = end location
            if (Vector3.Distance(currentLocation.vector, end) <= tileSize && 
                !IsBlocked(currentLocation.vector, currentLocation.vector - end, (currentLocation.vector - end).magnitude, layer)) {

                cameFrom[endLocation] = currentLocation; // Ensure cameFrom is updated
                currentLocation = endLocation;
                path = ReconstructPath(cameFrom, currentLocation); // path is set to the variable, note that the path will have need a getter

                lookatVector = end;
                foreach (Vector3 direction in directions) {
                    RaycastHit hitInfo;
                    if (!PerformCapsuleCast(end, direction, 2f, layer, out hitInfo)) {
                        lookatVector += direction;
                    }
                }
                
                isPathfinding = false;

                // NOTE: This part is optional if location are to be stored in a JSON file,
                // set the location as taken if you have multiple NPCs and a list of NPC locations
                NPC.ChangeLocationStatus(); // Basically sets the previous location as available, and new current location as taken/occupied
                // We only want to do this if the pathfinding succeeds
                yield break;
            }

            // checks locations nearby for other tile candidates that will be added to the open list
            foreach (Location neighbor in GetValidNeighbors(currentLocation, end, currentLocation.g)) {
                if (closedList.Contains(neighbor)) {
                    continue;
                }

                float tentativeG = currentLocation.g + Vector3.Distance(new Vector3(currentLocation.x, currentLocation.y, currentLocation.z), new Vector3(neighbor.x, neighbor.y, neighbor.z));

                if (!openList.Contains(neighbor)) {
                    openList.Add(neighbor);
                }
                else if (tentativeG >= neighbor.g) {
                    continue;
                }

                cameFrom[neighbor] = currentLocation;
                neighbor.g = tentativeG;
                neighbor.h = Heuristic(new Vector3(neighbor.x, neighbor.y, neighbor.z), end);
                neighbor.f = neighbor.g + neighbor.h;
            }

            // checks one BEST location and adds neighbors to open list every FRAME to improve performance
            yield return null;
            // NOTE: Can't use yield return new WaitUntilEndOfFrame because the update functions in this class isn't being used

        }
        // If it breaks from the while loop aka, checked too many tiles or out of open tiles to check
        Debug.Log($"No path could be found from {start} to {end}");
        isPathfinding = false;
        NPC.moveToRandom(); // restart pathfinding
        yield break;
    }

    public List<Location> GetPath() {
        return path;
    }
    public void SetPath(List<Location> paramPath) {
        this.path = paramPath;
    }
  
    List<Location> GetValidNeighbors(Location currentLocation, Vector3 end, float currentG) {
        validNeighbors.Clear();
        for(int i = 0; i < directions.Count(); i++) {      
            if (!IsBlocked(new Vector3(currentLocation.x, currentLocation.y, currentLocation.z), directions[i], tileSize, layer)) {
                Vector3 neighborPos = currentLocation.vector + directions[i].normalized * tileSize;
                float tentativeG = currentG + Vector3.Distance(currentLocation.vector, neighborPos);
                validNeighbors.Add(new Location(neighborPos, tentativeG, Heuristic(neighborPos, end)));
            }
        }
        return validNeighbors;
    }

    public bool PerformCapsuleCast(Vector3 location, Vector3 dir, float distance, int layerMask, out RaycastHit hitInfo) {
        return Physics.CapsuleCast(
            new Vector3(location.x, location.y + NPCHeight / 2f, location.z),
            new Vector3(location.x, location.y - NPCHeight / 2f, location.z),
            NPCRadius * 1.2f,
            dir,
            out hitInfo,
            distance,
            layerMask
        );
    }

    private bool IsBlocked(Vector3 location, Vector3 dir, float distance, int layer) {
        RaycastHit hitInfo;

        // Perform capsule cast
        bool hit = PerformCapsuleCast(location, dir, distance, layer, out hitInfo);

        // Check if we hit an NPC
        if (hit && LayerMask.LayerToName(hitInfo.collider.gameObject.layer) == "NPC") {
            NPCManager NPC = hitInfo.collider.gameObject.GetComponent<NPCManager>();
            if (NPC.isIdle) {
                // NPC is pathfinding, perform capsule cast again excluding the NPC layer
                int layerWithoutNPC = layer;
                layerWithoutNPC &= ~(1 << LayerMask.NameToLayer("NPC"));
                return IsBlocked(location, dir, distance, layerWithoutNPC);
            }
        }

        // Check if we hit a door
        if (hit && LayerMask.LayerToName(hitInfo.collider.gameObject.layer) == "Doors") {
            Doors door = hitInfo.collider.gameObject.GetComponent<Doors>();
            DoorHandle handle = door.doorHandle.GetComponent<DoorHandle>();
            if (!handle.isLocked) {
                int layerWithoutDoors = layer;
                layerWithoutDoors &= ~(1 << LayerMask.NameToLayer("Doors"));
                return IsBlocked(location, dir, distance, layerWithoutDoors);
            }
        }

        return hit; // Return the result of the capsule cast
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
}