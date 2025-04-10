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
        var A = Matrix<float>.Build.Dense(src.Count * 2, 9);
        for (int i = 0; i < src.Count; i++)
        {
            float x = src[i][0], y = src[i][1];
            float u = dst[i][0], v = dst[i][1];

            A.SetRow(i * 2, [-x, -y, -1, 0, 0, 0, x * u, y * u, u]);
            A.SetRow(i * 2 + 1, [0, 0, 0, -x, -y, -1, x * v, y * v, v]);
        }
        return A;
    }
}
