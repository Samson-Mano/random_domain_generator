using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace random_map_generator
{
    public partial class Form1 : Form
    {
        map_generator main_map = new map_generator();

        public Form1()
        {
            InitializeComponent();
        }

        private void button_randommap_Click(object sender, EventArgs e)
        {
            the_static_class.bounding_box = new SizeF((float)(0.8 * main_pic.Width), (float)(0.8 * main_pic.Height));
            the_static_class.bounding_midpt = new PointF((float)(0.5 * main_pic.Width), (float)(0.5 * main_pic.Height));
            generate_map();
        }

        public void generate_map()
        {
            // generate map 
            main_map = new map_generator(the_static_class.bounding_box.Width, the_static_class.bounding_box.Height);
            mt_pic.Refresh();
        }

        public static class the_static_class
        {
            public static SizeF bounding_box;
            public static PointF bounding_midpt;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            the_static_class.bounding_box = new SizeF((float)(0.5 * main_pic.Width), (float)(0.5 * main_pic.Height));
            the_static_class.bounding_midpt = new PointF((float)(0.5 * main_pic.Width), (float)(0.5 * main_pic.Height));
            //generate_map();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            the_static_class.bounding_box = new SizeF((float)(0.8 * main_pic.Width), (float)(0.8 * main_pic.Height));
            the_static_class.bounding_midpt = new PointF((float)(0.5 * main_pic.Width), (float)(0.5 * main_pic.Height));
            mt_pic.Refresh();
        }

        private void main_pic_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality; // Paint high quality
            e.Graphics.TranslateTransform(the_static_class.bounding_midpt.X, the_static_class.bounding_midpt.Y); // Translate transform to make the orgin as center

            //e.Graphics.DrawRectangle(new Pen(Color.Brown, 2), 0, 0, 8, 8);
            System.Drawing.Graphics gr1 = e.Graphics;
            main_map.paint_me(ref gr1);
        }
    }
}
