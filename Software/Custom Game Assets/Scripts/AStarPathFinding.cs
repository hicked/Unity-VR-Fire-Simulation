using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AStarPathfinding;

namespace AStarPathfinding {

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
            this.parent;
        }
    }

    public void findPath(Vector2Int Start, Vector2Int End) {
        List<Location> openList = new List<Location>();
        List<Location> closedList = new List<Location>();
        
        int g = 0;
        Location start = Location(Start, Start, End, g);
        
        List<Location> neighbors = getNeighbors(start);
        List<Location> path = new List<Location>();
        
        foreach (Location neighbor in neightbors) {
            openList.Add(neighbor);
        }
        
        Location currentLocation = start;
        while (currentLocation.x != End.x && currentLocation.y != End.y) {
            
            while (openList.Count > 0) {
                // Find the node with the lowest F score in the open list
                Location bestNode = openList[0];
                foreach (Location node in openList) {
                    if (node.f < current.f) {
                        current = location;
                    }
                }

                // Move the current node from open list to closed list
                openList.Remove(current);
                closedList.Add(current);
    }

    // Get neighboring nodes of a given location
    List<Location> getNeighbors(Location location) {
        List<Location> neighbors = new List<Location>();

        // Define offsets for neighboring locations (up, down, left, right)
        Vector2Int[] offsets = {
            new Vector2Int(0, 1),   // Up
            new Vector2Int(0, -1),  // Down
            new Vector2Int(-1, 0),  // Left
            new Vector2Int(1, 0)    // Right
        };

        // Perform raycasts to check for obstacles and add valid neighbors
        foreach (Vector2Int offset in offsets) {
            Vector2Int neighborPos = new Vector2Int(location.x + offset.x, location.y + offset.y);
            
            // Perform raycast to check for obstacles
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(location.x, location.y), offset, 1f);
            
            // If no obstacle found, add neighbor to list
            if (hit.collider == null) {
                neighbors.Add(grid[neighborPos.x, neighborPos.y]);
            }
        }

        return neighbors;
    }
}