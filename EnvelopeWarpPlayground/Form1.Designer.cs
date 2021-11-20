// <copyright file="Form1.Designer.cs">
//     Copyright © 2019 - 2020 Shkyrockett. All rights reserved.
// </copyright>
// <author id="shkyrockett">Shkyrockett</author>
// <license>
//     Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </license>
// <summary></summary>
// <remarks></remarks>

namespace EnvelopeWarpPlayground;

/// <summary>
/// 
/// </summary>
/// <seealso cref="System.Windows.Forms.Form" />
partial class Form1
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// The canvas control
    /// </summary>
    private CanvasControl canvasControl;

    /// <summary>
    /// The pan reset button
    /// </summary>
    private System.Windows.Forms.Button buttonResetPan;

    /// <summary>
    /// The reset scale button
    /// </summary>
    private System.Windows.Forms.Button buttonResetScale;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components is not null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
        this.canvasControl = new EnvelopeWarpPlayground.CanvasControl();
        this.buttonResetPan = new System.Windows.Forms.Button();
        this.buttonResetScale = new System.Windows.Forms.Button();
        this.splitContainer1 = new System.Windows.Forms.SplitContainer();
        this.splitContainer2 = new System.Windows.Forms.SplitContainer();
        this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
        this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
        ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
        this.splitContainer1.Panel1.SuspendLayout();
        this.splitContainer1.Panel2.SuspendLayout();
        this.splitContainer1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
        this.splitContainer2.Panel1.SuspendLayout();
        this.splitContainer2.Panel2.SuspendLayout();
        this.splitContainer2.SuspendLayout();
        this.flowLayoutPanel1.SuspendLayout();
        this.SuspendLayout();
        // 
        // canvasControl
        // 
        this.canvasControl.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.canvasControl.Dock = System.Windows.Forms.DockStyle.Fill;
        this.canvasControl.GhostPolygonPen = null;
        this.canvasControl.Location = new System.Drawing.Point(0, 0);
        this.canvasControl.Name = "canvasControl";
        this.canvasControl.Pan = ((System.Drawing.PointF)(resources.GetObject("canvasControl.PanPoint")));
        this.canvasControl.Zoom = 1F;
        this.canvasControl.Size = new System.Drawing.Size(750, 495);
        this.canvasControl.TabIndex = 0;
        this.canvasControl.TabStop = false;
        // 
        // buttonResetPan
        // 
        this.buttonResetPan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.buttonResetPan.Location = new System.Drawing.Point(3, 3);
        this.buttonResetPan.Name = "buttonResetPan";
        this.buttonResetPan.Size = new System.Drawing.Size(75, 23);
        this.buttonResetPan.TabIndex = 1;
        this.buttonResetPan.Text = "Reset Pan";
        this.buttonResetPan.UseVisualStyleBackColor = true;
        this.buttonResetPan.Click += new System.EventHandler(this.ButtonResetPan_Click);
        // 
        // buttonResetScale
        // 
        this.buttonResetScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.buttonResetScale.Location = new System.Drawing.Point(3, 32);
        this.buttonResetScale.Name = "buttonResetScale";
        this.buttonResetScale.Size = new System.Drawing.Size(75, 23);
        this.buttonResetScale.TabIndex = 2;
        this.buttonResetScale.Text = "Reset Scale";
        this.buttonResetScale.UseVisualStyleBackColor = true;
        this.buttonResetScale.Click += new System.EventHandler(this.ButtonResetScale_Click);
        // 
        // splitContainer1
        // 
        this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
        | System.Windows.Forms.AnchorStyles.Left)
        | System.Windows.Forms.AnchorStyles.Right)));
        this.splitContainer1.Location = new System.Drawing.Point(12, 12);
        this.splitContainer1.Name = "splitContainer1";
        // 
        // splitContainer1.Panel1
        // 
        this.splitContainer1.Panel1.Controls.Add(this.canvasControl);
        // 
        // splitContainer1.Panel2
        // 
        this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
        this.splitContainer1.Size = new System.Drawing.Size(909, 495);
        this.splitContainer1.SplitterDistance = 750;
        this.splitContainer1.TabIndex = 3;
        // 
        // splitContainer2
        // 
        this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
        this.splitContainer2.Location = new System.Drawing.Point(0, 0);
        this.splitContainer2.Name = "splitContainer2";
        this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
        // 
        // splitContainer2.Panel1
        // 
        this.splitContainer2.Panel1.Controls.Add(this.flowLayoutPanel1);
        // 
        // splitContainer2.Panel2
        // 
        this.splitContainer2.Panel2.Controls.Add(this.propertyGrid1);
        this.splitContainer2.Size = new System.Drawing.Size(155, 495);
        this.splitContainer2.SplitterDistance = 60;
        this.splitContainer2.TabIndex = 0;
        // 
        // flowLayoutPanel1
        // 
        this.flowLayoutPanel1.Controls.Add(this.buttonResetPan);
        this.flowLayoutPanel1.Controls.Add(this.buttonResetScale);
        this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
        this.flowLayoutPanel1.Name = "flowLayoutPanel1";
        this.flowLayoutPanel1.Size = new System.Drawing.Size(155, 60);
        this.flowLayoutPanel1.TabIndex = 0;
        // 
        // propertyGrid1
        // 
        this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
        this.propertyGrid1.Name = "propertyGrid1";
        this.propertyGrid1.Size = new System.Drawing.Size(155, 431);
        this.propertyGrid1.TabIndex = 0;
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(933, 519);
        this.Controls.Add(this.splitContainer1);
        this.Name = "Form1";
        this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
        this.Text = "Envelope Warp Playground";
        this.Load += new System.EventHandler(this.Form1_Load);
        this.splitContainer1.Panel1.ResumeLayout(false);
        this.splitContainer1.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
        this.splitContainer1.ResumeLayout(false);
        this.splitContainer2.Panel1.ResumeLayout(false);
        this.splitContainer2.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
        this.splitContainer2.ResumeLayout(false);
        this.flowLayoutPanel1.ResumeLayout(false);
        this.ResumeLayout(false);

    }
    #endregion

    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.SplitContainer splitContainer2;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private System.Windows.Forms.PropertyGrid propertyGrid1;
}

