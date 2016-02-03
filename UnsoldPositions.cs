using System;
using System.Collections.Generic;
using System.Text;
using Helpers;
namespace TestHarness
{
    public class UnsoldPositions : IStockMatch
    {
        DateTime asofDate;
        long thisstockqty = 0;
        long longtermdays = 365;
        bool debug = false;

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



        public UnsoldPositions(DateTime date)
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
        }

        // Called once for each transaction for which a match will be searched.
        void IStockMatch.BeginMatch(SingleTransaction s)
        {
        }

        void IStockMatch.NoMatchFound(SingleTransaction s)
        {
            if (asofDate.CompareTo(s.TransactionDate) < 0)
            {
                if (debug)
                    System.Console.WriteLine("Transaction happened on {0,15:d} after date {1,15:d}. Ignoring...", s.TransactionDate, asofDate);
                return;
            }

            TimeSpan ts = new TimeSpan();
            ts = asofDate - s.TransactionDate;

            //Prefix LT or ST to the transaction
            if (ts.Days > longtermdays)
                System.Console.Write("LONG TERM  ");
            else
                System.Console.Write("SHORT TERM ");
            
            // Output the transaction details
            s.Dump();

            // Keep track of stock balance for unsold stocks
            if (s.IsAcquisition())
                thisstockqty += s.TransactionQty;
            else
                thisstockqty -= s.TransactionQty;
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

            Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("Stock balance for {0,6} on {1,8:d} is {2,7}", stock, asofDate, thisstockqty);
            Console.WriteLine("=============================================================================================================================================");
        }

        // Called once when the operation is about to end.
        void IStockMatch.EndOperation()
        {
        }
    }
}
