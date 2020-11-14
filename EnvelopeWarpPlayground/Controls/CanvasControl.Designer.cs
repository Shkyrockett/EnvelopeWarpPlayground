// <copyright file="CanvasControl.Designer.cs">
//     Copyright © 2020 Shkyrockett. All rights reserved.
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
    /// <seealso cref="System.Windows.Forms.UserControl" />
    partial class CanvasControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CanvasControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Name = "CanvasControl";
            this.Load += new System.EventHandler(this.CanvasControl_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CanvasControl_Paint);
            this.Enter += new System.EventHandler(this.CanvasControl_Enter);
            this.Leave += new System.EventHandler(this.CanvasControl_Leave);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CanvasControl_MouseDown);
            this.MouseEnter += new System.EventHandler(this.CanvasControl_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.CanvasControl_MouseLeave);
            this.MouseHover += new System.EventHandler(this.CanvasControl_MouseHover);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CanvasControl_MouseMove_NotDrawing);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.CanvasControl_MouseWheel);
            this.Resize += new System.EventHandler(this.CanvasControl_Resize);
            this.ResumeLayout(false);
        }
        #endregion
    }
}
