using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour {
    [SerializeField] private float NPCHeight = 0.2f;
    [SerializeField] private static float tileSize = 1f; // square meters
    [SerializeField] private float buffer = 3f;
    private RaycastHit hitInfo;

    private void Start() {
        List<Location> qwerty = findPath(transform.position, new Vector3(3.5f, 0, -20f));

        Debug.Log(qwerty.Count);
        foreach (Location location in qwerty) {
            Debug.Log(new Vector3(location.x, location.y, location.z));
        }
    }

    private void Update() {

    }

    public List<Location> findPath(Vector3 Start, Vector3 End) {
        List<Location> path = new List<Location>();
        
        Location start = new Location(Start, Start, End);
        Location end = new Location(End, Start, End);

        Location currentLocation = start; // current location initialized as starting location
        List<Location> validNeighbors = getValidNeighbors(currentLocation, start, end);
        int bla = 0;
        while (Vector3.Distance(new Vector3(currentLocation.x, currentLocation.y, currentLocation.z), End) > tileSize) {
            bla++;
            if (bla > 50) {
                break;
            }
            if (validNeighbors.Count == 0) {
                // No valid path found, terminate the loop
                break;
            }

            Location bestLocation = null;
            for (int i = 0; i < (validNeighbors.Count); i++) {
                if (i == 0) { // if it is the first neightbor, just assume its the best one so far
                    bestLocation = validNeighbors[0];
                }
                
                else if (validNeighbors[i].f < bestLocation.f) {
                    bestLocation = validNeighbors[i];

                    Debug.Log(bestLocation.h);
                }
            }
            path.Add(bestLocation);
            
            currentLocation = bestLocation;
            validNeighbors = getValidNeighbors(currentLocation, start, end);
        }
        return path;
    }

    // Get neighboring nodes of a given location
    List<Location> getValidNeighbors(Location location, Location start, Location end) {
        List<Location> validNeighbors = new List<Location>();

        // Define offsets for neighboring locations (up, down, left, right)
        Vector3[] directions = {
            new Vector3(1f, 0f, 0f),
            new Vector3(-1f, 0f, 0f),            
            new Vector3(0f, 0f, 1f),
            new Vector3(0f, 0f, -1f)
            };

        // Perform raycasts to check for obstacles and add valid neighbors
        foreach (Vector3 direction in directions) {
            if (!isBlocked(new Vector3(location.x, location.y, location.z), direction, tileSize)) {
                validNeighbors.Add(new Location(new Vector3(location.x, location.y, location.z) + direction*tileSize, new Vector3(start.x, start.y, start.z), new Vector3(end.x, end.y, end.z)));
            }
        }

        return validNeighbors;
    }

    private bool isBlocked(Vector3 location, Vector3 dir, float distance) {
        hitInfo = default(RaycastHit);
        bool hit = Physics.Raycast(
            new Vector3(
                location.x, 
                location.y + NPCHeight, 
                location.z), 
            dir, 
            out hitInfo,
            distance);
        return hit;
    }

    public class Location {
        public float x;
        public float y;
        public float z;
        public float g; // Cost to get to location (from start) REAL
        public float h; // Cost to get to end (from current location)
        public float f;

        // Constructor with parameters
        public Location(Vector3 current, Vector3 start, Vector3 end) {
            this.x = current.x;
            this.y = current.y;
            this.z = current.z;
            this.g = (Mathf.Abs(current.x - start.x) + Mathf.Abs(current.z - start.z))/tileSize; // sub this for a passed value of g
            this.h = (Mathf.Abs(current.x - end.x) + Mathf.Abs(current.z - end.z))/tileSize;
            this.f = this.g + this.h;
        }
    }
}