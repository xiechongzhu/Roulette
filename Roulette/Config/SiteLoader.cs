using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Roulette.Config
{
    class SiteInfo
    {
        public String Name { get; set; }
        public String Url { get; set; }
        public SupplierType supplierType { get; set; }
    }
    class SiteLoader
    {
        private static SiteLoader _instance = new SiteLoader();
        private List<SiteInfo> siteList = new List<SiteInfo>();
        public static SiteLoader GetInstance()
        {
            return _instance;
        }

        public bool LoadConfig(ref String errMsg)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("Site.xml");       //加载Xml文件
                XmlElement rootElem = doc.DocumentElement;    //获取根节点
                XmlNodeList supplierNodes = rootElem.GetElementsByTagName("Supplier"); //获取Supplier子节点集合
                foreach (XmlNode node in supplierNodes)
                {
                    SiteInfo siteInfo = new SiteInfo();
                    siteInfo.Name = ((XmlElement)node).GetAttribute("Name");
                    siteInfo.Url = ((XmlElement)node).GetAttribute("Url");
                    switch(((XmlElement)node).GetAttribute("Type"))
                    {
                        case "AG":
                            siteInfo.supplierType = SupplierType.SUPPLIER_AG;
                            break;
                        default:
                            siteInfo.supplierType = SupplierType.SUPPLIER_UNKNOW;
                            break;
                    }
                    siteList.Add(siteInfo);
                }
            }
            catch(Exception e)
            {
                errMsg = e.Message;
                return false;
            }
            return true;
        }

        public List<SiteInfo> GetSiteList()
        {
            return siteList;
        }

        public String GetSiteUrl(String name)
        {
            foreach(SiteInfo siteInfo in siteList)
            {
                if(name.Equals(siteInfo.Name))
                {
                    return siteInfo.Url;
                }
            }
            return null;
        }

        public SupplierType GetSupplierType(String name)
        {
            foreach (SiteInfo siteInfo in siteList)
            {
                if (name.Equals(siteInfo.Name))
                {
                    return siteInfo.supplierType;
                }
            }
            return SupplierType.SUPPLIER_UNKNOW;
        }
    }
}
