using System;
using System.Collections.Generic;
using System.Text;
using Helpers;

namespace TestHarness
{
    public class StockTakingListener: IStockMatch
    {
        DateTime asofDate;
        long thisstockqty = 0;
        long thisstockqtyLT = 0;
        long longtermdays = 365;
        bool debug = false;
        bool zeros = false;

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

        public bool ShowZeroBalances
        {
            get
            {
                return zeros;
            }
            set
            {
                zeros = value;
            }
        }


        public StockTakingListener(DateTime date)
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
            thisstockqtyLT = 0;
        }

        // Called once for each transaction for which a match will be searched.
        void IStockMatch.BeginMatch(SingleTransaction s)
        {
            int ret = asofDate.CompareTo(s.TransactionDate);
            if (ret < 0)
            {
                //if (debug)
                //    System.Console.WriteLine("Transaction happened on {0,15:d} after date {1,15:d}. Ignoring...", s.TransactionDate, asofDate);
                return;
            }

            TimeSpan ts = new TimeSpan();
            ts = asofDate - s.TransactionDate;

            if (debug)
            {
                System.Console.WriteLine("--------------------------------------------------------------------------------------------------------");
                s.Dump();
            }

            if (s.IsAcquisition())
            {
                thisstockqty += s.TransactionQty;
                if (ts.Days > longtermdays)
                    thisstockqtyLT += s.TransactionQty;
            }
            else
            {

                thisstockqty -= s.TransactionQty;
                if (ts.Days > longtermdays)
                    thisstockqtyLT -= s.TransactionQty;
            }
            if (debug)
                System.Console.WriteLine("BEGIN MATCH Stock balance for {0,6} as of {1,15:d} is {2,15}. Long term quantities {3,15}", s.StockCode, asofDate, thisstockqty, thisstockqtyLT);
        }

        void IStockMatch.NoMatchFound(SingleTransaction s)
        {
        }

        // Called each time a matching transaction is found
        void IStockMatch.MatchFound(SingleTransaction s, SingleTransaction matched, bool firstPartialMatch, bool secondPartialMatch)
        {
            // This stock was sold. So remove it from our counting
            int ret = asofDate.CompareTo(s.TransactionDate);
            if (ret < 0)
            {
                //if (debug)
                //System.Console.WriteLine("Transaction happened on {0,15:d} after date {1,15:d}. Ignoring...", s.TransactionDate, asofDate);
                return;
            }

            TimeSpan ts = new TimeSpan();
            ts = asofDate - s.TransactionDate;
            if (debug)
            {
                s.Dump();
                matched.Dump();
            }
            if (matched.IsDisposal())
            {
                thisstockqty -= matched.TransactionQty;
                if (ts.Days > longtermdays)
                    thisstockqtyLT -= matched.TransactionQty;
            }
            else
            {
                thisstockqty += matched.TransactionQty;
                if (ts.Days > longtermdays)
                    thisstockqtyLT += matched.TransactionQty;
            }
            if (debug)
            {
                System.Console.WriteLine("END MATCH Stock balance for {0,6} as of {1,15:d} is {2,15}. Long term quantities {3,15}", s.StockCode, asofDate, thisstockqty, thisstockqtyLT);
                System.Console.WriteLine("-----------------------------------------------------------------------------------");
            }
        }

        // Called once for each stock after all transactions are processed.
        void IStockMatch.EndStock(string stock)
        {
            if (thisstockqty == 0 && !zeros)
                return;

            System.Console.WriteLine("Stock balance for {0,6} as of {1,15:d} is {2,15}. Long term quantities {3,15}", stock, asofDate, thisstockqty, thisstockqtyLT);
        }

        // Called once when the operation is about to end.
        void IStockMatch.EndOperation()
        {
        }
    }
}
