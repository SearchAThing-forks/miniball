﻿using System.Globalization;
using static System.Math;

namespace SEB;

public static class PointSetUtils
{

    /// <summary>
    /// Generates a random point set given a random number generator.
    /// </summary>
    /// <param name="d">dimension of the points in the point set</param>
    /// <param name="n">number of points</param>
    /// <param name="r">the random number generator</param>
    /// <param name="on_boundary">Normalize length to "almost" 1 (makes it harder for the algorithm)</param>
    /// <returns>a point set with {@code n} {@code d}-dimensional points</returns>
    public static ArrayPointSet RandomPointSet(int d, int n, Random r, bool on_boundary = false)
    {
        var pts = new ArrayPointSet(d, n);

        for (int i = 0; i < n; ++i)
        {
            var len = 0d;

            for (int j = 0; j < d; ++j)
            {
                // Generate coordindates in [-1,1]
                var v = 2d * r.NextDouble() - 1;
                pts.Set(i, j, v);
                len += v * v;
            }

            // Normalize length to "almost" 1 (makes it harder for the algorithm)
            if (on_boundary)
            {
                var Wiggle = 1e-2;
                len = 1d / Sqrt(len) + Wiggle * r.NextDouble();
                for (int j = 0; j < d; ++j) pts.Set(i, j, pts.Coord(i, j) * len);
            }
        }

        return pts;
    }

    /// <summary>
    /// Reads a point set from an input stream.
    /// <br/>
    /// The stream is assumed to be encoded in UTF-8 and should contain integers and double values, all
    /// separated by space or return. The first two numbers must be integers, specifying the number of
    /// points in the point set and the dimension. The following numbers are all doubles and specify
    /// the points by their Euclidean coordinates.
    /// <br/>
    /// For example, the three two-dimensional points (0,0), (2,3), (4,5) could
    /// be stored as follows:
    /// 
    /// <pre>
    /// 3 2
    /// 0 0
    /// 2 3
    /// 4 5
    /// </pre>
    /// </summary>
    /// <param name="s">the input stream to read from</param>
    /// <returns></returns>
    public static ArrayPointSet PointsFromStream(StreamReader s)
    {
        var line = s.ReadLine();
        if (line is null) throw new Exception($"can't find header");
        var ss = line.Trim().Split(' ');
        if (ss.Length != 2) throw new Exception($"invalid header");

        var n = int.Parse(ss[0]);
        var d = int.Parse(ss[1]);
        var pts = new ArrayPointSet(d, n);

        for (int i = 0; i < n; ++i)
        {
            line = s.ReadLine();
            if (line is null) throw new Exception($"can't find {i}-idx row");
            ss = line.Trim().Split(' ');
            if (ss.Length != d) throw new Exception($"expecting {d} dimension instead of {ss.Length} at row {i}-idx");

            for (int j = 0; j < d; ++j)
                pts.Set(i, j, double.Parse(ss[j], CultureInfo.InvariantCulture));
        }

        return pts;
    }

}