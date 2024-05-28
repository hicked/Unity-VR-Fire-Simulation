using UnityEngine;

public class Location {
    public float x;
    public float y;
    public float z;
    public float g; // Cost from start to this node
    public float h; // Heuristic cost from this node to the end
    public float f; // Total cost (g + h)

    public Location(Vector3 current, float g, float h) {
        this.x = current.x;
        this.y = current.y;
        this.z = current.z;
        this.g = g;
        this.h = h;
        this.f = g + h;
    }

    public override bool Equals(object obj) {
        return obj is Location location &&
                x == location.x &&
                y == location.y &&
                z == location.z;
    }

    public override int GetHashCode() {
        return new Vector3(x, y, z).GetHashCode();
    }
}
