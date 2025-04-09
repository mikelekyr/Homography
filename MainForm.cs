using MathNet.Numerics.LinearAlgebra;

namespace HomographyApp;

public partial class MainForm : Form
{
    enum PointsInsert
    {
        original,
        reference,
    }

    private PointsInsert pointsInsertState = PointsInsert.original;

    private Bitmap? bitmapOrig;
    private Bitmap? bitmapRef;
    private Bitmap? bitmapFinal;

    private readonly Point[] pointsOrig =
    [
        new (0,0),
        new (1,0),
        new (1,1),
        new (0,1),
    ];

    private readonly Point[] pointsRef =
    [
        new (1,1),
        new (3,1),
        new (3,4),
        new (1,4),
    ];

    private readonly Point[] pointsFinal = new Point[4];

    private readonly PointF[] pointsOrigTransformed = new PointF[4];
    private readonly PointF[] pointsRefTransformed = new PointF[4];
    private readonly PointF[] pointsFinalTransformed = new PointF[4];

    private int currentIndexOrig = 0;
    private int currentIndexRef = 0;

    private readonly CoordTrans ctOrig = new();
    private readonly CoordTrans ctRef = new();
    private readonly CoordTrans ctFinal = new();

    public MainForm()
    {
        InitializeComponent();

        // coord trans original
        ctOrig.Xmin = 0f;
        ctOrig.Xmax = 7f;
        ctOrig.Ymin = 0f;
        ctOrig.Ymax = 7f;

        ctOrig.Umin = 0;
        ctOrig.Umax = 640;
        ctOrig.Vmin = 480;
        ctOrig.Vmax = 0;

        // coord trans reference
        ctRef.Xmin = 0f;
        ctRef.Xmax = 7f;
        ctRef.Ymin = 0f;
        ctRef.Ymax = 7f;

        ctRef.Umin = 640;
        ctRef.Umax = 1280;
        ctRef.Vmin = 480;
        ctRef.Vmax = 0;

        // coord trans final
        ctFinal.Xmin = 0f;
        ctFinal.Xmax = 7f;
        ctFinal.Ymin = 0f;
        ctFinal.Ymax = 7f;

        ctFinal.Umin = 0;
        ctFinal.Umax = 640;
        ctFinal.Vmin = 960;
        ctFinal.Vmax = 480;
    }

    /// <summary>
    /// PanelDrawing_MouseDown
    /// </summary>
    private void PanelDrawing_MouseDown(object sender, MouseEventArgs e)
    {
        Point p = e.Location;

        if (pointsInsertState == PointsInsert.original)
        {
            pointsOrig[currentIndexOrig] = p;

            currentIndexOrig++;

            if (currentIndexOrig > 3)
                currentIndexOrig = 0;
        }
        else
        {
            pointsRef[currentIndexRef] = p;

            currentIndexRef++;

            if (currentIndexRef > 3)
                currentIndexRef = 0;
        }

        // point transformation
        for (int i = 0; i < 4; i++)
            pointsOrigTransformed[i] = ctOrig.FromUVtoXY(pointsOrig[i]);

        for (int i = 0; i < 4; i++)
            pointsRefTransformed[i] = ctRef.FromUVtoXY(pointsRef[i]);

        RefreshTextBox();

        panelDrawing.Invalidate();
    }

    /// <summary>
    /// ButtonLoadOrigImage_Click
    /// </summary>
    private void ButtonLoadOrigImage_Click(object sender, EventArgs e)
    {
        OpenFileDialog openFileDialog = new()
        {
            InitialDirectory = @"Desktop",
            RestoreDirectory = true,
            Title = "Browse PNG Files",
            DefaultExt = "png",
            Filter = "png files (*.png)|*.png|All files (*.*)|*.*",
            FilterIndex = 2,
            CheckFileExists = true,
            CheckPathExists = true
        };
        openFileDialog.ShowDialog();

        string bitmapPath = openFileDialog.FileName;

        if (string.IsNullOrEmpty(bitmapPath))
            return;

        try
        {
            bitmapOrig = new(bitmapPath);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error loading bitmap" + ex.Message);
            return;
        }

        panelDrawing.Invalidate();
    }

    /// <summary>
    /// ButtonRef_Click
    /// </summary>
    private void ButtonRef_Click(object sender, EventArgs e)
    {
        OpenFileDialog openFileDialog = new()
        {
            InitialDirectory = @"Desktop",
            RestoreDirectory = true,
            Title = "Browse PNG Files",
            DefaultExt = "png",
            Filter = "png files (*.png)|*.png|All files (*.*)|*.*",
            FilterIndex = 2,
            CheckFileExists = true,
            CheckPathExists = true
        };
        openFileDialog.ShowDialog();

        string bitmapPath = openFileDialog.FileName;

        if (string.IsNullOrEmpty(bitmapPath))
            return;

        try
        {
            bitmapRef = new(bitmapPath);
            bitmapFinal = (Bitmap)bitmapRef.Clone();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error loading bitmap" + ex.Message);
            return;
        }

        panelDrawing.Invalidate();
    }

