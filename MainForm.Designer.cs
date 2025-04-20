namespace HomographyApp
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panelMenu = new Panel();
            buttonPointsRef = new Button();
            buttonPointsOrig = new Button();
            textBoxInfo = new TextBox();
            buttonRef = new Button();
            buttonLoadOrigImage = new Button();
            panelDrawing = new Panel();
            buttonSaveMatrixH = new Button();
            panelMenu.SuspendLayout();
            SuspendLayout();
            // 
            // panelMenu
            // 
            panelMenu.BackColor = Color.DarkGray;
            panelMenu.Controls.Add(buttonSaveMatrixH);
            panelMenu.Controls.Add(buttonPointsRef);
            panelMenu.Controls.Add(buttonPointsOrig);
            panelMenu.Controls.Add(textBoxInfo);
            panelMenu.Controls.Add(buttonRef);
            panelMenu.Controls.Add(buttonLoadOrigImage);
            panelMenu.Dock = DockStyle.Left;
            panelMenu.Location = new Point(0, 0);
            panelMenu.Name = "panelMenu";
            panelMenu.Size = new Size(270, 984);
            panelMenu.TabIndex = 0;
            // 
            // buttonPointsRef
            // 
            buttonPointsRef.Location = new Point(140, 44);
            buttonPointsRef.Name = "buttonPointsRef";
            buttonPointsRef.Size = new Size(115, 26);
            buttonPointsRef.TabIndex = 4;
            buttonPointsRef.Text = "Points Ref";
            buttonPointsRef.UseVisualStyleBackColor = true;
            buttonPointsRef.Click += ButtonPointsRef_Click;
            // 
            // buttonPointsOrig
            // 
            buttonPointsOrig.Location = new Point(140, 12);
            buttonPointsOrig.Name = "buttonPointsOrig";
            buttonPointsOrig.Size = new Size(115, 26);
            buttonPointsOrig.TabIndex = 3;
            buttonPointsOrig.Text = "Points Orig";
            buttonPointsOrig.UseVisualStyleBackColor = true;
            buttonPointsOrig.Click += ButtonPointsOrig_Click;
            // 
            // textBoxInfo
            // 
            textBoxInfo.Location = new Point(12, 108);
            textBoxInfo.Multiline = true;
            textBoxInfo.Name = "textBoxInfo";
            textBoxInfo.ScrollBars = ScrollBars.Vertical;
            textBoxInfo.Size = new Size(243, 864);
            textBoxInfo.TabIndex = 2;
            // 
            // buttonRef
            // 
            buttonRef.Location = new Point(12, 44);
            buttonRef.Name = "buttonRef";
            buttonRef.Size = new Size(122, 26);
            buttonRef.TabIndex = 1;
            buttonRef.Text = "Load Ref";
            buttonRef.UseVisualStyleBackColor = true;
            buttonRef.Click += ButtonRef_Click;
            // 
            // buttonLoadOrigImage
            // 
            buttonLoadOrigImage.Location = new Point(12, 12);
            buttonLoadOrigImage.Name = "buttonLoadOrigImage";
            buttonLoadOrigImage.Size = new Size(122, 26);
            buttonLoadOrigImage.TabIndex = 0;
            buttonLoadOrigImage.Text = "Load Orig";
            buttonLoadOrigImage.UseVisualStyleBackColor = true;
            buttonLoadOrigImage.Click += ButtonLoadOrigImage_Click;
            // 
            // panelDrawing
            // 
            panelDrawing.BackColor = SystemColors.ControlLightLight;
            panelDrawing.Dock = DockStyle.Fill;
            panelDrawing.Location = new Point(270, 0);
            panelDrawing.Name = "panelDrawing";
            panelDrawing.Size = new Size(1314, 984);
            panelDrawing.TabIndex = 1;
            panelDrawing.Paint += PanelDrawing_Paint;
            panelDrawing.MouseDown += PanelDrawing_MouseDown;
            // 
            // buttonSaveMatrixH
            // 
            buttonSaveMatrixH.Location = new Point(12, 76);
            buttonSaveMatrixH.Name = "buttonSaveMatrixH";
            buttonSaveMatrixH.Size = new Size(122, 26);
            buttonSaveMatrixH.TabIndex = 5;
            buttonSaveMatrixH.Text = "Save Matrix H";
            buttonSaveMatrixH.UseVisualStyleBackColor = true;
            buttonSaveMatrixH.Click += ButtonSaveMatrixH_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1584, 984);
            Controls.Add(panelDrawing);
            Controls.Add(panelMenu);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Homography caluculator";
            panelMenu.ResumeLayout(false);
            panelMenu.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelMenu;
        private Button buttonLoadOrigImage;
        private Panel panelDrawing;
        private Button buttonRef;
        private Button buttonPointsOrig;
        private TextBox textBoxInfo;
        private Button buttonPointsRef;
        private Button buttonSaveMatrixH;
    }
}
