using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers
{
    public class ReturnOnInvestmentListener : IStockMatch
    {
        double totalshortterm = 0;
        double gainorloss = 0;
        bool header = false;
        public void BeginMatch(SingleTransaction s)
        {
        }

        public void BeginOperation()
        {
        }

        public void BeginStock(string stock)
        {
            Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------------------------------");
            gainorloss = 0;
            header = true;
        }

        public void EndOperation()
        {
            Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------------------------------");
        }

        public void EndStock(string stock)
        {
            Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------------------------------");
        }

        public void MatchFound(SingleTransaction first, SingleTransaction matched, bool firstPartialMatch, bool secondPartialMatch)
        {
            double spunit = 0;
            double cpunit = 0;
            double gainlossunit = 0;
            double thistransgainorloss = 0.0;
            double returnOnInvestment = 0.0;
            double roIAnnualized = 0.0;

            if (first.TransactionDate.CompareTo(matched.TransactionDate) < 0)
            {
                spunit = Convert.ToDouble(matched.TransactionPrice - matched.UnitCharges);
                cpunit = Convert.ToDouble(first.TransactionPrice + first.UnitCharges);
            }
            else
            {
                spunit = Convert.ToDouble(first.TransactionPrice - first.UnitCharges);
                cpunit = Convert.ToDouble(matched.TransactionPrice + matched.UnitCharges);
            }
            gainlossunit = spunit - cpunit;

            SingleTransaction selltrans = null;
            if (first.IsDisposal())
                selltrans = first;
            else
                selltrans = matched;

            TimeSpan ts = new TimeSpan();
            ts = matched.TransactionDate - first.TransactionDate;
            int days = ts.Days;
            if (days < 0)
                days *= -1;
            double yearsheld = Convert.ToDouble(days) / 365;
            double baseamount = 0;
            if (!selltrans.IsRemoval()) // if this is a removal there is no gain or loss.
            {
                thistransgainorloss = gainlossunit * first.TransactionQty;
                gainorloss = gainorloss + thistransgainorloss;
                totalshortterm += thistransgainorloss;
                baseamount = Convert.ToDouble(first.TransactionPrice * first.TransactionQty) + Convert.ToDouble(first.transactionCharges) + Convert.ToDouble(matched.transactionCharges);
                returnOnInvestment = (thistransgainorloss / baseamount);
                if (returnOnInvestment < 0)
                {
                    roIAnnualized = -1 * (Math.Pow((-1*returnOnInvestment)+1, (1 / yearsheld))-1);
                }
                else
                {
                    roIAnnualized = Math.Pow(returnOnInvestment+1, (1 / yearsheld)) -1;
                }
            }
            OutputHelper.PrintRoI(first, matched, thistransgainorloss, days, (returnOnInvestment*100), (roIAnnualized*100), header);
            header = false;
        }

        public void NoMatchFound(SingleTransaction s)
        {
        }
    }
}
