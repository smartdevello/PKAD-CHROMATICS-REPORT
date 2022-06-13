using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Linq;

namespace PKAD_CHROMATICS_REPORT
{
    public class Renderer
    {
        private int width = 0, height = 0;
        private double totHeight = 1000;
        private Bitmap bmp = null;
        private Graphics gfx = null;
        private string batchID = "";
        private List<BallotData> data = null;
        private Dictionary<string, int> precinct_map = null;

        private Dictionary<int, Color> colorDic = null;
        public Renderer(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
        public void setRenderSize(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
        public int getDataCount()
        {
            if (this.data == null) return 0;
            else return this.data.Count;
        }
        public List<BallotData> getData()
        {
            return this.data;
        }
        public int getPrecinctCount()
        {
            if (this.precinct_map == null) return 0;
            else return this.precinct_map.Count();
        }
        public void setChatData(List<BallotData> data, Dictionary<string, int> precinct_map, string batchID)
        {
            this.data = data;
            this.batchID = batchID;
            this.precinct_map = precinct_map;
            var sorted_precinct_map = precinct_map.OrderByDescending(o => o.Value);

            colorDic = new Dictionary<int, Color>();
            Random rnd = new Random();
            for (int i = 0; i < sorted_precinct_map.Count(); i++)
            {
                int count = sorted_precinct_map.ElementAt(i).Value;
                if (!colorDic.ContainsKey(count))
                {
                    colorDic[count] = Color.FromArgb(rnd.Next(50, 255), rnd.Next(50, 255), rnd.Next(50, 255));
                }
            }

        }
        public Bitmap getBmp()
        {
            return this.bmp;
        }

        public Point convertCoord(Point a)
        {
            double px = height / totHeight;

            Point res = new Point();
            res.X = (int)((a.X + 20) * px);
            res.Y = (int)((1000 - a.Y) * px);
            return res;
        }
        public PointF convertCoord(PointF p)
        {
            double px = height / totHeight;
            PointF res = new PointF();
            res.X = (int)((p.X + 20) * px);
            res.Y = (int)((1000 - p.Y) * px);
            return res;
        }
        public void drawCenteredString_withBorder(string content, Rectangle rect, Brush brush, Font font, Color borderColor)
        {

            //using (Font font1 = new Font("Arial", fontSize, FontStyle.Bold, GraphicsUnit.Point))

            // Create a StringFormat object with the each line of text, and the block
            // of text centered on the page.
            double px = height / totHeight;
            rect.Location = convertCoord(rect.Location);
            rect.Width = (int)(px * rect.Width);
            rect.Height = (int)(px * rect.Height);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            // Draw the text and the surrounding rectangle.
            gfx.DrawString(content, font, brush, rect, stringFormat);

            Pen borderPen = new Pen(new SolidBrush(borderColor), 2);
            gfx.DrawRectangle(borderPen, rect);
            borderPen.Dispose();
        }
        public void drawLine(Point p1, Point p2, Color color, int linethickness = 1)
        {
            if (color == null)
                color = Color.Gray;

            p1 = convertCoord(p1);
            p2 = convertCoord(p2);
            gfx.DrawLine(new Pen(color, linethickness), p1, p2);

        }
        public void drawCenteredString(string content, Rectangle rect, Brush brush, Font font)
        {

            //using (Font font1 = new Font("Arial", fontSize, FontStyle.Bold, GraphicsUnit.Point))

            // Create a StringFormat object with the each line of text, and the block
            // of text centered on the page.
            double px = height / totHeight;
            rect.Location = convertCoord(rect.Location);
            rect.Width = (int)(px * rect.Width);
            rect.Height = (int)(px * rect.Height);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            // Draw the text and the surrounding rectangle.
            gfx.DrawString(content, font, brush, rect, stringFormat);
            //gfx.DrawRectangle(Pens.Black, rect);

        }
        public void drawString(Font font, Color brushColor, string content, Point o)
        {
            o = convertCoord(o);
            SolidBrush drawBrush = new SolidBrush(brushColor);
            gfx.DrawString(content, font, drawBrush, o.X, o.Y);
        }

        public void drawString(Point o, string content, int font = 15)
        {

            o = convertCoord(o);

            // Create font and brush.
            Font drawFont = new Font("Arial", font);
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            gfx.DrawString(content, drawFont, drawBrush, o.X, o.Y);

        }
        public void drawString(Color color, Point o, string content, int font = 15)
        {

            o = convertCoord(o);

            // Create font and brush.
            Font drawFont = new Font("Arial", font);
            SolidBrush drawBrush = new SolidBrush(color);

            gfx.DrawString(content, drawFont, drawBrush, o.X, o.Y);

            drawFont.Dispose();
            drawBrush.Dispose();

        }
        public void fillRectangle(Color color, Rectangle rect)
        {
            rect.Location = convertCoord(rect.Location);
            double px = height / totHeight;
            rect.Width = (int)(rect.Width * px);
            rect.Height = (int)(rect.Height * px);

            Brush brush = new SolidBrush(color);
            gfx.FillRectangle(brush, rect);
            brush.Dispose();

        }
        public void drawRectangle(Pen pen, Rectangle rect)
        {
            rect.Location = convertCoord(rect.Location);
            double px = height / totHeight;
            rect.Width = (int)(rect.Width * px);
            rect.Height = (int)(rect.Height * px);
            gfx.DrawRectangle(pen, rect);
        }
        public void drawPolygon(Pen pen, PointF[] curvePoints)
        {
            for (int i = 0; i < curvePoints.Length; i++)
            {
                curvePoints[i] = convertCoord(curvePoints[i]);
            }
            gfx.DrawPolygon(pen, curvePoints);
        }
        public void drawPolygon(Pen pen, Point[] curvePoints)
        {
            for (int i = 0; i < curvePoints.Length; i++)
            {
                curvePoints[i] = convertCoord(curvePoints[i]);
            }
            gfx.DrawPolygon(pen, curvePoints);
        }
        public void fillPolygon(Brush brush, PointF[] curvePoints)
        {
            for (int i = 0; i < curvePoints.Length; i++)
            {
                curvePoints[i] = convertCoord(curvePoints[i]);
            }
            gfx.FillPolygon(brush, curvePoints);
        }
        public void fillPolygon(Brush brush, Point[] curvePoints)
        {
            for (int i = 0; i < curvePoints.Length; i++)
            {
                curvePoints[i] = convertCoord(curvePoints[i]);
            }
            gfx.FillPolygon(brush, curvePoints);
        }
        public void drawImg(Image img, Point o, Size size)
        {
            double px = height / totHeight;
            o = convertCoord(o);
            Rectangle rect = new Rectangle(o, new Size((int)(size.Width * px), (int)(size.Height * px)));
            gfx.DrawImage(img, rect);

        }

        public void drawPie(Color color, Point o, Size size, float startAngle, float sweepAngle, string content = "")
        {
            // Create location and size of ellipse.
            double px = height / totHeight;
            size.Width = (int)(size.Width * px);
            size.Height = (int)(size.Height * px);

            Rectangle rect = new Rectangle(convertCoord(o), size);
            // Draw pie to screen.            
            Brush grayBrush = new SolidBrush(color);
            gfx.FillPie(grayBrush, rect, startAngle, sweepAngle);

            if (sweepAngle >= 90 )
            {
                o.X += size.Width / 2;
                o.Y -= size.Height / 2;
                float radius = size.Width * 0.3f;
                o.X += (int)(radius * Math.Cos(Helper.DegreesToRadians(startAngle + sweepAngle / 2))) - 30;
                o.Y -= (int)(radius * Math.Sin(Helper.DegreesToRadians(startAngle + sweepAngle / 2)));
                content += "\n" + string.Format("{0:F}%", sweepAngle * 100.0f / 360.0f);

                //drawString(o, content, 9);
                //drawString(o, string.Format("{0:F}%", sweepAngle * 100.0f / 360.0f), 9);

                drawString(new Font("Arial", 25, FontStyle.Bold, GraphicsUnit.Point), Color.White, string.Format("{0}%", Math.Round(sweepAngle * 100 / 360, 1)), o);
            }

        }
        public void drawFilledCircle(Brush brush, Point o, Size size)
        {
            double px = height / totHeight;
            size.Width = (int)(size.Width * px);
            size.Height = (int)(size.Height * px);

            Rectangle rect = new Rectangle(convertCoord(o), size);

            gfx.FillEllipse(brush, rect);
        }
        public void drawRoundedRectangle(Pen pen, Rectangle rect, int borderRadius)
        {

            using (GraphicsPath path = RoundedRect(rect, borderRadius))
            {
                SmoothingMode initMode = gfx.SmoothingMode;
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                gfx.DrawPath(pen, path);
                gfx.SmoothingMode = initMode;
            }
        }
        public GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {

            //Convert Rectangle Coord
            bounds.Location = convertCoord(bounds.Location);
            double px = height / totHeight;
            bounds.Width = (int)(bounds.Width * px);
            bounds.Height = (int)(bounds.Height * px);


            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        public void draw()
        {
            if (bmp == null)
                bmp = new Bitmap(width, height);
            else
            {
                if (bmp.Width != width || bmp.Height != height)
                {
                    bmp.Dispose();
                    bmp = new Bitmap(width, height);

                    gfx.Dispose();
                    gfx = Graphics.FromImage(bmp);
                    gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                }
            }

            if (gfx == null)
            {
                gfx = Graphics.FromImage(bmp);
                gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            }
            else
            {
                gfx.Clear(Color.Transparent);
            }


            Image logoImg = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "logo.png"));
            Image yellow_warningImg = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "yellow_warning.png"));
            
