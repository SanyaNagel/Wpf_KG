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
        public List<List<Edge>> listListEdge = new List<List<Edge>>();
        public Point3D internalPoint = new Point3D(0,0,0);


        public double movingX = 0.0;
        public double movingY = 0.0;
        public double movingZ = 0.0;
        
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
                {movingX, movingY, movingZ, 1}}); 
        }

        //Матрица масштабирования
        public Matrix<double> scale_matr()
        {
            return Matrix<double>.Build.DenseOfArray(new double[,] {
                {scale + 1, 0, 0, 0},
                {0, scale + 1, 0, 0},
                {0, 0, scale + 1, 0},
                {0, 0, 0, scale + 1 }
            });
        }

        public double rotation = 2.0;

        //Проверяем можно ли отрисовывать грань, состоящую из списка рёбер
        public bool isDraw(List<Edge> list)
        {
            //Ищем произведение двух векторов
            Edge e1 = list.First();
            Edge e2 = list.Last();
            Vector<double> v1 = Vector<double>.Build.DenseOfArray(new double[] { e1.P2.X - e1.P1.X, e1.P2.Y - e1.P1.Y, e1.P2.Z - e1.P1.Z});
            Vector<double> v2 = Vector<double>.Build.DenseOfArray(new double[] { e2.P2.X - e2.P1.X, e2.P2.Y - e2.P1.Y, e2.P2.Z - e2.P1.Z });
            
            //Выполняем векторное произведение
            Vector<double> vectorNormal = Vector<double>.Build.DenseOfArray(new double[] { v1[1] * v2[2] - v1[2]*v2[1], v1[2] * v2[0] - v1[0]*v2[2], v1[0]*v2[1]-v1[1]*v2[0]});

            //Теперь нужно определить вектор наружний или внутренний
            //Откладываем вектор от внутренней точки
            Vector<double> vP = Vector<double>.Build.DenseOfArray(new double[] { e1.P2.X - internalPoint.X, e1.P2.Y - internalPoint.Y, e1.P2.Z - internalPoint.Z });

            //Выполняем сколярное произведение точки на полученный вектор
            double v1vP = vectorNormal * vP;
            if(v1vP < 0)    //Если вектор направлен внуторь
            {               //То делаем обратно направление вектора
                vectorNormal *= -1;
            }

            //Находим угол между проекцией наблюдения и вектором нормали
            double vNormalVProection = vectorNormal * vProection;
            if (vNormalVProection < 0)//Если угол отрицател
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public Vector<double> vProection = Vector<double>.Build.DenseOfArray(new double[] { 0,0,100});

        public void setRotation(double r)
        {
            rotation = r;
        }

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

        public Matrix<double> projection()
        {
            return Matrix<double>.Build.DenseOfArray(new double[,] {
            {1, 0, 0, 0 },
            {0, 1, 0, 0 },
            {Math.Cos(Math.PI/4), Math.Cos(Math.PI/4), 0, 0},
            {0, 0, 0, 1 }
            });
        }

        public void Command(String name)
        {
            switch (name)
            {
                case "Перемещение": Transformation(moving_matr()); break;

                case "Вращение по X": Transformation(rotation_matrix_X()); break;

                case "Вращение по Y": Transformation(rotation_matrix_Y()); break;

                case "Вращение по Z": Transformation(rotation_matrix_Z()); break;

                case "Масштабирование": Transformation(scale_matr()); break;

                default: break;
            }
        }

        public void DrawAllListEdge(List<List<Edge>> listList)
        {
            if (listList == null)
                return;

            foreach (List<Edge> listEdg in listList)   //Цикл по всем граням
            {
                if (isDraw(listEdg) == true)   //Если грань видимая то отрисовываем
                    DrawAllEdge(listEdg);
            }
        }

        public void DrawAllEdge(List<Edge> list)
        {
            if (list == null)
                return;
            //List<Edge> lClone = listClone(list);
            //Transformation(projection());
            foreach (Edge edg in list)
            {
                mw.CanvasArea.Children.Add(edg.line);
            }
        }

        /*
        public List<Edge> listClone(List<Edge> list)
        {
            List<Edge> listClone = new List<Edge>();
            foreach(Edge edg in list)
            {
                listClone.Add(new Edge(edg.P1,edg.P2,edg.Color));
            }
            return listClone;
        }
        */
        
        public void Transformation(Matrix<double> M)
        {
            mw.RedrawScene();
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
            //DrawAllEdge(listEdge);
            DrawAllListEdge(listListEdge);
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
