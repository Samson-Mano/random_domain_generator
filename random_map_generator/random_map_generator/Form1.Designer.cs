﻿namespace random_map_generator
{
    partial class Form1
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
            this.main_pic = new System.Windows.Forms.Panel();
            this.button_randommap = new System.Windows.Forms.Button();
            this.mt_pic = new System.Windows.Forms.PictureBox();
            this.main_pic.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mt_pic)).BeginInit();
            this.SuspendLayout();
            // 
            // main_pic
            // 
            this.main_pic.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.main_pic.BackColor = System.Drawing.Color.White;
            this.main_pic.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.main_pic.Controls.Add(this.mt_pic);
            this.main_pic.Location = new System.Drawing.Point(12, 64);
            this.main_pic.Name = "main_pic";
            this.main_pic.Size = new System.Drawing.Size(800, 600);
            this.main_pic.TabIndex = 0;
            this.main_pic.Paint += new System.Windows.Forms.PaintEventHandler(this.main_pic_Paint);
            // 
            // button_randommap
            // 
            this.button_randommap.Font = new System.Drawing.Font("Cambria", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_randommap.Location = new System.Drawing.Point(12, 12);
            this.button_randommap.Name = "button_randommap";
            this.button_randommap.Size = new System.Drawing.Size(133, 37);
            this.button_randommap.TabIndex = 1;
            this.button_randommap.Text = "Create random map";
            this.button_randommap.UseVisualStyleBackColor = true;
            this.button_randommap.Click += new System.EventHandler(this.button_randommap_Click);
            // 
            // mt_pic
            // 
            this.mt_pic.BackColor = System.Drawing.Color.Transparent;
            this.mt_pic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mt_pic.Enabled = false;
            this.mt_pic.Location = new System.Drawing.Point(0, 0);
            this.mt_pic.Name = "mt_pic";
            this.mt_pic.Size = new System.Drawing.Size(796, 596);
            this.mt_pic.TabIndex = 0;
            this.mt_pic.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 676);
            this.Controls.Add(this.button_randommap);
            this.Controls.Add(this.main_pic);
            this.Name = "Form1";
            this.Text = "Random Map Generator";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.main_pic.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mt_pic)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel main_pic;
        private System.Windows.Forms.Button button_randommap;
        private System.Windows.Forms.PictureBox mt_pic;
    }
}

