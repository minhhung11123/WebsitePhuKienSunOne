using System.Xml;
using System;

namespace WebsitePhuKienSunOne.Extension
{
    public class ExchangeRate
    {
        public static decimal GetUSDBuyRate()
        {
            string url = "https://portal.vietcombank.com.vn/Usercontrols/TVPortal.TyGia/pXML.aspx?b=10";

            using (XmlReader reader = XmlReader.Create(url))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Exrate" && reader.GetAttribute("CurrencyCode") == "USD")
                    {
                        string strBuyRate = reader.GetAttribute("Buy");
                        string fixBuyRate = strBuyRate.Replace(",", "").Replace(".00", "");
                        decimal buyRate = decimal.Parse(fixBuyRate);
                        return buyRate;
                    }
                }
            }
            throw new Exception("Could not retrieve USD exchange rate from Vietcombank API");
        }
    }
}