    /// <summary>
    /// PanelDrawing_Paint
    /// </summary>
    private void PanelDrawing_Paint(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;

        if (bitmapOrig != null)
            g.DrawImage(bitmapOrig, 0, 0);

        if (bitmapRef != null)
            g.DrawImage(bitmapRef, 640, 0);

        if (bitmapFinal != null)
            g.DrawImage(bitmapFinal, 0, 480);

        using Pen pOrig = new(Brushes.Red, 3f);
        using Pen pRef = new(Brushes.Yellow, 3f);
        using Pen pFinal = new(Brushes.LightGreen, 3f);

        g.DrawLines(pOrig, pointsOrig);
        g.DrawLine(pOrig, pointsOrig[3], pointsOrig[0]);

        g.DrawLines(pRef, pointsRef);
        g.DrawLine(pRef, pointsRef[3], pointsRef[0]);

        g.DrawLines(pFinal, pointsFinal);
        g.DrawLine(pFinal, pointsFinal[3], pointsFinal[0]);
    }

    /// <summary>
    /// ButtonPointsOrig_Click
    /// </summary>
    private void ButtonPointsOrig_Click(object sender, EventArgs e)
    {
        pointsInsertState = PointsInsert.original;
    }

    /// <summary>
    /// ButtonPointsRef_Click
    /// </summary>
    private void ButtonPointsRef_Click(object sender, EventArgs e)
    {
        pointsInsertState |= PointsInsert.reference;
    }

    /// <summary>
    /// Refresh text box
    /// </summary>
    private void RefreshTextBox()
    {
        textBoxInfo.Clear();

        textBoxInfo.AppendText("Original [U,V]:\r\n");
        foreach (var pt in pointsOrig)
            textBoxInfo.AppendText(pt.ToString() + "\r\n");

        textBoxInfo.AppendText("\r\nReference [U,V]:\r\n");
        foreach (var pt in pointsRef)
            textBoxInfo.AppendText(pt.ToString() + "\r\n");

        textBoxInfo.AppendText("\r\nOriginal [X,Y]:\r\n");
        foreach (var pt in pointsOrigTransformed)
            textBoxInfo.AppendText(pt.ToString() + "\r\n");

        textBoxInfo.AppendText("\r\nReference [X,Y]:\r\n");
        foreach (var pt in pointsRefTransformed)
            textBoxInfo.AppendText(pt.ToString() + "\r\n");

        var H = Homography.Calculate(pointsOrigTransformed, pointsRefTransformed);

        textBoxInfo.AppendText("\r\nHomography Matrix H:\r\n");
        textBoxInfo.AppendText(H.ToMatrixString());

        var vector = Vector<double>.Build.DenseOfArray(new double[3]);

        for (int i = 0; i < 4; i++)
        {
            vector[0] = pointsOrigTransformed[i].X;
            vector[1] = pointsOrigTransformed[i].Y;
            vector[2] = 1.0;

            var resultPt = H.Multiply(vector);

            pointsFinalTransformed[i] = new PointF((float)resultPt[0], (float)resultPt[1]);
        }

        textBoxInfo.AppendText("\r\nFinal [X,Y]:\r\n");
        foreach (var pt in pointsFinalTransformed)
            textBoxInfo.AppendText(pt.ToString() + "\r\n");

        // point transformation
        for (int i = 0; i < 4; i++)
            pointsFinal[i] = ctFinal.FromXYtoUV(pointsFinalTransformed[i]);

        textBoxInfo.AppendText("\r\nFinal [U,V]:\r\n");
        foreach (var pt in pointsFinal)
            textBoxInfo.AppendText(pt.ToString() + "\r\n");
    }

    private void RefreshTextBox2()
    {
        textBoxInfo.Clear();

        PointF[] sourcePoints = new PointF[4];
        PointF[] targetPoints = new PointF[4];


        textBoxInfo.AppendText("Original [U,V]:\r\n");
        foreach (var pt in pointsOrig)
            textBoxInfo.AppendText(pt.ToString() + "\r\n");

        textBoxInfo.AppendText("\r\nReference [U,V]:\r\n");
        foreach (var pt in pointsRef)
            textBoxInfo.AppendText(pt.ToString() + "\r\n");

        for(int i = 0;i < 4; i++)
        {
            sourcePoints[i] = new PointF(pointsOrig[i].X, pointsOrig[i].Y);
            targetPoints[i] = new PointF(pointsRef[i].X, pointsRef[i].Y);
        }

        textBoxInfo.AppendText("\r\nSource points [U,V]:\r\n");
        foreach (var pt in sourcePoints)
            textBoxInfo.AppendText(pt.ToString() + "\r\n");

        textBoxInfo.AppendText("\r\nTarget points [U,V]:\r\n");
        foreach (var pt in targetPoints)
            textBoxInfo.AppendText(pt.ToString() + "\r\n");


        var H = Homography.Calculate(sourcePoints, targetPoints);

        textBoxInfo.AppendText("\r\nHomography Matrix H:\r\n");
        textBoxInfo.AppendText(H.ToMatrixString());

        var vector = Vector<double>.Build.DenseOfArray(new double[3]);

        for (int i = 0; i < 4; i++)
        {
            vector[0] = pointsOrig[i].X;
            vector[1] = pointsOrig[i].Y;
            vector[2] = 1.0;

            var resultPt = H.Multiply(vector);

            pointsFinalTransformed[i] = new PointF((float)resultPt[0], (float)resultPt[1]);
        }

        textBoxInfo.AppendText("\r\nFinal [X,Y]:\r\n");
        foreach (var pt in pointsFinalTransformed)
            textBoxInfo.AppendText(pt.ToString() + "\r\n");

        //// point transformation
        //for (int i = 0; i < 4; i++)
        //    pointsFinal[i] = ctFinal.FromXYtoUV(pointsFinalTransformed[i]);

        //textBoxInfo.AppendText("\r\nFinal [U,V]:\r\n");
        //foreach (var pt in pointsFinal)
        //    textBoxInfo.AppendText(pt.ToString() + "\r\n");
    }

}