            Font textFont = new Font("Arial", 18, FontStyle.Bold, GraphicsUnit.Point);
            Font textNumberFont = new Font("Arial", 15, FontStyle.Bold, GraphicsUnit.Point);
            Font titleNumberFont = new Font("Arial", 30, FontStyle.Bold, GraphicsUnit.Point);
            Font titleFont = new Font("Arial", 25, FontStyle.Bold, GraphicsUnit.Point);
            Font descFont = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Point);
            Font bold_descFont = new Font("Arial", 10, FontStyle.Bold, GraphicsUnit.Point);

            ///////////////////////////////////
            ///First Arrow Polygon////////////////////////
            int arrow_gap = 105;
            Color color = Color.FromArgb(153, 204, 33);
            SolidBrush brush = new SolidBrush(color);
            Point point1 = new Point(0, 980);
            Point point2 = new Point(100, 980);
            Point point3 = new Point(115, 965);
            Point point4 = new Point(100, 950);
            Point point5 = new Point(0, 950);
            Point point6 = new Point(15, 965);

            Point[] vertex = new Point[] { point1, point2, point3, point4, point5, point6 };
            Point[] tmp_vertex = new Point[6];
            vertex.CopyTo(tmp_vertex, 0);
            fillPolygon(brush, tmp_vertex);


