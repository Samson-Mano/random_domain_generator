using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace constrained_delaunay_triangulation_rupperts
{
    public class pslg_datastructure
    {
        List<point2d> _all_points = new List<point2d>(); // List of point object to store all the points in the drawing area
        List<edge2d> _all_edges = new List<edge2d>(); // List of edge object to store all the edges created from Delaunay triangulation
        List<face2d> _all_faces = new List<face2d>(); // List of face object to store all the faces created from Delaunay triangulation

        public List<surface_store> all_surfaces = new List<surface_store>(); // List of all detected surfaces (visible outside)


        public class surface_store
        {
            private int _surface_id;
            public List<point2d> surface_nodes = new List<point2d>();
            List<edge2d> surface_edges = new List<edge2d>();
            List<surface_store> inner_surfaces = new List<surface_store>();

            System.Drawing.Drawing2D.HatchBrush tri_brush;
            private double signed_area_chk;
            public double surface_area;

            private System.Drawing.Region surface_region = new System.Drawing.Region();

            public int surface_id
            {
                get { return this._surface_id; }
            }


            public surface_store(int i_surf_id, List<point2d> i_surface_nodes, List<edge2d> i_surface_edges, int surf_count)
            {
                this._surface_id = i_surf_id; // add surface id
                surface_nodes.AddRange(i_surface_nodes);
                surface_edges.AddRange(i_surface_edges);

                List<PointF> temp_sur_pts = new List<PointF>();
                //foreach (point2d pt in this.surface_nodes)
                //{
                //    temp_sur_pts.Add(pt.get_point());
                //}

                temp_sur_pts.Add(this.surface_edges[0].start_pt.get_point()); // Add the first point of the surface edge
                foreach (edge2d ed in this.surface_edges)
                {
                    temp_sur_pts.Add(ed.end_pt.get_point()); // since all the surface edges are interconnected only store the end points
                    //surface_path.AddLine(ed.start_pt.get_point(), ed.end_pt.get_point());
                }



                // Set the path of outter surface
                System.Drawing.Drawing2D.GraphicsPath surface_path = new System.Drawing.Drawing2D.GraphicsPath();
                surface_path.StartFigure();
                surface_path.AddPolygon(temp_sur_pts.ToArray());
                surface_path.CloseFigure();

                // set region
                surface_region = new Region(surface_path);


                Color hatch_color = Form1.the_static_class.GetRandomColor(surf_count);
                System.Drawing.Drawing2D.HatchStyle hatch_style = Form1.the_static_class.GetRandomHatchStyle(surf_count);
                Color trans_color = Color.FromArgb(0, 10, 10, 10);

                tri_brush = new System.Drawing.Drawing2D.HatchBrush(hatch_style, hatch_color, trans_color);

                signed_area_chk = this.SignedPolygonArea();
                surface_area = Math.Abs(signed_area_chk);
            }

            public void reverse_surface_orinetation()
            {
                List<edge2d> temp_edge_list = new List<edge2d>();

                for (int i = this.surface_edges.Count - 1; i >= 0; i--) // reverse the list
                {
                    temp_edge_list.Add(new edge2d(this.surface_edges[i].edge_id, this.surface_edges[i].end_pt, this.surface_edges[i].start_pt));
                }

                // clear the edge list
                this.surface_edges = new List<edge2d>();
                this.surface_edges.AddRange(temp_edge_list);
            }

            public void set_inner_surfaces(List<surface_store> i_inner_surfaces)
            {
                // add inner surface one after another
                for (int i = 0; i < i_inner_surfaces.Count; i++)
                {
                    if (i_inner_surfaces[i].SignedPolygonArea() > 0) // check whether the inner surface is oriented clockwise (positive area = anti-clockwise)
                    {
                        // anti-clockwise orientation detected so reverse the orientation to be clockwise
                        i_inner_surfaces[i].reverse_surface_orinetation();
                    }
                    inner_surfaces.Add(i_inner_surfaces[i]); // inner surface is clockwise
                }
                //inner_surfaces.AddRange(i_inner_surfaces);

                foreach (surface_store surf in inner_surfaces) // cycle through all the inner surfaces
                {
                    // Set the path of inner surface
                    List<PointF> temp_sur_pts = new List<PointF>();
                    foreach (edge2d ed in surf.surface_edges)
                    {
                        temp_sur_pts.Add(ed.end_pt.get_point());
                    }

                    System.Drawing.Drawing2D.GraphicsPath inner_surface = new System.Drawing.Drawing2D.GraphicsPath();
                    inner_surface.StartFigure();
                    inner_surface.AddPolygon(temp_sur_pts.ToArray());
                    inner_surface.CloseFigure();


                    // set region
                    surface_region.Exclude(inner_surface); // exclude the inner surface region

                }
            }

            public void paint_me(ref Graphics gr0) // this function is used to paint the points
            {
                gr0.FillRegion(tri_brush, surface_region);// Fill the surface region

                // Paint the surface ID and inner surface ids and surface area
                String txt ="surface " + this.surface_id.ToString() + " " + ((inner_surfaces.Count>0)?"=>":"");
                foreach (surface_store inner_surf in inner_surfaces)
                {
                    txt = txt  + inner_surf.surface_id.ToString()+ ",";
                }
                txt = (inner_surfaces.Count > 0) ? txt.Remove(txt.Length - 1) : txt;

                Font drawFont = new Font("Arial", 16);
                gr0.DrawString(txt, drawFont, new SolidBrush(Color.Black), surface_nodes[0].get_point());

                Graphics gr1 = gr0;
                surface_edges.ForEach(obj => obj.paint_me(ref gr1)); // Paint the edges
                surface_nodes.ForEach(obj => obj.paint_me(ref gr1)); // Paint the faces
            }

            // Return the polygon's area in "square units."
            // The value will be negative if the polygon is
            // oriented clockwise.
            public double SignedPolygonArea()
            {
                //The total calculated area is negative if the polygon is oriented clockwise

                // Extract point
                List<PointF> polygon_pt = new List<PointF>();
                foreach (edge2d ed in this.surface_edges)
                {
                    polygon_pt.Add(ed.end_pt.get_point());
                }

                // Add the first point to the end.
                int num_points = polygon_pt.Count;
                PointF[] pts = new PointF[num_points + 1];
                polygon_pt.CopyTo(pts, 0);
                pts[num_points] = polygon_pt[0];

                // Get the areas.
                double area = 0;
                for (int i = 0; i < num_points; i++)
                {
                    area +=
                        (pts[i + 1].X - pts[i].X) *
                        (pts[i + 1].Y + pts[i].Y) / 2;
                }

                // Return the result.
                return area;
            }


            // Return True if the point is in the polygon.
            public bool PointInPolygon(double X, double Y)
            {
                // Extract point
                List<PointF> polygon_pt = new List<PointF>();
                //polygon_pt.Add(this.surface_edges[0].start_pt.get_point());
                foreach (edge2d ed in this.surface_edges)
                {
                    polygon_pt.Add(ed.end_pt.get_point());
                }

                PointF test_pt = new PointF((float)X, (float)Y);

                return Form1.the_static_class.IsPointInPolygon4(polygon_pt.ToArray(), test_pt);

                //// Get the angle between the point and the
                //// first and last vertices.
                //int max_point = polygon_pt.Count - 1;
                //double total_angle = Form1.the_static_class.GetAngle(
                //    polygon_pt[max_point].X, polygon_pt[max_point].Y,
                //    X, Y,
                //    polygon_pt[0].X, polygon_pt[0].Y);

                //// Add the angles from the point
                //// to each other pair of vertices.
                //for (int i = 0; i < max_point; i++)
                //{
                //    total_angle += Form1.the_static_class.GetAngle(
                //        polygon_pt[i].X, polygon_pt[i].Y,
                //        X, Y,
                //        polygon_pt[i + 1].X, polygon_pt[i + 1].Y);
                //}

                //// The total angle should be 2 * PI or -2 * PI if
                //// the point is in the polygon and close to zero
                //// if the point is outside the polygon.
                //// The following statement was changed. See the comments.
                ////return (Math.Abs(total_angle) > 0.000001);
                //return (Math.Abs(total_angle) > 1);
            }
        }


        public List<point2d> all_points
        {
            set { this._all_points = value; }
            get { return this._all_points; }
        }

        public List<edge2d> all_edges
        {
            set { this._all_edges = value; }
            get { return this._all_edges; }
        }

        public List<face2d> all_faces
        {
            set { this._all_faces = value; }
            get { return this._all_faces; }
        }

        public pslg_datastructure()
        {
            // Empty constructor used to initialize and re-intialize
            this._all_points = new List<point2d>();
            this._all_edges = new List<edge2d>();
        }

        public void paint_me(ref Graphics gr1)
        {
            Graphics gr0 = gr1;

            //all_edges.ForEach(obj => obj.paint_me(ref gr0)); // Paint the edges
            //all_faces.ForEach(obj => obj.paint_me(ref gr0)); // Paint the faces

            //all_points.ForEach(obj => obj.paint_me(ref gr0)); // Paint the points

            all_surfaces.ForEach(obj => obj.paint_me(ref gr0)); // Pain the surface
        }

        public class point2d // class to store the points
        {
            int _id;
            double _x;
            double _y;

            public int id
            {
                get { return this._id; }
            }

            public double x
            {
                get { return this._x; }
            }
            public double y
            {
                get { return this._y; }
            }
            public point2d(int i_id, double i_x, double i_y)
            {
                // constructor 1
                this._id = i_id;
                this._x = i_x;
                this._y = i_y;
            }

            public void paint_me(ref Graphics gr0) // this function is used to paint the points
            {
                gr0.FillEllipse(new Pen(Color.BlueViolet, 2).Brush, new RectangleF(get_point_for_ellipse(), new SizeF(4, 4)));

                if (Form1.the_static_class.ispaint_label == true)
                {
                    string my_string = (this.id + 1).ToString() + "(" + this._x.ToString("F2") + ", " + this._y.ToString("F2") + ")";
                    SizeF str_size = gr0.MeasureString(my_string, new Font("Cambria", 6)); // Measure string size to position the dimension

                    gr0.DrawString(my_string, new Font("Cambria", 6),
                                                                       new Pen(Color.DarkBlue, 2).Brush,
                                                                       get_point_for_ellipse().X + 3 + Form1.the_static_class.to_single(-str_size.Width * 0.5),
                                                                       Form1.the_static_class.to_single(str_size.Height * 0.5) + get_point_for_ellipse().Y + 3);
                }
            }

            public PointF get_point_for_ellipse()
            {
                return (new PointF(Form1.the_static_class.to_single(this._x) - 2,
               Form1.the_static_class.to_single((this._y) - 2))); // return the point as PointF as edge of an ellipse
            }

            public PointF get_point()
            {
                return (new PointF(Form1.the_static_class.to_single(this._x),
               Form1.the_static_class.to_single( this._y))); // return the point as PointF as edge of an ellipse
            }

            public bool Equals(point2d other)
            {
                return (this._x == other.x && this._y == other.y); // Equal function is used to check the uniqueness of the points added
            }
        }

        public class points_equality_comparer : IEqualityComparer<point2d>
        {
            public bool Equals(point2d a, point2d b)
            {
                return (a.Equals(b));
            }

            public int GetHashCode(point2d other)
            {
                return (other.x.GetHashCode() * 17 + other.y.GetHashCode() * 19);
                // 17,19 are just ranfom prime numbers
            }
        }

        public class edge2d
        {
            int _edge_id;
            point2d _start_pt;
            point2d _end_pt;
            point2d _mid_pt; // not stored in point list

            public int edge_id
            {
                get { return this._edge_id; }
            }

            public point2d start_pt
            {
                get { return this._start_pt; }
            }

            public point2d end_pt
            {
                get { return this._end_pt; }
            }

            public point2d mid_pt
            {
                get { return this._mid_pt; }
            }

            public edge2d(int i_edge_id, point2d i_start_pt, point2d i_end_pt)
            {
                // constructor 1
                this._edge_id = i_edge_id;
                this._start_pt = i_start_pt;
                this._end_pt = i_end_pt;
                this._mid_pt = new point2d(-1, (i_start_pt.x + i_end_pt.x) * 0.5, (i_start_pt.y + i_end_pt.y) * 0.5);
            }

            public void paint_me(ref Graphics gr0) // this function is used to paint the points
            {
                Pen edge_pen = new Pen(Color.DarkOrange, 1);

                gr0.DrawLine(edge_pen, mid_pt.get_point(), end_pt.get_point());

                System.Drawing.Drawing2D.AdjustableArrowCap bigArrow = new System.Drawing.Drawing2D.AdjustableArrowCap(3, 3);
                edge_pen.CustomEndCap = bigArrow;
                gr0.DrawLine(edge_pen, start_pt.get_point(), mid_pt.get_point());
            }

            public bool Equals(edge2d other)
            {
                return (other.start_pt.Equals(this._start_pt) && other.end_pt.Equals(this._end_pt));
            }

        }

        public class face2d
        {
            int _face_id;
            point2d _p1;
            point2d _p2;
            point2d _p3;
            point2d _mid_pt;
            double shrink_factor = 0.6f; //
                                         // in Circle
            point2d _circle_center;
            double _circle_radius;
            point2d _ellipse_edge;

            public int face_id
            {
                get { return this._face_id; }
            }

            public PointF get_p1
            {
                get
                {
                    return new PointF(Form1.the_static_class.to_single(_mid_pt.get_point().X * (1 - shrink_factor) + (_p1.get_point().X * shrink_factor)),
                                       Form1.the_static_class.to_single(_mid_pt.get_point().Y * (1 - shrink_factor) + (_p1.get_point().Y * shrink_factor)));
                }
            }

            public PointF get_p2
            {
                get
                {
                    return new PointF(Form1.the_static_class.to_single(_mid_pt.get_point().X * (1 - shrink_factor) + (_p2.get_point().X * shrink_factor)),
                                      Form1.the_static_class.to_single(_mid_pt.get_point().Y * (1 - shrink_factor) + (_p2.get_point().Y * shrink_factor)));
                }
            }

            public PointF get_p3
            {
                get
                {
                    return new PointF(Form1.the_static_class.to_single(_mid_pt.get_point().X * (1 - shrink_factor) + (_p3.get_point().X * shrink_factor)),
                                      Form1.the_static_class.to_single(_mid_pt.get_point().Y * (1 - shrink_factor) + (_p3.get_point().Y * shrink_factor)));
                }
            }

            public face2d(int i_face_id, point2d i_p1, point2d i_p2, point2d i_p3)
            {
                this._face_id = i_face_id;
                this._p1 = i_p1;
                this._p2 = i_p2;
                this._p3 = i_p3;
                this._mid_pt = new point2d(-1, (i_p1.x + i_p2.x + i_p3.x) / 3, (i_p1.y + i_p2.y + i_p3.y) / 3);

                set_incircle();

            }

            private void set_incircle()
            {
                double dA = (this._p1.x * this._p1.x) + (this._p1.y * this._p1.y);
                double dB = (this._p2.x * this._p2.x) + (this._p2.y * this._p2.y);
                double dC = (this._p3.x * this._p3.x) + (this._p3.y * this._p3.y);

                double aux1 = (dA * (this._p3.y - this._p2.y) + dB * (this._p1.y - this._p3.y) + dC * (this._p2.y - this._p1.y));
                double aux2 = -(dA * (this._p3.x - this._p2.x) + dB * (this._p1.x - this._p3.x) + dC * (this._p2.x - this._p1.x));
                double div = (2 * (this._p1.x * (this._p3.y - this._p2.y) + this._p2.x * (this._p1.y - this._p3.y) + this._p3.x * (this._p2.y - this._p1.y)));

                if (div != 0)
                {

                }

                //Circumcircle
                double center_x = aux1 / div;
                double center_y = aux2 / div;

                this._circle_center = new point2d(-1, center_x, center_y);
                this._circle_radius = Math.Sqrt((center_x - this._p1.x) * (center_x - this._p1.x) + (center_y - this._p1.y) * (center_y - this._p1.y));
                this._ellipse_edge = new point2d(-1, center_x - this._circle_radius, center_y + this._circle_radius);

            }


            public void paint_me(ref Graphics gr0) // this function is used to paint the points
            {
                Pen triangle_pen = new Pen(Color.LightGreen, 1);

                if (Form1.the_static_class.is_paint_mesh == true)
                {
                    PointF[] curve_pts = { get_p1, get_p2, get_p3 };
                    gr0.FillPolygon(triangle_pen.Brush, curve_pts); // Fill the polygon

                    if (Form1.the_static_class.ispaint_label == true)
                    {
                        string my_string = this._face_id.ToString();
                        SizeF str_size = gr0.MeasureString(my_string, new Font("Cambria", 6)); // Measure string size to position the dimension

                        gr0.DrawString(my_string, new Font("Cambria", 6), new Pen(Color.DeepPink, 2).Brush, this._mid_pt.get_point());

                    }
                }

                if (Form1.the_static_class.is_paint_incircle == true)
                {
                    gr0.DrawEllipse(triangle_pen, this._ellipse_edge.get_point().X,
                                                     this._ellipse_edge.get_point().Y,
                                                     Form1.the_static_class.to_single(this._circle_radius * 2),
                                                     Form1.the_static_class.to_single(this._circle_radius * 2));
                }

            }
        }
    }
}
