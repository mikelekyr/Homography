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
        new(144, 48),
        new(60, 422),
        new(579, 424),
        new(505, 46)
    ];

    private readonly Point[] pointsRef =
    [
        new(754, 35),
        new(754, 444),
        new(1164, 444),
        new(1164, 35)
    ];

    private readonly Point[] pointsFinal = new Point[4];

    private readonly List<Vector<float>> pointsOrigTransformed = [];
    private readonly List<Vector<float>> pointsRefTransformed = [];
    private readonly List<Vector<float>> pointsFinalTransformed = [];

    private int currentIndexOrig = 0;
    private int currentIndexRef = 0;

    private readonly CoordTrans ctOrig = new();
    private readonly CoordTrans ctRef = new();
    private readonly CoordTrans ctFinal = new();
    private readonly CoordTrans ctImageNormalisedOrig = new();
    private readonly CoordTrans ctImageNormalisedRef = new();

    public MainForm()
    {
        InitializeComponent();

        // coord trans original
        ctOrig.Xmin = 0f;
        ctOrig.Xmax = 0.93333f;
        ctOrig.Ymin = 0f;
        ctOrig.Ymax = 7f;

        ctOrig.Umin = 0;
        ctOrig.Umax = 639;
        ctOrig.Vmin = 479;
        ctOrig.Vmax = 0;

        // coord trans reference
        ctRef.Xmin = 0f;
        ctRef.Xmax = 0.93333f;
        ctRef.Ymin = 0f;
        ctRef.Ymax = 7f;

        ctRef.Umin = 640;
        ctRef.Umax = 1279;
        ctRef.Vmin = 479;
        ctRef.Vmax = 0;

        // coord trans final
        ctFinal.Xmin = 0f;
        ctFinal.Xmax = 0.93333f;
        ctFinal.Ymin = 0f;
        ctFinal.Ymax = 7f;

        ctFinal.Umin = 0;
        ctFinal.Umax = 639;
        ctFinal.Vmin = 959;
        ctFinal.Vmax = 480;

        // coord trans Image Normalised origina;
        ctImageNormalisedOrig.Xmin = 0f;
        ctImageNormalisedOrig.Xmax = 1f;
        ctImageNormalisedOrig.Ymin = 0f;
        ctImageNormalisedOrig.Ymax = 1f;

        ctImageNormalisedOrig.Umin = 0;
        ctImageNormalisedOrig.Umax = 639;
        ctImageNormalisedOrig.Vmin = 479;
        ctImageNormalisedOrig.Vmax = 0;

        // coord trans Image Normalised reference
        ctImageNormalisedRef.Xmin = 0f;
        ctImageNormalisedRef.Xmax = 1f;
        ctImageNormalisedRef.Ymin = 0f;
        ctImageNormalisedRef.Ymax = 1f;

        ctImageNormalisedRef.Umin = 640;
        ctImageNormalisedRef.Umax = 1279;
        ctImageNormalisedRef.Vmin = 479;
        ctImageNormalisedRef.Vmax = 0;

        bitmapOrig = new("../../../Assets/imageOrig.png");
        bitmapRef = new("../../../Assets/imageRef.png");
        bitmapFinal = (Bitmap)bitmapRef.Clone();

        RefreshTextBox();
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
        ArgumentNullException.ThrowIfNull(bitmapOrig);

        textBoxInfo.Clear();

        pointsOrigTransformed.Clear();
        pointsRefTransformed.Clear();

        // point transformation
        for (int i = 0; i < 4; i++)
        {
            pointsOrigTransformed.Add(ctOrig.FromUVtoXYVectorFloat(pointsOrig[i]));
            pointsRefTransformed.Add(ctRef.FromUVtoXYVectorFloat(pointsRef[i]));
        }

        //textBoxInfo.AppendText("Original [U,V]:\r\n");
        //foreach (var pt in pointsOrig)
        //    textBoxInfo.AppendText(pt.ToString() + "\r\n");

        //textBoxInfo.AppendText("\r\nOriginal [X,Y]:\r\n");
        //foreach (var pt in pointsOrigTransformed)
        //    textBoxInfo.AppendText(pt.ToString() + "\r\n");

        //textBoxInfo.AppendText("\r\nReference [U,V]:\r\n");
        //foreach (var pt in pointsRef)
        //    textBoxInfo.AppendText(pt.ToString() + "\r\n");

        //textBoxInfo.AppendText("\r\nReference [X,Y]:\r\n");
        //foreach (var pt in pointsRefTransformed)
        //    textBoxInfo.AppendText(pt.ToString() + "\r\n");

        // calculate homography for real world coordinates
        var H = Homography.Calculate(pointsOrigTransformed, pointsRefTransformed);

        textBoxInfo.AppendText("\r\nHomography Matrix H:\r\n");
        textBoxInfo.AppendText(H.ToMatrixString());

        pointsFinalTransformed.Clear();

        for (int i = 0; i < 4; i++)
        {
            var result = H.Multiply(pointsOrigTransformed[i]);
            result /= result[2];

            pointsFinalTransformed.Add(result);
        }

        //textBoxInfo.AppendText("\r\nFinal [X,Y]:\r\n");
        //foreach (var pt in pointsFinalTransformed)
        //    textBoxInfo.AppendText(pt.ToString() + "\r\n");

        // point transformation
        for (int i = 0; i < 4; i++)
            pointsFinal[i] = ctFinal.FromXYVectorFtoUV(pointsFinalTransformed[i]);

        //textBoxInfo.AppendText("\r\nFinal [U,V]:\r\n");
        //foreach (var pt in pointsFinal)
        //    textBoxInfo.AppendText(pt.ToString() + "\r\n");

        List<Vector<float>> pointOrigList = [];
        List<Vector<float>> pointRefList = [];

        // point transformation
        for (int i = 0; i < 4; i++)
        {
            pointOrigList.Add(ctImageNormalisedOrig.FromUVtoXYVectorFloat(pointsOrig[i]));
            pointRefList.Add(ctImageNormalisedRef.FromUVtoXYVectorFloat(pointsRef[i]));
        }

        textBoxInfo.AppendText("\r\nOriginal normalised [X,Y]:\r\n");
        foreach (var pt in pointOrigList)
            textBoxInfo.AppendText(pt.ToString() + "\r\n");

        textBoxInfo.AppendText("\r\nReference normalised [X,Y]:\r\n");
        foreach (var pt in pointRefList)
            textBoxInfo.AppendText(pt.ToString() + "\r\n");

        var HImage = Homography.Calculate(pointOrigList, pointRefList);

        textBoxInfo.AppendText("\r\nHomography Matrix HImage:\r\n");
        textBoxInfo.AppendText(HImage.ToMatrixString());

        bitmapFinal = Homography.ComputeOutputImage(bitmapOrig, HImage, ctImageNormalisedOrig);
    }
}
