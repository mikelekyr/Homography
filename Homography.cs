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

    /// <summary>
    /// Compute output image
    /// </summary>
    public static Bitmap ComputeOutputImage(Bitmap origImage, Matrix<float> H)
    {
        ArgumentNullException.ThrowIfNull(origImage);
        ArgumentNullException.ThrowIfNull(H);

        var Hinv = H.Inverse();

        int width = origImage.Width;
        int height = origImage.Height;

        Bitmap outputImage = new(width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Map output pixel (x, y) back to source (u, v)
                var destPoint = Vector<float>.Build.Dense([x, y, 1]);
                var srcPoint = Hinv.Multiply(destPoint);

                float u = srcPoint[0] / srcPoint[2];
                float v = srcPoint[1] / srcPoint[2];

                //int 

                // Check if (u,v) is inside input image
                if (u >= 0 && v >= 0 && u < width && v < height)
                {
                    // Sample pixel color (nearest neighbor)
                    Color color = origImage.GetPixel((int)u, (int)v);
                    outputImage.SetPixel(x, y, color);
                }
            }
        }

        return outputImage;
    }
}