            /////////////////Second Arrow Polygon/////////////////////////
            ///
            color = Color.FromArgb(240, 196, 11);
            brush = new SolidBrush(color);
            for (int i = 0; i< vertex.Length; i++)
            {
                vertex[i].X = vertex[i].X + arrow_gap;
            }
            vertex.CopyTo(tmp_vertex, 0);
            fillPolygon(brush, tmp_vertex);

            /////////////////Third Arrow Polygon/////////////////////////
            ///
            color = Color.FromArgb(35, 178, 237);
            brush = new SolidBrush(color);
            for (int i = 0; i < vertex.Length; i++)
            {
                vertex[i].X = vertex[i].X + arrow_gap;
            }
            vertex.CopyTo(tmp_vertex, 0);
            fillPolygon(brush, tmp_vertex);

            /////////////////Fourth Arrow Polygon/////////////////////////
            ///
            color = Color.FromArgb(240, 55, 82);
            brush = new SolidBrush(color);
            for (int i = 0; i < vertex.Length; i++)
            {
                vertex[i].X = vertex[i].X + arrow_gap;
            }
            vertex.CopyTo(tmp_vertex, 0);
            fillPolygon(brush, tmp_vertex);

            /////////////////Fifth Arrow Polygon/////////////////////////
            ///
            color = Color.FromArgb(116, 56, 145);
            brush = new SolidBrush(color);
            for (int i = 0; i < vertex.Length; i++)
            {
                vertex[i].X = vertex[i].X + arrow_gap;
            }
            vertex.CopyTo(tmp_vertex, 0);
            fillPolygon(brush, tmp_vertex);



