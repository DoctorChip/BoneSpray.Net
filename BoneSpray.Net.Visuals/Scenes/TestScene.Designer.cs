using System;

namespace BoneSpray.Net.Visuals.Scenes
{
    partial class TestScene
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
            if (disposing && (components != null))
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
            this.GLControl = new SharpGL.OpenGLControl();
            ((System.ComponentModel.ISupportInitialize)(this.GLControl)).BeginInit();
            this.SuspendLayout();
            this.Load += new EventHandler(this.Start);

            this.GLControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));

            this.GLControl.DrawFPS = true;
            this.GLControl.FrameRate = 28;
            this.GLControl.Location = new System.Drawing.Point(0, 0);
            this.GLControl.Name = "GL";
            this.GLControl.RenderContextType = SharpGL.RenderContextType.FBO;
            this.GLControl.Size = new System.Drawing.Size(1280, 800);
            this.GLControl.TabIndex = 0;
            this.GLControl.OpenGLDraw += new SharpGL.RenderEventHandler(this.GLControl_OpenGLDraw);

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(1280, 800);
            this.Controls.Add(this.GLControl);
            this.Name = "Bone Spray";

            ((System.ComponentModel.ISupportInitialize)(this.GLControl)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private SharpGL.OpenGLControl GLControl;

    }
}