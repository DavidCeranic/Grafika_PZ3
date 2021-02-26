using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PZ3.Models
{
    public class MyLine
    {
        public Object p1; //zbog bojanja
        public Object p2; //zbog bojanja
        public LineEntity line;
        public GeometryModel3D model;
        public bool IsSelected;

        public MyLine(Object p1, Object p2, LineEntity line, GeometryModel3D model)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.line = line;
            this.model = model;
        }

        public MyLine()
        {
        }
    }
}