            //////Draw title////////////////////
            drawCenteredString("PKAD CHROMATICS REPORT", new Rectangle(0, 950, 500, 50), Brushes.Black, textFont);


            if (this.data == null) return;
            //////Draw batchID////////////////////
            ///
            color = Color.FromArgb(199, 208, 216);
            drawRectangle(new Pen(color, 10), new Rectangle(800, 980, 800, 60));
            drawCenteredString(batchID, new Rectangle(800, 980, 800, 60), Brushes.Black, textNumberFont);

            /////////////////////////////////////////////////////////////////////////  
            ///
            
            Color white_color = Color.FromArgb(255, 235, 235);
            Color purple_color = Color.FromArgb(116, 56, 145);
            Color green_color = Color.FromArgb(39, 174, 96);
            Color gold_color = Color.FromArgb(255, 222, 89);
            Color platinum_color = Color.FromArgb(199, 208, 216);
            Color gray_color = Color.FromArgb(115, 115, 115);
            Color pink_color = Color.FromArgb(255, 102, 196);
            Color olive_color = Color.FromArgb(156, 168, 105);

            Dictionary<string, int> color_map = new Dictionary<string, int>();
            foreach(var item in data)
            {
                //if (item.is_color.ToUpper() != "COLOR") continue;
                if (color_map.ContainsKey(item.color)) color_map[item.color]++;
                else color_map[item.color] = 1;
            }
            int max_file_count = 0;
            foreach(var item in color_map)
            {
                if (item.Key == "NA" || string.IsNullOrEmpty(item.Key.Trim())) continue;
                if (max_file_count < item.Value) max_file_count = item.Value;
            }
            double px_perfile = 300 / (double)max_file_count;

            int basex = 50, gap = 100, basey = 480, index =0 ;
            foreach(var item in color_map)
            {
                if (item.Key == "NA" || string.IsNullOrEmpty(item.Key.Trim())) continue;
                switch (item.Key)
                {

                    case "WHITE":
                        color = white_color;
                        break;
                    case "PURPLE":
                        color = purple_color;
                        break;
                    case "GREEN":
                        color = green_color;
                        break;
                    case "GOLD":
                        color = gold_color;
                        break;
                    case "PLATINUM":
                        color = platinum_color;
                        break;
                    case "GRAY":
                        color = gray_color;
                        break;
                    case "PINK":
                        color = pink_color;
                        break;
                    case "OLIVE":
                        color = olive_color;
                        break;
                    default:
                        color = white_color;
                        break;
                }
                int h = (int)(px_perfile * item.Value);
                Rectangle rect = new Rectangle(basex + index * gap, basey + h, gap - 20, h);
                fillRectangle(color, rect);                
                drawCenteredString(string.Format("{0}\n{1:N2}%", item.Key, item.Value * 100 / (double)data.Count()), new Rectangle(basex + index * gap - 10, basey + h + 50, gap , 50), Brushes.Black, bold_descFont);
                drawCenteredString(item.Value.ToString(), new Rectangle(basex + index * gap - 10, basey + h + 90, gap, 50), Brushes.Black, textFont);
                index++;                
            }
            drawCenteredString("CHROMA-ROTATION", new Rectangle(50, 480, Math.Max(index * gap, 300), 50), Brushes.Black, textFont);

