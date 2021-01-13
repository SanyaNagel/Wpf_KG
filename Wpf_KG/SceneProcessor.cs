using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using MathNet.Numerics.LinearAlgebra;

namespace Wpf_KG
{
    class SceneProcessor
    {
        public MainWindow mw;
        public List<Edge> listEdge = new List<Edge>();
        Point3D internalPoint = new Point3D();


        public double moving = 100.0;
        public double scale = 4.0;

        public void setScale(double scal)
        {
            scale = scal;
        }

        //Перемещение по диагонали
        private Matrix<double> moving_matr() { 
            return Matrix<double>.Build.DenseOfArray(new double[,] {
                {1, 0, 0, 0},
                {0, 1, 0, 0},
                {0, 0, 1, 0},
                {moving, moving, moving, 1 }}); 
        }

        //Матрица масштабирования
        public Matrix<double> scale_matr()
        {
            return Matrix<double>.Build.DenseOfArray(new double[,] {
                {scale, 0, 0, 0},
                {0, scale, 0, 0},
                {0, 0, scale, 0},
                {0, 0, 0, scale }
            });
        }

        public double rotation = 3.14;
        public Matrix<double> rotation_matrix_X()
        {
            return Matrix<double>.Build.DenseOfArray(new double[,] {
                {1, 0, 0, 0},
                {0, Math.Cos(rotation), Math.Sin(rotation), 0},
                {0, -Math.Sin(rotation), Math.Cos(rotation), 0 },
                {0, 0, 0, 1 }
            });
        }

        public Matrix<double> rotation_matrix_Y()
        {
            return Matrix<double>.Build.DenseOfArray(new double[,] {
                {Math.Cos(rotation), 0, -Math.Sin(rotation), 0},
                {0, 1, 0, 0 },
                {Math.Sin(rotation), 0, Math.Cos(rotation), 0 },
                {0, 0, 0, 1 }
            });
        }

        public Matrix<double> rotation_matrix_Z()
        {
            return Matrix<double>.Build.DenseOfArray(new double[,] {
            {Math.Cos(rotation), Math.Sin(rotation), 0, 0},
            {-Math.Sin(rotation), Math.Cos(rotation), 0, 0 },
            {0, 0, 1, 0 },
            {0, 0, 0, 1 }
            });
        }

        public void Command(String name, bool auto = false)
        {
            switch (name)
            {
                case "Перемещение": Transformation(moving_matr()); break;

                case "Вращение": break;

                case "Масштабирование": Transformation(scale_matr(), auto); break;

                default: break;
            }
        }

        public void DrawAllEdge(List<Edge> list)
        {
            if (list == null)
                return;
            foreach( Edge edg in list)
            {
                mw.CanvasArea.Children.Add(edg.line);
            }
        }

        public List<Edge> cloneList(List<Edge> listClone)
        {
            List<Edge> newList = new List<Edge>();
            foreach(Edge edg in listClone)
            {
                newList.Add(new Edge(edg.P1, edg.P2, edg.Color));
            }
            return newList;
        }

        public void Transformation(Matrix<double> M, bool clone = false)
        {
            mw.RedrawScene();
            if (clone == true)
            {
                List<Edge> listClone = cloneList(listEdge);
                for (int i = 0; i < listClone.Count; i++)
                {
                    Vector<double> p1Coords = Vector<double>.Build.DenseOfArray(new double[] { listClone[i].P1.X, listClone[i].P1.Y, listClone[i].P1.Z, 1 });
                    Vector<double> p2Coords = Vector<double>.Build.DenseOfArray(new double[] { listClone[i].P2.X, listClone[i].P2.Y, listClone[i].P2.Z, 1 });
                    p1Coords = p1Coords * M;
                    p2Coords = p2Coords * M;
                    listClone[i].changeCoords(p1Coords[0], p1Coords[1], p1Coords[2], p2Coords[0], p2Coords[1], p2Coords[2]);
                }
                Vector<double> oCoords = Vector<double>.Build.DenseOfArray(new double[] { internalPoint.X, internalPoint.Y, internalPoint.Z, 1 });
                oCoords = oCoords * M;
                internalPoint.X = oCoords[0];
                internalPoint.Y = oCoords[1];
                internalPoint.Z = oCoords[2];
                DrawAllEdge(listClone);
            }
            else
            {
                for (int i = 0; i < listEdge.Count; i++)
                {
                    Vector<double> p1Coords = Vector<double>.Build.DenseOfArray(new double[] { listEdge[i].P1.X, listEdge[i].P1.Y, listEdge[i].P1.Z, 1 });
                    Vector<double> p2Coords = Vector<double>.Build.DenseOfArray(new double[] { listEdge[i].P2.X, listEdge[i].P2.Y, listEdge[i].P2.Z, 1 });
                    p1Coords = p1Coords * M;
                    p2Coords = p2Coords * M;
                    listEdge[i].changeCoords(p1Coords[0], p1Coords[1], p1Coords[2], p2Coords[0], p2Coords[1], p2Coords[2]);
                }
                Vector<double> oCoords = Vector<double>.Build.DenseOfArray(new double[] { internalPoint.X, internalPoint.Y, internalPoint.Z, 1 });
                oCoords = oCoords * M;
                internalPoint.X = oCoords[0];
                internalPoint.Y = oCoords[1];
                internalPoint.Z = oCoords[2];
                DrawAllEdge(listEdge);
            }
        }


        public Line DisplayLine(string color, double x1, double y1, double x2, double y2, int width = 3)
        {
            Line line = new Line();
            line.Visibility = Visibility.Visible;
            line.StrokeThickness = width;
            SolidColorBrush brush = (SolidColorBrush)new BrushConverter().ConvertFromString(color);
            line.Stroke = brush;
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
            mw.CanvasArea.Children.Add(line);
            return line;
        }
    }
}
