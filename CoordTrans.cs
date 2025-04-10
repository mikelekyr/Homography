using MathNet.Numerics.LinearAlgebra;

namespace HomographyApp;

public class CoordTrans
{
    public float Xmin { get; set; } = -50f;
    public float Xmax { get; set; } = 50f;
    public float Ymin { get; set; } = -50f;
    public float Ymax { get; set; } = 50f;

    public float Umin { get; set; } = 0f;
    public float Umax { get; set; } = 128;
    public float Vmin { get; set; } = 96;
    public float Vmax { get; set; } = 0;

    public float XRange { get { return Math.Abs(Xmax - Xmin); } }
    public float YRange { get { return Math.Abs(Ymax - Ymin); } }
    public float URange { get { return Math.Abs(Umax - Umin); } }
    public float VRange { get { return Math.Abs(Vmax - Vmin); } }

    /// <summary>
    /// FromUVtoXY
    /// </summary>
    public PointF FromUVtoXY(Point p)
    {
        return new PointF((p.X - Umin) / (float)(Umax - Umin) * (Xmax - Xmin) + Xmin,
                          (p.Y - Vmin) / (float)(Vmax - Vmin) * (Ymax - Ymin) + Ymin);
    }

    /// <summary>
    /// FromXYtoUV
    /// </summary>
    public Point FromXYtoUV(PointF p)
    {
        return new Point((int)((p.X - Xmin) / (Xmax - Xmin) * (Umax - Umin) + Umin),
                         (int)((p.Y - Ymin) / (Ymax - Ymin) * (Vmax - Vmin) + Vmin));
    }

    /// <summary>
    /// FromXYtoUVF
    /// </summary>
    public PointF FromXYtoUVF(PointF p)
    {
        return new PointF((p.X - Xmin) / (Xmax - Xmin) * (Umax - Umin) + Umin,
                         (p.Y - Ymin) / (Ymax - Ymin) * (Vmax - Vmin) + Vmin);
    }

    /// <summary>
    /// FromUVtoXYVector
    /// </summary>
    public Vector<float> FromUVtoXYVectorFloat(Point p)
    {
        Vector<float> v = Vector<float>.Build.Dense(3);

        v[0] = (p.X - Umin) / (float)(Umax - Umin) * (Xmax - Xmin) + Xmin;
        v[1] = (p.Y - Vmin) / (float)(Vmax - Vmin) * (Ymax - Ymin) + Ymin;
        v[2] = 1f;

        return v;
    }

    /// <summary>
    /// FromXYtoUVF
    /// </summary>
    public Point FromXYVectorFtoUV(Vector<float> p)
    {
        return new Point((int)((p[0] - Xmin) / (Xmax - Xmin) * (Umax - Umin) + Umin),
                         (int)((p[1] - Ymin) / (Ymax - Ymin) * (Vmax - Vmin) + Vmin));
    }

    /// <summary>
    /// GetUVF
    /// </summary>
    public PointF GetUVF(Vector<float> v)
    {
        if (v == null || v.Count != 3)
            throw new ApplicationException($"Wrong vertex data input!");


        return new PointF(((float)v[0] - Xmin) / (Xmax - Xmin) * (Umax - Umin) + Umin,
                          ((float)v[1] - Ymin) / (Ymax - Ymin) * (Vmax - Vmin) + Vmin);
    }

    /// <summary>
    /// GetUV
    /// </summary>
    public Point GetUV(Vector<float> v)
    {
        PointF p = GetUVF(v);
        return new Point((int)Math.Round(p.X), (int)Math.Round(p.Y));
    }

    /// <summary>
    /// Calculates distance between two points
    /// </summary>
    public static float CalculateDistanceBetweenPoints(Vector<float> point1, Vector<float> point2)
    {
        float deltaX = Math.Abs(point1[0] - point2[0]);
        float deltaY = Math.Abs(point1[1] - point2[1]);

        return (float)Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
    }
}