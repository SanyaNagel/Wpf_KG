using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Wpf_KG
{
    class ObjParser
    {
        public ObjParser()
        {

        }

        public List<Edge> WriteObject(String name)
        {
            switch (name)
            {
                case "Куб": return DrawObject(File.ReadAllLines("cube.obj"));
                    
                case "Призма": return DrawObject(File.ReadAllLines("prism.obj"));
                
                default: return null;
            }
        }

        public List<Edge> DrawObject(string[] lines)
        {
            List<Edge> listEdge = new List<Edge>();
            string color = "";
            foreach (string line in lines)
            {
                double buf;
                double[] point = new double[3];
                string[] word = line.Split(' ');
                Point3D p1 = new Point3D(), p2 = new Point3D();
                bool firstPoint = true;
                for(int i = 0; i < word.Length; i++)
                {
                    word[i] = word[i].Replace('.', ',');
                    if (double.TryParse(word[i], out buf))
                    {   //Если это число
                        if (i - 2 > 0)
                            point[i - 3] = buf; //если это вторая точка
                        else
                            point[i] = buf; // если это первая точка
                        if (i == 2 || (i - 3) == 2)
                        {
                            if (firstPoint == true)
                            {
                                p1 = new Point3D(point[0], point[1], point[2]);
                                firstPoint = false;
                            }
                            else
                            {
                                p2 = new Point3D(point[0], point[1], point[2]);
                                firstPoint = true;
                            }
                        }
                    }
                    else
                    {   //Иначе это цвет
                        color = word[i];
                    }
                }
                if (word.Length > 1)
                    listEdge.Add(new Edge(p1,p2, "#FFFFEB00"));
            }
            return listEdge;
        }


    }
}
