using UnityEngine;
using System;

public class Location : IEquatable<Location> {
    public float x, y, z, g, h, f;
    public Vector3 vector;

    public Location(Vector3 position, float g, float h) {
        this.x = position.x;
        this.y = position.y;
        this.z = position.z;
        this.g = g;
        this.h = h;
        this.f = g + h;
        this.vector = new Vector3(x,y,z);
    }

    public bool Equals(Location other) {
        return this.x == other.x && this.y == other.y && this.z == other.z;
    }

    public override int GetHashCode() {
        return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
    }
}
