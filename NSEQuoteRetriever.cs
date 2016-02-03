using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Collections;

namespace TestHarness
{
    public class QuoteDatum
    {
        [JsonIgnoreAttribute]
        public string extremeLossMargin { get; set; }
        [JsonIgnoreAttribute]
        public string cm_ffm { get; set; }
        [JsonIgnoreAttribute]
        public string bcStartDate { get; set; }
        [JsonIgnoreAttribute]
        public string change { get; set; }
        [JsonIgnoreAttribute]
        public string buyQuantity3 { get; set; }
        [JsonIgnoreAttribute]
        public string sellPrice1 { get; set; }
        [JsonIgnoreAttribute]
        public string buyQuantity4 { get; set; }
        [JsonIgnoreAttribute]
        public string sellPrice2 { get; set; }
        [JsonIgnoreAttribute]
        public string priceBand { get; set; }
        [JsonIgnoreAttribute]
        public string buyQuantity1 { get; set; }
        [JsonIgnoreAttribute]
        public string deliveryQuantity { get; set; }
        [JsonIgnoreAttribute]
        public string buyQuantity2 { get; set; }
        [JsonIgnoreAttribute]
        public string sellPrice5 { get; set; }
        [JsonIgnoreAttribute]
        public string quantityTraded { get; set; }
        [JsonIgnoreAttribute]
        public string buyQuantity5 { get; set; }
        [JsonIgnoreAttribute]
        public string sellPrice3 { get; set; }
        [JsonIgnoreAttribute]
        public string sellPrice4 { get; set; }
        [JsonIgnoreAttribute]
        public string open { get; set; }
        [JsonIgnoreAttribute]
        public string low52 { get; set; }
        [JsonIgnoreAttribute]
        public string securityVar { get; set; }
        [JsonIgnoreAttribute]
        public string marketType { get; set; }
        [JsonIgnoreAttribute]
        public string pricebandupper { get; set; }
        [JsonIgnoreAttribute]
        public string totalTradedValue { get; set; }
        [JsonIgnoreAttribute]
        public string faceValue { get; set; }
        [JsonIgnoreAttribute]
        public string ndStartDate { get; set; }
        [JsonIgnoreAttribute]
        public string previousClose { get; set; }
        public string symbol { get; set; }
        [JsonIgnoreAttribute]
        public string varMargin { get; set; }
        public string lastPrice { get; set; }
        [JsonIgnoreAttribute]
        public string pChange { get; set; }
        [JsonIgnoreAttribute]
        public string adhocMargin { get; set; }
        [JsonIgnoreAttribute]
        public string companyName { get; set; }
        [JsonIgnoreAttribute]
        public string averagePrice { get; set; }
        [JsonIgnoreAttribute]
        public string secDate { get; set; }
        [JsonIgnoreAttribute]
        public string series { get; set; }
        [JsonIgnoreAttribute]
        public string isinCode { get; set; }
        [JsonIgnoreAttribute]
        public string indexVar { get; set; }
        [JsonIgnoreAttribute]
        public string pricebandlower { get; set; }
        [JsonIgnoreAttribute]
        public string totalBuyQuantity { get; set; }
        [JsonIgnoreAttribute]
        public string high52 { get; set; }
        [JsonIgnoreAttribute]
        public string purpose { get; set; }
        [JsonIgnoreAttribute]
        public string cm_adj_low_dt { get; set; }
        [JsonIgnoreAttribute]
        public string closePrice { get; set; }
        [JsonIgnoreAttribute]
        public bool isExDateFlag { get; set; }
        [JsonIgnoreAttribute]
        public string recordDate { get; set; }
        [JsonIgnoreAttribute]
        public string cm_adj_high_dt { get; set; }
        [JsonIgnoreAttribute]
        public string totalSellQuantity { get; set; }
        [JsonIgnoreAttribute]
        public string dayHigh { get; set; }
        [JsonIgnoreAttribute]
        public string exDate { get; set; }
        [JsonIgnoreAttribute]
        public string sellQuantity5 { get; set; }
        [JsonIgnoreAttribute]
        public string bcEndDate { get; set; }
        [JsonIgnoreAttribute]
        public string css_status_desc { get; set; }
        [JsonIgnoreAttribute]
        public string ndEndDate { get; set; }
        [JsonIgnoreAttribute]
        public string sellQuantity2 { get; set; }
        [JsonIgnoreAttribute]
        public string sellQuantity1 { get; set; }
        [JsonIgnoreAttribute]
        public string buyPrice1 { get; set; }
        [JsonIgnoreAttribute]
        public string sellQuantity4 { get; set; }
        [JsonIgnoreAttribute]
        public string buyPrice2 { get; set; }
        [JsonIgnoreAttribute]
        public string sellQuantity3 { get; set; }
        [JsonIgnoreAttribute]
        public string applicableMargin { get; set; }
        [JsonIgnoreAttribute]
        public string buyPrice4 { get; set; }
        [JsonIgnoreAttribute]
        public string buyPrice3 { get; set; }
        [JsonIgnoreAttribute]
        public string buyPrice5 { get; set; }
        [JsonIgnoreAttribute]
        public string dayLow { get; set; }
        [JsonIgnoreAttribute]
        public string deliveryToTradedQuantity { get; set; }
        [JsonIgnoreAttribute]
        public string totalTradedVolume { get; set; }
    }

