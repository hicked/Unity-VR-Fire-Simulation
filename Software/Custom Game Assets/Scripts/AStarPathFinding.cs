using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public class AStarPathfinding : MonoBehaviour {
    [SerializeField] public float tileSize = 1f; // square meters
    [SerializeField] private LayerMask layer;
    [SerializeField] NPCAI NPC;
    [SerializeField] private int maxTilesAllowedToCheck = 2000;
    private float NPCHeight;
    private float NPCRadius;

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
        NPC = GetComponent<NPCAI>();
        NPCHeight = NPC.NPCHeight;
        NPCRadius = NPC.NPCRadius;
    }
    
    private void Update() {
        if (path != null) {
            for (int i = 1; i < path.Count; i++) {
                Debug.DrawLine(path[i].vector, path[i-1].vector, Color.yellow);
            }
        }
    }

    public IEnumerator FindPathCoroutine(Vector3 start, Vector3 end) { 
        path = null;
        openList.Clear();
        closedList.Clear();
        cameFrom.Clear();

        Location startLocation = new Location(start, 0, Heuristic(start, end));
        Location endLocation = new Location(end, 0, 0);  // End location's heuristic is not needed here

        openList.Add(startLocation);

        Debug.Log("Calculating Path");
        while (openList.Count > 0) {
            // checks if too many tiles have been checked, and simply breaks
            if (openList.Count > maxTilesAllowedToCheck) { 
                Debug.Log($"Checked too many tiles, assuming no path possible to get to location {end} from {start} with NPC {NPC}");
                path = null;
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
                !IsBlocked(currentLocation.vector, currentLocation.vector - end, (currentLocation.vector - end).magnitude)) {

                cameFrom[endLocation] = currentLocation; // Ensure cameFrom is updated
                currentLocation = endLocation;
                path = ReconstructPath(cameFrom, currentLocation); // path is set to the variable, note that the path will have need a getter
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
        path = null;
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
            if (!IsBlocked(new Vector3(currentLocation.x, currentLocation.y, currentLocation.z), directions[i], tileSize)) {
                Vector3 neighborPos = currentLocation.vector + directions[i].normalized * tileSize;
                float tentativeG = currentG + Vector3.Distance(currentLocation.vector, neighborPos);
                validNeighbors.Add(new Location(neighborPos, tentativeG, Heuristic(neighborPos, end)));
            }
        }
        return validNeighbors;
    }

    private bool IsBlocked(Vector3 location, Vector3 dir, float distance) {
        bool hit = Physics.CapsuleCast(
            new Vector3(
                location.x, 
                location.y + NPCHeight/2f,
                location.z), 
            new Vector3(
                location.x, 
                location.y - NPCHeight/2f,
                location.z),
            NPCRadius*1.2f,  // random buffer for safety
            dir,
            distance,
            layer
        );
        return hit;
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