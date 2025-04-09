using MathNet.Numerics.LinearAlgebra;

namespace HomographyApp;

public static class HomographyDLT
{
    public static Matrix<double> ComputeHomography(List<Vector<double>> srcPoints, List<Vector<double>> dstPoints)
    {
        if (srcPoints.Count != dstPoints.Count || srcPoints.Count < 4)
            throw new ArgumentException("Need at least 4 pairs of points.");

        // 1. Normalize points
        var (srcNorm, T_src) = NormalizePoints(srcPoints);
        var (dstNorm, T_dst) = NormalizePoints(dstPoints);

        // 2. Build matrix A
        var A = BuildMatrixA(srcNorm, dstNorm);

        // 3. Solve A * h = 0 using SVD
        var svd = A.Svd(true);
        var h = svd.VT.Transpose().Column(A.ColumnCount - 1);

        var Hnorm = Matrix<double>.Build.DenseOfColumnMajor(3, 3, [.. h]);

        // 4. Denormalize
        var H = T_dst.Inverse() * Hnorm * T_src;

        // Normalize so that H[2,2] = 1
        return H.Divide(H[2, 2]);
    }

    private static (List<Vector<double>>, Matrix<double>) NormalizePoints(List<Vector<double>> points)
    {
        double meanX = 0, meanY = 0;
        foreach (var p in points)
        {
            meanX += p[0];
            meanY += p[1];
        }
        meanX /= points.Count;
        meanY /= points.Count;

        double meanDist = 0;
        foreach (var p in points)
        {
            meanDist += Math.Sqrt(Math.Pow(p[0] - meanX, 2) + Math.Pow(p[1] - meanY, 2));
        }
        meanDist /= points.Count;

        double scale = Math.Sqrt(2) / meanDist;

        var T = Matrix<double>.Build.DenseOfArray(new double[,]
        {
            { scale, 0, -scale * meanX },
            { 0, scale, -scale * meanY },
            { 0, 0, 1 }
        });

        var normalized = new List<Vector<double>>();
        foreach (var p in points)
        {
            var pH = Vector<double>.Build.Dense([p[0], p[1], 1]);
            var pNorm = T * pH;
            normalized.Add(pNorm.SubVector(0, 2));
        }

        return (normalized, T);
    }

    private static Matrix<double> BuildMatrixA(List<Vector<double>> src, List<Vector<double>> dst)
    {
        var A = Matrix<double>.Build.Dense(src.Count * 2, 9);
        for (int i = 0; i < src.Count; i++)
        {
            double x = src[i][0], y = src[i][1];
            double u = dst[i][0], v = dst[i][1];

            A.SetRow(i * 2, [-x, -y, -1, 0, 0, 0, x * u, y * u, u]);
            A.SetRow(i * 2 + 1, [0, 0, 0, -x, -y, -1, x * v, y * v, v]);
        }
        return A;
    }
}