    public class QuoteRootObject
    {
        public string lastUpdateTime { get; set; }
        public string tradedDate { get; set; }
        public List<QuoteDatum> data { get; set; }
    }
    public class NSEQuoteRetriever
    {
        public bool DownloadFile(string urlforFile)
        {
            try
            {
                HttpWebRequest myhttpRequest = (HttpWebRequest)WebRequest.Create(urlforFile);
                myhttpRequest.Accept = @"text/html, application/xhtml+xml, */*";
                myhttpRequest.AutomaticDecompression = System.Net.DecompressionMethods.GZip & System.Net.DecompressionMethods.Deflate;
                myhttpRequest.UserAgent = "TestHarness client";

                WebResponse myresponse = myhttpRequest.GetResponse();
                Console.WriteLine(((HttpWebResponse)myresponse).StatusDescription);
                // Get the stream containing content returned by the server.
                Stream dataStream = myresponse.GetResponseStream();
                string filename = @"c:\sundeep_personal\nse.csv.zip";

                byte[] buffer = new byte[1024];
                FileStream outFile = new FileStream(filename, FileMode.Create);

                int bytesRead;
                while ((bytesRead = dataStream.Read(buffer, 0, buffer.Length)) != 0)
                    outFile.Write(buffer, 0, bytesRead);

                // Or using statement instead
                outFile.Close();
                //WebClient Client = new WebClient();
                //Client.DownloadFile(@"http://www.nseindia.com/content/historical/EQUITIES/2016/JAN/cm05JAN2016bhav.csv.zip", @"c:\sundeep_personal\nse.csv.zip");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        } //DownloadFile

        public bool GetLiveQuote(string stocksymbol)
        {
            try
            {
                string urlforFile = @"http://live-nse.herokuapp.com/?symbol=" + stocksymbol;
                HttpWebRequest myhttpRequest = (HttpWebRequest)WebRequest.Create(urlforFile);
                myhttpRequest.Accept = @"text/json, */*";
                myhttpRequest.AutomaticDecompression = System.Net.DecompressionMethods.None;
                myhttpRequest.UserAgent = "TestHarness client";

                WebResponse myresponse = myhttpRequest.GetResponse();
                Console.WriteLine(((HttpWebResponse)myresponse).StatusDescription);
                // Get the stream containing content returned by the server.
                Stream dataStream = myresponse.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                string myjson = reader.ReadToEnd();
                if (System.Text.RegularExpressions.Regex.IsMatch(myjson, ".*Invalid Symbol", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    return false;

                //ConsoleConsole.WriteLine(myjson);
                JsonSerializerSettings jss = new JsonSerializerSettings
                {
                   
                    Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
                    {
                        Console.WriteLine("Error deserializing Json: {0}", errorArgs.ErrorContext.Error.Message);
                        errorArgs.ErrorContext.Handled = true;
                    }
                };
                var x = JsonConvert.DeserializeObject<QuoteRootObject>(myjson, jss);

                foreach (QuoteDatum q in x.data)
                {
                    Console.WriteLine("{0} last trade price: {1}", stocksymbol, q.lastPrice);
                }


            }
            catch (Exception e)
            {
                Console.WriteLine("GetLiveQuote got exception processing stock {0}: {1}", stocksymbol, e.Message);
                return false;
            }

            return true;
        } //GetLiveQuote

        public bool GetLiveQuote(ArrayList stocksymbols, out Hashtable results)
        {
            results = new Hashtable();

            if (stocksymbols.Count < 1)
                return false; // nothing to do


            try
            {
                // Create the URL to pull all the quotes at one go
                string urlforFile = @"http://live-nse.herokuapp.com/?symbol=";
                bool firstsymbol = true;
                foreach (string symb in stocksymbols)
                {
                    if (!firstsymbol)
                    {
                        urlforFile += ",";
                    }
                    urlforFile += symb;
                    firstsymbol = false;
                }

                Console.WriteLine(urlforFile);

                //Setup the Web Request
                HttpWebRequest myhttpRequest = (HttpWebRequest)WebRequest.Create(urlforFile);
                myhttpRequest.Accept = @"text/json, */*";
                myhttpRequest.AutomaticDecompression = System.Net.DecompressionMethods.None;
                myhttpRequest.UserAgent = "TestHarness client";

                //Retrieve the Web response
                WebResponse myresponse = myhttpRequest.GetResponse();
                Console.WriteLine(((HttpWebResponse)myresponse).StatusDescription);
                // Get the stream containing content returned by the server.
                Stream dataStream = myresponse.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                string myjson = reader.ReadToEnd();
                if (System.Text.RegularExpressions.Regex.IsMatch(myjson, ".*Invalid Symbol", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    return false;

                //Use JSon.NET to deserialize the response
                JsonSerializerSettings jss = new JsonSerializerSettings
                {

                    Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
                    {
                        Console.WriteLine("Error deserializing Json: {0}", errorArgs.ErrorContext.Error.Message);
                        errorArgs.ErrorContext.Handled = true;
                    }
                }; var x = JsonConvert.DeserializeObject<QuoteRootObject>(myjson, jss);

                
                foreach (QuoteDatum q in x.data)
                {
                    Console.WriteLine("{0,15} : {1,10}", q.symbol, q.lastPrice);
                    results.Add(q.symbol, q);
                }


            }
            catch (Exception e)
            {
                Console.WriteLine("GetLiveQuote got exception processing stock {0}", e.Message);
                return false;
            }

            return true;
        } //GetLiveQuote
        Hashtable _iciciDirectToNSE;
        public bool ReadICICIDirectToNSEStockSymbolMapFile()
        {
            // Create an instance of StreamReader to read from a file.
            StreamReader sr = new StreamReader(@"c:\sundeep_personal\map.txt");
            _iciciDirectToNSE = new Hashtable();
            String line;
            int numLinesInFile = 0;
            string delimStr = ",";
            char[] delimiter = delimStr.ToCharArray();
            string[] split = null;

            // Read and display lines from the file until the end of 
            // the file is reached.
            while ((line = sr.ReadLine()) != null)
            {
                numLinesInFile++;
                split = line.Split(delimiter);
                if (split.Length >1)
                {
                    _iciciDirectToNSE.Add(split[0], split[1]);
                }
            }

            Console.WriteLine("Read {0} stock symbols from map file of {1} lines", _iciciDirectToNSE.Keys.Count, numLinesInFile);

            return true;
        } // ReadICICIDirectToNSEStockSymbolMapFile

        public bool GetNSESymbolFromICICIDirectCode(string icicisymbol, out string nsesymbol)
        {
            if (_iciciDirectToNSE.ContainsKey(icicisymbol))
            {
                nsesymbol = (string)_iciciDirectToNSE[icicisymbol];
                return true;
            }
            else
            {
                nsesymbol = "";
                return false;
            }
        } // GetNSESymbolFromICICIDirectCode

        public bool GetNSESymbolFromICICIDirectCode(ArrayList icicisymbols, out ArrayList nsesymbols)
        {
            nsesymbols = new ArrayList();
            if (icicisymbols.Count < 1)
                return false; // nothing to do

            bool matchfound = false;
            foreach (string symb in icicisymbols)
            {
                if (_iciciDirectToNSE.ContainsKey(symb))
                {
                    nsesymbols.Add((string)_iciciDirectToNSE[symb]);
                    matchfound = true;
                }
            }

            return matchfound;
        } // GetNSESymbolFromICICIDirectCode
    }
}
