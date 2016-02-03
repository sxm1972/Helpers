using System;
using System.Collections.Generic;
using System.Text;
using Helpers;

namespace TestHarness
{
    public class CapGainsListener: IStockMatch
    {
        DateTime fromdate;
        DateTime todate;
        decimal totalshortterm = 0.0M;
        decimal totallongterm = 0.0M;
        decimal shortterm = 0.0M;
        decimal longterm = 0.0M;
        long longtermdays = 365;
        bool atLeastOneTrans = false;
        bool header = false;

        public CapGainsListener(DateTime from, DateTime to, long daysInLongTerm)
        {
            fromdate = from;
            todate = to;
            longtermdays = daysInLongTerm;
        }

        void IStockMatch.BeginOperation()
        {
        }
        void IStockMatch.BeginStock(string stock)
        {
            //helperAc.PrintGains(null, null, 0, 0, true);
            shortterm = 0.0M;
            longterm = 0.0M;
            atLeastOneTrans = false;
            header = true;
        }
        void IStockMatch.BeginMatch(SingleTransaction s)
        {
        }

        void IStockMatch.NoMatchFound(SingleTransaction s)
        {
        }

        void IStockMatch.MatchFound(SingleTransaction first, SingleTransaction matched, bool firstPartialMatch, bool secondPartialMatch)
        {
            decimal spunit = 0.0M; decimal cpunit = 0.0M; decimal gainlossunit = 0.0M;
            decimal tshortterm = 0.0M; decimal tlongterm = 0.0M;
            if (first.TransactionDate.CompareTo(matched.TransactionDate) < 0)
            {
                spunit = matched.TransactionPrice - matched.UnitCharges;
                cpunit = first.TransactionPrice + first.UnitCharges;
            }
            else
            {
                spunit = first.TransactionPrice - first.UnitCharges;
                cpunit = matched.TransactionPrice + matched.UnitCharges;
            }
            gainlossunit = spunit - cpunit;

            SingleTransaction selltrans = null;
            if (first.IsDisposal())
                selltrans = first;
            else
                selltrans = matched;

            if (selltrans.TransactionDate.CompareTo(fromdate) >= 0 && selltrans.TransactionDate.CompareTo(todate) <= 0)
            {
                atLeastOneTrans = true;
                TimeSpan ts = new TimeSpan();
                ts = matched.TransactionDate - first.TransactionDate;
                int days = ts.Days;
                if (days < 0)
                    days *= -1;
                if (!selltrans.IsRemoval()) // if this is a removal there is no gain or loss.
                {
                    if (days < longtermdays || gainlossunit < 0)
                    {
                        tshortterm = gainlossunit * first.TransactionQty;
                        shortterm += tshortterm;
                        totalshortterm += tshortterm;
                        tlongterm = 0.0M;
                    }
                    else
                    {
                        tlongterm = gainlossunit * first.TransactionQty;
                        longterm += tlongterm;
                        totallongterm += tlongterm;
                        tshortterm = 0.0M;
                    }
                    }
                Helpers.OutputHelper.PrintGains(first, matched, tshortterm, tlongterm, header);
                header = false;
            }
        }

        void IStockMatch.EndStock(string stock)
        {
            if (atLeastOneTrans)
            {
                System.Console.WriteLine("{0,110} {1,12:F2} {2,12:F2}", "", shortterm, longterm);
                System.Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------------------------");
            }
        }

        void IStockMatch.EndOperation()
        {
            Console.WriteLine("{0,110} {1,12:F2} {2,12:F2}", "", totalshortterm, totallongterm);
            Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------------------------");
        }
    }
}
