using MathNet.Numerics.LinearAlgebra;
using System.Text.Json;

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
        new(117, 24),
        new(73, 448),
        new(552, 444),
        new(517, 20)
    ];

    private readonly Point[] pointsRef =
    [
        new(765, 30),
        new(765, 446),
        new(1159, 446),
        new(1159, 30)
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

    private Matrix<float>? H; 

    public MainForm()
    {
        InitializeComponent();

        // coord trans original
        ctOrig.Xmin = -3.25f;
        ctOrig.Xmax = 3.25f;
        ctOrig.Ymin = 0f;
        ctOrig.Ymax = 4.6f;

        ctOrig.Umin = 0;
        ctOrig.Umax = 639;
        ctOrig.Vmin = 479;
        ctOrig.Vmax = 0;

        // coord trans reference
        ctRef.Xmin = -3.25f;
        ctRef.Xmax = 3.25f;
        ctRef.Ymin = 0f;
        ctRef.Ymax = 4.6f;

        ctRef.Umin = 640;
        ctRef.Umax = 1279;
        ctRef.Vmin = 479;
        ctRef.Vmax = 0;

        // coord trans final
        ctFinal.Xmin = -3.25f; 
        ctFinal.Xmax = 3.25f;
        ctFinal.Ymin = 0f;
        ctFinal.Ymax = 4.6f;

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

        bitmapFinal = (Bitmap)bitmapOrig.Clone();

        RefreshData();
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

        RefreshData();

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
    private void RefreshData()
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

        textBoxInfo.AppendText("Original [U,V]:\r\n");
        foreach (var pt in pointsOrig)
            textBoxInfo.AppendText(pt.ToString() + "\r\n");

        textBoxInfo.AppendText("\r\nOriginal [X,Y]:\r\n");
        foreach (var pt in pointsOrigTransformed)
            textBoxInfo.AppendText(pt.ToString() + "\r\n");

        textBoxInfo.AppendText("\r\nReference [U,V]:\r\n");
        foreach (var pt in pointsRef)
            textBoxInfo.AppendText(pt.ToString() + "\r\n");

        textBoxInfo.AppendText("\r\nReference [X,Y]:\r\n");
        foreach (var pt in pointsRefTransformed)
            textBoxInfo.AppendText(pt.ToString() + "\r\n");

        // calculate homography for real world coordinates
        H = Homography.Calculate(pointsOrigTransformed, pointsRefTransformed);

        textBoxInfo.AppendText("\r\nHomography Matrix H:\r\n");
        textBoxInfo.AppendText(H.ToMatrixString());

        pointsFinalTransformed.Clear();

        for (int i = 0; i < 4; i++)
        {
            var result = H.Multiply(pointsOrigTransformed[i]);
            result /= result[2];

            pointsFinalTransformed.Add(result);
        }

        textBoxInfo.AppendText("\r\nFinal [X,Y]:\r\n");
        foreach (var pt in pointsFinalTransformed)
            textBoxInfo.AppendText(pt.ToString() + "\r\n");

        // point transformation
        for (int i = 0; i < 4; i++)
            pointsFinal[i] = ctFinal.FromXYVectorFtoUV(pointsFinalTransformed[i]);

        textBoxInfo.AppendText("\r\nFinal [U,V]:\r\n");
        foreach (var pt in pointsFinal)
            textBoxInfo.AppendText(pt.ToString() + "\r\n");

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
    /// ButtonSaveMatrixH_Click
    /// </summary>
    private void ButtonSaveMatrixH_Click(object sender, EventArgs e)
    {
        if (H == null)
        {
            MessageBox.Show("Matrix does not exist.");
            return;
        }

        using SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            InitialDirectory = @"Desktop",
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            Title = "Save matrix as JSON",
            DefaultExt = "json",
            FileName = "MatrixH.json"
        };

        saveFileDialog.ShowDialog();

        string filePath = saveFileDialog.FileName;

        if (string.IsNullOrEmpty(filePath))
            return;

        try
        {
            // Convert to jagged array
            float[][] jagged = new float[H.RowCount][];
            for (int i = 0; i < H.RowCount; i++)
            {
                jagged[i] = [.. H.Row(i)];
            }

            var json = JsonSerializer.Serialize(jagged, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(saveFileDialog.FileName, json);
            MessageBox.Show("Matrix saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error saving matrix to file" + ex.Message);
            return;
        }
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
}
