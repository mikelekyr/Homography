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
    /// MatrixFWithPointOffset
    /// </summary>
    public Vector<float> MatrixFWithPointOffset(Vector<float> point, Point offset)
    {
        float ratioX = (Umax - Umin) / (Xmax - Xmin);
        float ratioY = (Vmax - Vmin) / (Ymax - Ymin);

        Vector<float> v = Vector<float>.Build.Dense(3);

        v[0] = point[0] + (offset.X / ratioX);
        v[1] = point[1] + (offset.Y / ratioY);
        v[2] = 1f;

        return v;
    }

    /// <summary>
    /// Calculate angle between points
    /// </summary>
    public static float GetAngleBetweenPoints(Vector<float> refPoint, Vector<float> endPoint)
    {
        float dY = endPoint[1] - refPoint[1];
        float dX = endPoint[0] - refPoint[0];

        return (float)Math.Atan2(dY, dX);
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

    /// <summary>
    /// Get distance between points
    /// </summary>
    public static float GetDistanceBetweenPoints(Vector<float> refPoint, Vector<float> endPoint)
    {
        float dY = endPoint[1] - refPoint[1];
        float dX = endPoint[0] - refPoint[0];

        return (float)Math.Sqrt((dX * dX) + (dY * dY));
    }

    /// <summary>
    /// Rotate point upon another point
    /// </summary>
    public static Vector<float> RotatePointAroundPoint(Vector<float> refPoint, Vector<float> endPoint, float angle)
    {
        float shiftX = refPoint[0];
        float shiftY = refPoint[1];

        var matrixNegativeTranslate = BuildTranslationMatrix(-shiftX, -shiftY);
        var matrixRotate = BuildRotationMatrix(angle);
        var matrixPositiveTranslate = BuildTranslationMatrix(shiftX, shiftY);

        var matrixTransform = matrixPositiveTranslate * matrixRotate * matrixNegativeTranslate;

        return matrixTransform * endPoint;
    }

    /// <summary>
    /// Linked node rotation
    /// </summary>
    public static Vector<float> LinkedNodeRotation(Vector<float> refPoint, float radius, float theta)
    {
        float origX = refPoint[0];
        float origY = refPoint[1];

        float newX = radius * (float)Math.Cos(theta);
        float newY = radius * (float)Math.Sin(theta);

        Vector<float> v = Vector<float>.Build.Dense(3);

        v[0] = newX + origX;
        v[1] = newY + origY;
        v[2] = 1f;

        return v;
    }

    /// <summary>
    /// BuildTranslationMatrix
    /// </summary>
    public static Matrix<float> BuildTranslationMatrix(float x, float y)
    {
        var matrix = Matrix<float>.Build.DenseIdentity(3);

        matrix[0, 0] = 1f;
        matrix[0, 1] = 0f;
        matrix[0, 2] = x;

        matrix[1, 0] = 0f;
        matrix[1, 1] = 1f;
        matrix[1, 2] = y;

        matrix[2, 0] = 0f;
        matrix[2, 1] = 0f;
        matrix[2, 2] = 1f;

        return matrix;
    }

    /// <summary>
    /// Create a rotation matrix that rotates values around point (0,0)
    /// </summary>
    public static Matrix<float> BuildRotationMatrix(float angle)
    {
        var matrix = Matrix<float>.Build.DenseIdentity(3);

        matrix[0, 0] = (float)Math.Cos(angle);
        matrix[0, 1] = (float)-Math.Sin(angle);
        matrix[0, 2] = 0f;

        matrix[1, 0] = (float)Math.Sin(angle);
        matrix[1, 1] = (float)Math.Cos(angle);
        matrix[1, 2] = 0f;

        matrix[2, 0] = 0f;
        matrix[2, 1] = 0f;
        matrix[2, 2] = 1f;

        return matrix;
    }

    /// <summary>
    /// BuildScalingMatrix
    /// </summary>
    public static Matrix<float> BuildScalingMatrix(float xScale, float yScale)
    {
        var matrix = Matrix<float>.Build.DenseIdentity(3);

        matrix[0, 0] = xScale;
        matrix[0, 1] = 0f;
        matrix[0, 2] = 0f;

        matrix[1, 0] = 0f;
        matrix[1, 1] = yScale;
        matrix[1, 2] = 0f;

        matrix[2, 0] = 0f;
        matrix[2, 1] = 0f;
        matrix[2, 2] = 1f;

        return matrix;
    }
}