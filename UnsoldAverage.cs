using System;
using System.Collections.Generic;
using System.Text;
using Helpers;
namespace TestHarness
{
    public class UnsoldAverage : IStockMatch
    {
        DateTime asofDate;
        long thisstockqty = 0;
        bool debug = false;
        decimal totalcost = 0.0M;
        string thisStockCode = "";
        public bool Debug
        {
            get
            {
                return debug;
            }
            set
            {
                debug = value;
            }
        }



        public UnsoldAverage(DateTime date)
        {
            asofDate = date;
        }
        // Called once before any matching is done
        void IStockMatch.BeginOperation()
        {
        }

        // Called once for each stock before any matching is done
        void IStockMatch.BeginStock(string stock)
        {
            thisstockqty = 0;
            thisStockCode = stock;
            totalcost = 0.0M;
        }

        // Called once for each transaction for which a match will be searched.
        void IStockMatch.BeginMatch(SingleTransaction s)
        {
            if (debug)
                System.Console.WriteLine("BEGIN MATCH Stock balance for {0,6} as of {1,15:d} is {2,15}.", s.StockCode, asofDate, thisstockqty);

        }

        void IStockMatch.NoMatchFound(SingleTransaction s)
        {
            if (asofDate.CompareTo(s.TransactionDate) < 0)
            {
                if (debug)
                    System.Console.WriteLine("Transaction happened on {0,15:d} after date {1,15:d}. Ignoring...", s.TransactionDate, asofDate);
                return;
            }

            // Keep track of stock balance for unsold stocks
            if (!s.IsAcquisition())
            {
                System.Console.Write("Unmatched sell transaction!!!");
                s.Dump();
                return;
            }

            thisstockqty += s.TransactionQty;
            totalcost += s.TransactionQty * (s.TransactionPrice + s.UnitCharges);
        }

        // Called each time a matching transaction is found
        void IStockMatch.MatchFound(SingleTransaction s, SingleTransaction matched, bool firstPartialMatch, bool secondPartialMatch)
        {
        }
        // Called once for each stock after all transactions are processed.
        void IStockMatch.EndStock(string stock)
        {
            if (thisstockqty == 0)
                return;

            Console.WriteLine("Stock average for {0,6} as of {1,15:d} for {2,15} shares is {3,12:F2}", thisStockCode, asofDate, thisstockqty, totalcost / thisstockqty);
            thisStockCode = "";
        }

        // Called once when the operation is about to end.
        void IStockMatch.EndOperation()
        {
        }
    }
}