            ///////////////////////Draw Yellow Warning Area///////////////////////////

            drawImg(yellow_warningImg, new Point(1300, 830), new Size(350, 350));
            drawCenteredString("PRECINCTS", new Rectangle(1300, 480, 350, 50), Brushes.Black, textFont);
            int precinct_count = precinct_map.Count();
            if (precinct_map.ContainsKey("NA")) precinct_count--;
            if (precinct_map.ContainsKey("")) precinct_count--;

            drawCenteredString("REPORTING", new Rectangle(1300, 630, 350, 50), Brushes.Black, textFont);
            drawCenteredString(precinct_count.ToString(), new Rectangle(1300, 730, 350, 150), Brushes.Black, new Font("Arial", 50, FontStyle.Bold, GraphicsUnit.Point));
            /////////////////////////Draw Rectangle Areas////////////////////////////////////////////////////

            color = Color.FromArgb(255, 205, 205);
            Color color2 = Color.FromArgb(255, 227, 227);
            string[] rec_contents = new string[8] {
                "1,081,393\n51.73%", "478,998\n22.91%", "115,232\n5.51%", "13,693\n.655%",
                "5,602\n.268%", "2,272\n.108%", "581\n.027%", "7\n.00033%"
            };
            for (int i =0; i< 8; i++)
            {
                if ( i < 6 ) fillRectangle(color, new Rectangle(50 + i * 190, 200, 170, 80));
                else fillRectangle(color2, new Rectangle(50 + i * 190, 200, 170, 80));

                drawCenteredString(rec_contents[i], new Rectangle(50 + i * 190, 200, 170, 80), Brushes.Black, textFont);

                if (i < 6)  fillRectangle(color, new Rectangle(50 + i * 190, 300, 170, 80));
                else  fillRectangle(color2, new Rectangle(50 + i * 190, 300, 170, 80));


            }
            rec_contents[0] = "WHITE";
            rec_contents[1] = "PURPLE";
            rec_contents[2] = "GREEN";
            rec_contents[3] = "GOLD";
            rec_contents[4] = "PLATINUM";
            rec_contents[5] = "GRAY";
            rec_contents[6] = "PINK";
            rec_contents[7] = "OLIVE";

            int[] percentBaseNum = new int[8]
            {
                1081393, 478998, 115232, 13693, 5602, 2272, 581, 7
            };
            Color background_color = Color.Black;
            Color letter_color = Color.Gray;
            for (int i = 0; i< 8; i++)
            {
                switch (rec_contents[i])
                {

                    case "WHITE":
                        background_color = white_color;
                        letter_color = Color.Black;
                        break;
                    case "PURPLE":
                        background_color = purple_color;
                        letter_color = Color.White;
                        break;
                    case "GREEN":
                        background_color = green_color;
                        letter_color = Color.White;
                        break;
                    case "GOLD":
                        background_color = gold_color;
                        letter_color = Color.Black;
                        break;
                    case "PLATINUM":
                        background_color = platinum_color;
                        letter_color = Color.Black;
                        break;
                    case "GRAY":
                        background_color = gray_color;
                        letter_color = Color.White;
                        break;
                    case "PINK":
                        background_color = pink_color;
                        letter_color = Color.Black;
                        break;
                    case "OLIVE":
                        background_color = olive_color;
                        letter_color = Color.White;
                        break;
                    default:
                        background_color = white_color;
                        letter_color = Color.Black;
                        break;
                }
                fillRectangle(background_color, new Rectangle(50 + i * 190, 400, 170, 80));
                drawCenteredString(rec_contents[i], new Rectangle(50 + i * 190, 400, 170, 80), new SolidBrush(letter_color), textFont);
                string content = "";
                if (color_map.ContainsKey(rec_contents[i]))
                {
                    content = string.Format("{0:N5}%", color_map[rec_contents[i]] * 100 / (double)percentBaseNum[i]);
                }
                else if ( rec_contents[i] == "WHITE") {
                    content = string.Format("{0:N5}%", data.Where(item => item.is_color != "NA" && item.color == "NA").Count() * 100 / (double)percentBaseNum[i]);                    
                } else  content = "NA%";

                drawCenteredString(content, new Rectangle(50 + i * 190, 300, 170, 80), Brushes.Black, textFont);
            }

