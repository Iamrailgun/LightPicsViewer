using System;
using System.Windows;

namespace lpv
{
  class Vector2D
  {
    public double X { get; set; }
    public double Y { get; set; }

    public Vector2D(double X, double Y)
    {
      this.X = X;
      this.Y = Y;
    }

    public Vector2D(Point point)
    {
      this.X = point.X;
      this.Y = point.Y;
    }

    public Vector2D Add(Vector2D v2d) => new Vector2D(this.X + v2d.X, this.Y + v2d.Y);

    public Vector2D Sub(Vector2D v2d) => new Vector2D(this.X - v2d.X, this.Y - v2d.Y);

    public Vector2D Neg() => new Vector2D(-1 * this.X, -1 * this.Y);

    public static Vector2D operator +(Vector2D v2da, Vector2D v2db) => v2da.Add(v2db);

    public static Vector2D operator -(Vector2D v2da, Vector2D v2db) => v2da.Sub(v2db);

    public static Vector2D operator -(Vector2D v2d) => v2d.Neg();

    public static Vector2D operator *(Vector2D v2d, double d) => new Vector2D(v2d.X * d, v2d.Y * d);

    public static Vector2D operator *(double d, Vector2D v2d) => v2d * d;

    public Vector2D Lerp(Vector2D to, double amount) => this + (to - this) * amount;

    public static Vector2D Lerp(Vector2D from, Vector2D to, double amount) => from.Lerp(to, amount);

    public static bool operator !=(Vector2D v2da, Vector2D v2db)
    {
      return !(v2da.X == v2db.X && v2db.Y == v2db.Y);
    }

    public static bool operator ==(Vector2D v2da, Vector2D v2db)
    {
      return v2da.X == v2db.X && v2db.Y == v2db.Y;
    }

    public override string ToString()
    {
      return $"X:{X} Y:{Y}";
    }

    public override bool Equals(object obj) => obj is Vector2D v2d &&
             X == v2d.X &&
             Y == v2d.Y;

    public override int GetHashCode()
    {
      var hashCode = 1861411795;
      hashCode = hashCode * -1521134295 + X.GetHashCode();
      hashCode = hashCode * -1521134295 + Y.GetHashCode();
      return hashCode;
    }
  }
}
