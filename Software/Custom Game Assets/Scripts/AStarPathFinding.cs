using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AStarPathfinding;

namespace AStarPathfinding {
    [SerializeField] private float NPCHeight = 1.5f;
    [SerializeField] private float tileSize = 1f;

    class Location {
        public int x;
        public int y;
        public int g; // Cost to get to location (from start) REAL
        public int h; // Cost to get to end (from current location)
        public int f;

        // Constructor with parameters
        public void Location(Vector2Int current, Vector2Int start, Vector2Int end, int gValue) {
            this.x = current.x;
            this.y = current.y;
            this.g = gValue;
            this.h = Mathf.Abs(current.x - end.x) + Mathf.Abs(current.y - end.y);
            this.f = this.g + this.h;
        }
    }

    private void Start() {
        foreach (Location location in findPath(new Vector2Int((int)transform.position.x, (int)transform.position.z), new Vector2Int(0, 0))) {
            Debug.Log(location);
        }
    }

    private void Update() {

    }

    public List<Location> findPath(Vector2Int Start, Vector2Int End) {
        List<Location> path = new List<Location>();
        
        int g = 0;
        Location currentLocation = new Location(Start, Start, End, g); // current location initialized as starting location
        List<Location> validNeighbors = getValidNeighbors(currentLocation, Start, End, g);
        
        while (currentLocation.x != End.x && currentLocation.y != End.y) {
            Location bestLocation = null;
            for (int i = 0; i < (validNeighbors.Count); i++) {
                if (i == 0) { // if it is the first neightbor, just assume its the best one so far
                    bestLocation = validNeighbors[i];
                }
                
                else if (validNeighbors[i].f < bestLocation.f) {
                    bestLocation = validNeighbors[i];
                }
            }   
            path.Add(bestLocation);
            
            currentLocation = bestLocation;
            g++;
            validNeighbors = getValidNeighbors(currentLocation, Start, End, g);
        }
        return path;
    }

    // Get neighboring nodes of a given location
    List<Location> getValidNeighbors(Location location, Location start, Location end, int g) {
        List<Location> validNeighbors = new List<Location>();

        // Define offsets for neighboring locations (up, down, left, right)
        Vector2Int[] directions = {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0)
        };

        // Perform raycasts to check for obstacles and add valid neighbors
        foreach (Vector2Int direction in directions) {
            if (!isBlocked(location, direction, tileSize)) {
                validNeighbors.Add(new Location(location + direction*tileSize, start, end, g));
            }
        }

        return validNeighbors;
    }

    private bool isBlocked(Vector2Int location, Vector2Int dir, float distance) {
        hitInfo = default(RaycastHit);
        bool hit = Physics.Raycast(
            new Vector3(
                location.x, 
                NPCHeight, 
                location.y), 
            new Vector3(dir.x, 0, dir.y), 
            out hitInfo,
            distance);
        return hit;
    }
}