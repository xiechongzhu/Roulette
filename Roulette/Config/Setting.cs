using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Roulette
{
    [Serializable]
    public class Setting
    {
        public static String settingFile = "./setting.xml";

        public int diff;
        public int win;
        public String tableName;

        public bool Save()
        {
            FileStream file = File.Create(settingFile);
            try
            {
                XmlSerializer wirter = new XmlSerializer(typeof(Setting));
                wirter.Serialize(file, this);
            }
            catch (Exception)
            {
                file.Close();
                return false;
            }
            file.Close();
            return true;
        }

        static public Setting Load()
        {
            Setting setting = null;
            StreamReader file = null;
            try
            {
                file = new StreamReader(settingFile);
                XmlSerializer reader = new XmlSerializer(typeof(Setting));
                setting = (Setting)reader.Deserialize(file);
                file.Close();
            }
            catch (Exception)
            {
                file?.Close();
                return null;
            }
            return setting;
        }
    }
}
