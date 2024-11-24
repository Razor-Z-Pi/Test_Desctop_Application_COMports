using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComPortsDesctop
{
    public class AdresId
    {
        private static List<int> id = new List<int>();
        private static List<int> Cloneid = new List<int>();

        // Метод для добавления идентификатора в список
        public static void AddId(int id_)
        {
            if (!id.Contains(id_))
            {
                id.Add(id_);
            }
        }

        public static void AddIdClone(int id_)
        {
            if (!Cloneid.Contains(id_))
            {
                Cloneid.Add(id_);
            }
        }

        // Метод для получения списка идентификаторов
        public static List<int> GetIds()
        {
            return id; // Возвращаем копию списка
        }

        // Метод для получения списка идентификаторов клона
        public static List<int> GetIdsClone()
        {
            return Cloneid;
        }

        // Метод для очистки списка идентификаторов
        public static void ClearIds()
        {
            id.Clear();
        }
    }
}
