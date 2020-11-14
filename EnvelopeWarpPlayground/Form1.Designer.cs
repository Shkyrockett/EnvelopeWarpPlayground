// <copyright file="Form1.Designer.cs">
//     Copyright © 2019 - 2020 Shkyrockett. All rights reserved.
// </copyright>
// <author id="shkyrockett">Shkyrockett</author>
// <license>
//     Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </license>
// <summary></summary>
// <remarks></remarks>

namespace EnvelopeWarpPlayground
{
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
            this.SuspendLayout();
            // 
            // canvasControl
            // 
            this.canvasControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.canvasControl.GhostPolygonPen = null;
            this.canvasControl.HandleRadius = 3;
            this.canvasControl.Location = new System.Drawing.Point(14, 14);
            this.canvasControl.Name = "canvasControl";
            this.canvasControl.PanPoint = ((System.Drawing.PointF)(resources.GetObject("canvasControl.PanPoint")));
            this.canvasControl.Scale = 1F;
            this.canvasControl.Size = new System.Drawing.Size(824, 491);
            this.canvasControl.TabIndex = 0;
            this.canvasControl.TabStop = false;
            // 
            // buttonResetPan
            // 
            this.buttonResetPan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonResetPan.Location = new System.Drawing.Point(845, 14);
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
            this.buttonResetScale.Location = new System.Drawing.Point(845, 43);
            this.buttonResetScale.Name = "buttonResetScale";
            this.buttonResetScale.Size = new System.Drawing.Size(75, 23);
            this.buttonResetScale.TabIndex = 2;
            this.buttonResetScale.Text = "Reset Scale";
            this.buttonResetScale.UseVisualStyleBackColor = true;
            this.buttonResetScale.Click += new System.EventHandler(this.ButtonResetScale_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 519);
            this.Controls.Add(this.buttonResetScale);
            this.Controls.Add(this.buttonResetPan);
            this.Controls.Add(this.canvasControl);
            this.Name = "Form1";
            this.Text = "Envelope Warp Playground";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }
        #endregion
    }
}

