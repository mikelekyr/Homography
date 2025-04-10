using MathNet.Numerics.LinearAlgebra;

namespace HomographyApp;

public static class Homography
{
    /// <summary>
    /// Calculate
    /// </summary>
    public static Matrix<float> Calculate(List<Vector<float>> src, List<Vector<float>> dst)
    {
        // Build A
        var A = BuildMatrixA(src, dst);

        // Compute SVD
        var svd = A.Svd(computeVectors: true);
        var V = svd.VT.Transpose();
        var h = V.Column(V.ColumnCount - 1);

        // Reshape h into 3x3 matrix
        var H = Matrix<float>.Build.Dense(3, 3);
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
    private static Matrix<float> BuildMatrixA(List<Vector<float>> src, List<Vector<float>> dst)
    {
        if (src.Count != src.Count)
            throw new ArgumentException("Point lists must have the same length.");

        int n = src.Count;
        var A = Matrix<float>.Build.Dense(n * 2, 9);

        for (int i = 0; i < n; i++)
        {
            float X = src[i][0];
            float Y = src[i][1];
            float x = dst[i][0];
            float y = dst[i][1];

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

        return A;
    }
}
