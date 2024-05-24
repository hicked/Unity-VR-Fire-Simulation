using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AStarPathfinding;

namespace AStarPathfinding {
    [SerializeField] private float NPCHeight = 0.2f;
    [SerializeField] private float tileSize = 1f;

    class Location {
        public float x;
        public float y;
        public float z;
        public float g; // Cost to get to location (from start) REAL
        public float h; // Cost to get to end (from current location)
        public float f;

        // Constructor with parameters
        public void Location(Vector3 current, Vector3 start, Vector3 end) {
            this.x = current.x;
            this.y = current.y;
            this.z = current.z;
            this.g = Mathf.Abs(current.x - start.x) + Mathf.Abs(current.z - start.z);
            this.h = Mathf.Abs(current.x - end.x) + Mathf.Abs(current.z - end.z);
            this.f = this.g + this.h;
        }
    }

    private void Start() {
        foreach (Location location in findPath(transform.position, new Vector3(0, 0, 0))) {
            Debug.Log(location);
        }
    }

    private void Update() {

    }

    public List<Location> findPath(Vector3 Start, Vector3 End) {
        List<Location> path = new List<Location>();
        
        Location currentLocation = new Location(Start, Start, End); // current location initialized as starting location
        List<Location> validNeighbors = getValidNeighbors(currentLocation, Start, End);
        
        while ((int)currentLocation.x != (int)End.x && (int)currentLocation.y != (int)End.y && (int)currentLocation.z != (int)End.z) {
            Location bestLocation = null;
            for (int i = 0; i < (validNeighbors.Count); i++) {
                if (i == 0) { // if it is the first neightbor, just assume its the best one so far
                    bestLocation = validNeighbors[0];
                }
                
                else if (validNeighbors[i].f < bestLocation.f) {
                    bestLocation = validNeighbors[i];
                }
            }   
            path.Add(bestLocation);
            
            currentLocation = bestLocation;
            validNeighbors = getValidNeighbors(currentLocation, Start, End);
        }
        return path;
    }

    // Get neighboring nodes of a given location
    List<Location> getValidNeighbors(Location location, Location start, Location end) {
        List<Location> validNeighbors = new List<Location>();

        // Define offsets for neighboring locations (up, down, left, right)
        Vector2Int[] directions = {
            new Vector3(1, 0, 0),
            new Vector3(-1, 0, 0),            
            new Vector3(0, 0, 1),
            new Vector3(0, 0, -1)
            };

        // Perform raycasts to check for obstacles and add valid neighbors
        foreach (Vector3 direction in directions) {
            if (!isBlocked(location, direction, tileSize)) {
                validNeighbors.Add(new Location(location + direction*tileSize, start, end));
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
}