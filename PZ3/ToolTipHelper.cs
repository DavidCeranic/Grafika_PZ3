using PZ3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ3
{
    public class ToolTipHelper
    {
        public static string ToolTipEntity(List<PowerEntity> entity)
        {
            string val = string.Empty;
            foreach (var item in entity)
            {
                val += "Type: " + GetType(item) + "\n" + "Name: " + item.Name + "\n" + "Id: " + item.Id + "\n" + "X: " + item.X + "\n" + "Y: " + item.Y + "\n";
                if (item is SwitchEntity) val += "Status: " + (item as SwitchEntity).Status + "\n";
            }

            return val;
        }

        static string GetType(PowerEntity entity)
        {
            if (entity is NodeEntity)
                return "Node";
            else if (entity is SubstationEntity)
                return "Substation";
            else if (entity is SwitchEntity)
                return "Switch";
            else return "";
        }
    }
}
