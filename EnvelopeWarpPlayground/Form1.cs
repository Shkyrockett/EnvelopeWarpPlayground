// <copyright file="Form1.cs">
//     Copyright © 2019 - 2020 Shkyrockett. All rights reserved.
// </copyright>
// <author id="shkyrockett">Shkyrockett</author>
// <license>
//     Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </license>
// <summary></summary>
// <remarks></remarks>

using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace EnvelopeWarpPlayground
{
    /// <summary>
    /// The form1 class.
    /// </summary>
    /// <seealso cref="Form" />
    public partial class Form1
        : Form
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Form1" /> class.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Form1()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles the Load event of the Form1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Form1_Load(object sender, EventArgs e)
        { }

        /// <summary>
        /// Handles the Click event of the ButtonResetPan control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ButtonResetPan_Click(object sender, EventArgs e)
        {
            canvasControl.PanPoint = new PointF(0f, 0f);
        }

        /// <summary>
        /// Handles the Click event of the ButtonResetScale control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ButtonResetScale_Click(object sender, EventArgs e)
        {
            canvasControl.Scale = 1;
        }
        #endregion Event Handlers
    }
}
