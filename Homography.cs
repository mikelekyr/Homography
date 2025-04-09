using MathNet.Numerics.LinearAlgebra;
using System.Diagnostics;

namespace HomographyApp;

public static class Homography
{
    public static Matrix<double> Calculate(PointF[] sourcePoints, PointF[] targetPoints)
    {
        // Build A
        var A = BuildMatrixA(sourcePoints, targetPoints);

        // Compute SVD
        var svd = A.Svd(computeVectors: true);
        var V = svd.VT.Transpose();
        var h = V.Column(V.ColumnCount - 1);

        // Reshape h into 3x3 matrix
        var H = Matrix<double>.Build.Dense(3, 3);
        for (int i = 0; i < 9; i++)
        {
            H[i / 3, i % 3] = h[i];
        }

        // Normalize
        H /= H[2, 2];

        return H;
    }

    /// <summary>
    /// BuildMatrixA
    /// </summary>
    private static Matrix<double> BuildMatrixA(PointF[] sourcePoints, PointF[] targetPoints)
    {
        if (sourcePoints.Length != targetPoints.Length)
            throw new ArgumentException("Point lists must have the same length.");

        int n = sourcePoints.Length;
        var A = Matrix<double>.Build.Dense(n * 2, 9);

        for (int i = 0; i < n; i++)
        {
            double X = sourcePoints[i].X;
            double Y = sourcePoints[i].Y;
            double x = targetPoints[i].X;
            double y = targetPoints[i].Y;

            // First row for this correspondence
            A.SetRow(2 * i,
            [
                -X, -Y, -1, 0, 0, 0, x * X, x * Y, x
            ]);

            // Second row for this correspondence
            A.SetRow(2 * i + 1,
            [
                0, 0, 0, -X, -Y, -1, y * X, y * Y, y
            ]);
        }

        Debug.WriteLine(A.ToMatrixString());

        return A;
    }
}