            /////////////////////Draw Pie//////////////////////////////////////

            //True color
            int true_color_cnt = 0, duplicated_cnt = 0, white_cnt =0;
            float startAngle = 270, sweepAngle = 0, percent = 0;

            foreach(var item in color_map)
            {
                if (item.Key != "NA") true_color_cnt += item.Value;
            }
            sweepAngle = true_color_cnt * 360 / (float)data.Count();
            percent = true_color_cnt * 100 / (float)data.Count();
            drawPie(Color.FromArgb(100, 100, 100), new Point(620, 830), new Size(350, 350), startAngle, sweepAngle, "TRUE COLOR");
            fillRectangle(Color.FromArgb(100, 100, 100), new Rectangle(1000, 750, 25, 25));
            //drawString(new Point(1030, 730),  string.Format("TRUE COLOR {0:N1}%", percent), 13);
            drawCenteredString(string.Format("TRUE COLOR {0:N1}%", percent), new Rectangle(1050, 730, 200, 20), Brushes.Black, descFont);
            drawCenteredString(true_color_cnt.ToString(), new Rectangle(1050, 770, 200, 50), Brushes.Black,  titleFont);
            startAngle += sweepAngle;


            duplicated_cnt = data.Where(item => item.is_color == "NA" && item.color == "NA").Count();
            sweepAngle = duplicated_cnt * 360 / (float)data.Count();
            percent = duplicated_cnt * 100 / (float)data.Count();
            drawPie(Color.FromArgb(136, 136, 136), new Point(620, 830), new Size(350, 350), startAngle, sweepAngle, "DUPLICATED");
            fillRectangle(Color.FromArgb(136, 136, 136), new Rectangle(1000, 650, 25, 25));
            //drawString(new Point(1030, 630), string.Format("DUPLICATED {0:N1}%", percent), 13);
            drawCenteredString(string.Format("DUPLICATED {0:N1}%", percent), new Rectangle(1050, 630, 200, 20), Brushes.Black, descFont);
            drawCenteredString(duplicated_cnt.ToString(), new Rectangle(1050, 670, 200, 50), Brushes.Black, titleFont);
            startAngle += sweepAngle;

            white_cnt = data.Where(item => item.is_color != "NA" && item.color == "NA").Count();
            sweepAngle = white_cnt * 360 / (float)data.Count();
            percent = white_cnt * 100 / (float)data.Count();
            drawPie(Color.FromArgb(173, 173, 173), new Point(620, 830), new Size(350, 350), startAngle, sweepAngle, "WHITE");
            fillRectangle(Color.FromArgb(173, 173, 173), new Rectangle(1000, 550, 25, 25));
            //drawString(new Point(1030, 530), string.Format("WHITE {0:N1}%", percent), 13);
            drawCenteredString(string.Format("WHITE {0:N1}%", percent), new Rectangle(1050, 530, 200, 20), Brushes.Black, descFont);
            drawCenteredString(white_cnt.ToString(), new Rectangle(1050, 570, 200, 50), Brushes.Black, titleFont);
            startAngle += sweepAngle;


            /////////////Draw Total number of files/////////////////////////////
            drawCenteredString(data.Count.ToString(), new Rectangle(1570, 250, 100, 100), Brushes.Black, titleNumberFont);
            //////////////////////////////////////////////////
            //////////////Draw Logo /////////////////////////
            ///
            drawImg(logoImg, new Point(0, 80), new Size(200, 70));
            drawString(descFont, Color.Black, "© 2021 Tesla Laboratories, llc & JHP", new Point(1300, 50));
        }
    }
}
