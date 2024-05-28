using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AStarPathfinding : MonoBehaviour {
    [SerializeField] private static float tileSize = 1f; // square meters
    [SerializeField] private LayerMask layer;
    private CapsuleCollider NPCHitbox;
    private RaycastHit hitInfo;

    private void Start() {
        NPCHitbox = GetComponent<CapsuleCollider>();
    }

    public List<Location> FindPath(Vector3 start, Vector3 end) {
        List<Location> openList = new List<Location>();
        HashSet<Location> closedList = new HashSet<Location>();
        Dictionary<Location, Location> cameFrom = new Dictionary<Location, Location>();

        Location startLocation = new Location(start, 0, Heuristic(start, end));
        Location endLocation = new Location(end, 0, 0);  // End location's heuristic is not needed here

        openList.Add(startLocation);

        while (openList.Count > 0) {
            Location currentLocation = openList[0];
            foreach (Location location in openList) {
                if (location.f < currentLocation.f) {
                    currentLocation = location;
                }
            }

            if (Vector3.Distance(new Vector3(currentLocation.x, currentLocation.y, currentLocation.z), end) <= tileSize) {
                cameFrom[endLocation] = currentLocation; // Ensure cameFrom is updated
                currentLocation = endLocation;

                return ReconstructPath(cameFrom, currentLocation);
            }

            openList.Remove(currentLocation);
            closedList.Add(currentLocation);

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

        return new List<Location>(); // No path found
    }

    List<Location> GetValidNeighbors(Location location, Vector3 end, float currentG) {
        List<Location> validNeighbors = new List<Location>();

        Vector3[] directions = {
            new Vector3(1f, 0f, 0f),
            new Vector3(-1f, 0f, 0f),
            new Vector3(0f, 0f, 1f),
            new Vector3(0f, 0f, -1f)
            // new Vector3(1f, 0f, 1f),
            // new Vector3(-1f, 0f, 1f),
            // new Vector3(-1f, 0f, -1f),
            // new Vector3(1f, 0f, -1f)
        };

        foreach (Vector3 direction in directions) {
            Vector3 neighborPos = new Vector3(location.x, location.y, location.z) + direction * tileSize;
            if (!IsBlocked(new Vector3(location.x, location.y, location.z), direction, tileSize)) {
                float tentativeG = currentG + Vector3.Distance(new Vector3(location.x, location.y, location.z), neighborPos);
                validNeighbors.Add(new Location(neighborPos, tentativeG, Heuristic(neighborPos, end)));
            }
        }

        return validNeighbors;
    }

    private bool IsBlocked(Vector3 location, Vector3 dir, float distance) {
        hitInfo = default(RaycastHit);
        bool hit = Physics.CapsuleCast(
            new Vector3(
                location.x, 
                location.y + NPCHitbox.height/2f,
                location.z), 
            new Vector3(
                location.x, 
                location.y - NPCHitbox.height/2f,
                location.z),
            NPCHitbox.radius*1.15f, 
            dir, 
            out hitInfo, 
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