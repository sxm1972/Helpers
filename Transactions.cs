using System;
using System.Diagnostics;
using System.Collections;
// Change history
// 20-Aug-2008: Initialize the matchfound variable to false in the for loop to take care of the situation where a 
//              transaction is partially matched and no further matching transaction is found. In that case a 
//              NoMatchFound event should be raised for the balance quantity in the partially matched transaction.
namespace Helpers
{
    public interface IStockMatch
    {
        // Called once before any matching is done
        void BeginOperation();      

        // Called once for each stock before any matching is done
        void BeginStock(string stock);

        // Called once for each transaction for which a match will be searched.
        void BeginMatch(SingleTransaction s);

        // Called each time no match is found for a transaction
        void NoMatchFound(SingleTransaction s);

        // Called each time a matching transaction is found
        void MatchFound(SingleTransaction first, SingleTransaction matched, bool firstPartialMatch, bool secondPartialMatch);
        
        // Called once for each stock after all transactions are processed.
        void EndStock(string stock);

        // Called once when the operation is about to end.
        void EndOperation();
    }
	/// <summary>
	/// Summary description for SingleTransaction.
	/// </summary>
	public class SingleTransaction: IComparable
	{
		public enum eTransactionType 
		{
			Buy = 0,
			Add,
			Remove,
			Sell,
			None,
			MaxTranType};
		internal eTransactionType transactionType;
		internal DateTime	transactionDate;
		internal string  transactionStockCode;
		internal long    transactionQty;
		internal decimal transactionPrice;
		internal decimal	transactionCharges;
		internal bool	transactionReconciledFlag;
		internal string  transactionLotIdentifier;

        public string StockCode
        {
            get
            {
                return transactionStockCode;
            }
        }
        public decimal UnitCharges
        {
            get
            {
                return this.transactionCharges / this.transactionQty;
            }
        }

        public decimal TransactionPrice
        {
            get
            {
                return transactionPrice;
            }
        }

        public DateTime TransactionDate
        {
            get
            {
                return transactionDate;
            }
        }

        public long TransactionQty
        {
            get
            {
                return transactionQty;
            }
        }

		public SingleTransaction()
		{
			transactionType = SingleTransaction.eTransactionType.None;
			transactionQty = 0;
			transactionPrice = 0.0M;
			transactionCharges = 0.0M;
			transactionReconciledFlag = false;
			transactionLotIdentifier = "";
			transactionStockCode = "";
			transactionDate = DateTime.Now;
		}

		public SingleTransaction(SingleTransaction.eTransactionType tType, DateTime tDate, string tStockCode, long tQty, decimal tPrice, decimal tCharges,string tlotId)
		{
			transactionType = tType;
			transactionStockCode = tStockCode;
			transactionQty = tQty;
			transactionPrice = tPrice;
			transactionCharges = tCharges;
			transactionDate = tDate;
			transactionReconciledFlag = false;
			transactionLotIdentifier = tlotId;
			Debug.WriteLine (this, "Constructor of SingleTransaction called");
			
		}

        public bool IsAcquisition()
        {
            return (transactionType == eTransactionType.Buy || transactionType == eTransactionType.Add);       
        }

        public bool IsAddition()
        {
            return (transactionType == eTransactionType.Add);
        }
        public bool IsDisposal()
        {
            return (transactionType == eTransactionType.Sell || transactionType == eTransactionType.Remove);
        }
        public bool IsRemoval()
        {
            return (transactionType == eTransactionType.Remove);
        }
        public bool IsOppositeType(eTransactionType e)
        {
            return (((e == eTransactionType.Buy || e == eTransactionType.Add) && this.IsDisposal())||
                ((e == eTransactionType.Sell || e == eTransactionType.Remove) && this.IsAcquisition()));
        }

		public void Dump()
		{
            Console.WriteLine("{0,6} {1,15:d} {2,-6} {3,6} {4,20:f} {5,20:f} {6}", transactionStockCode, transactionDate, transactionType.ToString(), transactionQty, transactionPrice, transactionQty * transactionPrice + transactionCharges, transactionLotIdentifier);
		}
		#region IComparable Members

		public int CompareTo(object obj)
		{
			if (obj is SingleTransaction)
			{
				SingleTransaction thisTrans = (SingleTransaction) obj;
				if (transactionStockCode != thisTrans.transactionStockCode)
					return transactionStockCode.CompareTo(thisTrans.transactionStockCode);

				int ret = transactionDate.CompareTo(thisTrans.transactionDate);
				if (ret != 0)
					return ret;

				ret = (int)transactionType - (int)thisTrans.transactionType;
                if (ret != 0)
                    return ret;

                return transactionLotIdentifier.CompareTo(thisTrans.transactionLotIdentifier);

			}
			throw new ArgumentException("Object is not SingleTransaction");
		}

		#endregion
	};

	public class Account
	{
		SingleTransaction[] acTransactions;
		public Account(int numTrans)
		{
			if (0 == numTrans)
				numTrans = 256;

			acTransactions = (SingleTransaction[])Array.CreateInstance(typeof(SingleTransaction),numTrans);
		}
		int lastTransactionUsed = 0;
		public int AddTransaction(SingleTransaction.eTransactionType ttype, DateTime tDate, string tStockCode, long tQty, decimal tPrice, decimal tCharges, string tlotId)
		{
			// Validations

			// Create a new transaction
			SingleTransaction thisTrans = new SingleTransaction(ttype,tDate,tStockCode,tQty, tPrice, tCharges, tlotId);
			acTransactions[lastTransactionUsed++] = thisTrans;
			if (lastTransactionUsed >= acTransactions.GetLength(0))
				return -1;
			return 0;
		} // AddTransaction
		public void Dump()
		{
			foreach (SingleTransaction s in acTransactions)
				if (s != null)
					s.Dump();
		} // Dump

        public ArrayList GetStockList()
        {
            int i = 0;
            int numtrans = lastTransactionUsed;
            ArrayList stocklist = new ArrayList();
            string thisStockName = "";
            for (i = 0; i < numtrans; i++)
            {
                SingleTransaction thisTrans = acTransactions[i];
                bool bStockHasBeenProcessed = false;

                if (thisStockName == "")
                {
                    //Moving to new stock code. Check if this has been processed already
                    foreach (String s in stocklist)
                    {
                        if (thisTrans.transactionStockCode == s)
                        {
                            bStockHasBeenProcessed = true;
                            break;
                        }
                    }
                }

                if (bStockHasBeenProcessed)
                    continue;

                // this stock has not been processed.
                thisStockName = thisTrans.transactionStockCode;

                // Done processing all transactions.
                stocklist.Add(thisStockName);

                thisStockName = "";

            }
            return stocklist;
        } //GetStockList
        public void StockTaking(DateTime asofDate, bool bShowZeroBalance)
		{
			int i = 0;
			int numtrans = lastTransactionUsed;
			ArrayList stocklist = new ArrayList();
			string thisStockName = "";
			for (i=0;i<numtrans;i++)
			{
				SingleTransaction thisTrans = acTransactions[i];
				bool bStockHasBeenProcessed = false;

				if (thisStockName == "")
				{
					//Moving to new stock code. Check if this has been processed already
					foreach (String s in stocklist)
					{
						if (thisTrans.transactionStockCode == s)
						{
							bStockHasBeenProcessed = true;
							break;
						}
					}
				}
				
				if (bStockHasBeenProcessed)
					continue; 

				// this stock has not been processed.
				thisStockName = thisTrans.transactionStockCode;
				//Console.WriteLine("Processing stock {0}", thisStockName);
				long nStockQty = 0;
				if (thisTrans.transactionType == SingleTransaction.eTransactionType.Buy ||
					thisTrans.transactionType == SingleTransaction.eTransactionType.Add )
					nStockQty += thisTrans.transactionQty;
				else
					nStockQty -= thisTrans.transactionQty;
				// go through the rest of the transactions
				for (int j = i+1; j<numtrans; j++)
				{
					SingleTransaction thisNextTrans = acTransactions[j];
					if (thisNextTrans.transactionStockCode != thisStockName)
						continue; //ignore other stock code transactions

					// check if the transaction happened before the asofDate
					if (thisNextTrans.transactionDate > asofDate)
					{
						//Console.WriteLine("Transaction happened after as of date. Ignoring");
						//thisNextTrans.Dump();
						continue; // ignore transactions that happened after asofDate
					}

					if (thisNextTrans.transactionType == SingleTransaction.eTransactionType.Buy ||
						thisNextTrans.transactionType == SingleTransaction.eTransactionType.Add )
						nStockQty += thisNextTrans.transactionQty;
					else
						nStockQty -= thisNextTrans.transactionQty;
				}
				// Done processing all transactions.
				stocklist.Add(thisStockName);
				if (nStockQty == 0 && !bShowZeroBalance)
				{
					thisStockName = "";
					continue;
				}
				Console.WriteLine("Stock balance for {0,6} as of {1,15:d} is {2,15}", thisStockName, asofDate, nStockQty);
				thisStockName = "";

			}
		}// StockTaking

		public void CapitalGains(System.DateTime fromdate, System.DateTime todate, int longtermdays)
		{
			// Sort transactions by stock code.
			// Data structure is a Hashtable of StockCode (Key) to ArrayList of SingleTransaction (value)
			Hashtable mystocktable = new Hashtable();
            if (!SortTransactionsByStockCode(out mystocktable))
                return;

			decimal totallongterm = 0.0M;
			decimal totalshortterm = 0.0M;

			// Now process each stock
            bool header = true;
            bool bAtLeastOneTransForThisStock = false;
            
			foreach (String stock in mystocktable.Keys)
			{
				ArrayList thisStockTransactions = (ArrayList)mystocktable[stock];
                header = true;
                bAtLeastOneTransForThisStock = false;
				thisStockTransactions.Sort();
				bool bfirst = true;
				long nStockQty = 0;
				decimal longterm = 0.0M;
				decimal shortterm = 0.0M;
				decimal sUnitBrokerage = 0.0M;
				for (int i=0;i< thisStockTransactions.Count;i++)
				{
					SingleTransaction s = (SingleTransaction)thisStockTransactions[i];
					if (s.transactionReconciledFlag)
						continue;
					sUnitBrokerage = s.transactionCharges/s.transactionQty;
					if (bfirst)
					{
						bfirst = false;
						if (!s.IsAcquisition())
							Console.WriteLine("First transaction for {0} is not a BUY order. ref {1}", s.transactionStockCode, s.transactionLotIdentifier);
						
						if (s.IsAcquisition())
							nStockQty += s.transactionQty;
						else
							nStockQty -= s.transactionQty;

					}
					if (s.transactionReconciledFlag)
						continue;
					if (nStockQty != 0)
					{
						// find matching transaction
						SingleTransaction.eTransactionType thisttype = s.transactionType;

						bool restartloop = false;
						do
						{
							restartloop = false;
							decimal tlongterm = 0.0M;
							decimal tshortterm = 0.0M;
							for (int j=i+1; j<thisStockTransactions.Count;j++)
							{
								SingleTransaction snext = (SingleTransaction)thisStockTransactions[j];
								if (snext.transactionReconciledFlag)
									continue;
								if (!snext.IsOppositeType(thisttype) )
									continue;

								if (SingleTransaction.ReferenceEquals(s,snext))
									continue;

								decimal spunit = 0.0M; decimal cpunit = 0.0M;decimal gainlossunit = 0.0M;
								if (s.transactionDate.CompareTo(snext.transactionDate) < 0)
								{
									spunit = snext.transactionPrice - snext.transactionCharges/snext.transactionQty;
									cpunit = s.transactionPrice + sUnitBrokerage;
								}
								else
								{
									spunit = s.transactionPrice - sUnitBrokerage;
									cpunit = snext.transactionPrice + snext.transactionCharges/snext.transactionQty;
								}
								gainlossunit = spunit - cpunit;
	
								if (nStockQty == snext.transactionQty)
								{
									SingleTransaction first = null;
									if (nStockQty == s.transactionQty)
										first = s;
									else
										first = new SingleTransaction(s.transactionType,s.transactionDate,s.transactionStockCode,nStockQty,s.transactionPrice,s.transactionCharges/s.transactionQty*nStockQty,s.transactionLotIdentifier);

									SingleTransaction selltrans = null;
									if (first.IsDisposal())
										selltrans = first;
									else
										selltrans = snext;

									if (selltrans.transactionDate.CompareTo(fromdate) >= 0 && selltrans.transactionDate.CompareTo(todate) <= 0)
									{
										TimeSpan ts = new TimeSpan();
										ts = snext.transactionDate-first.transactionDate;
										if (ts.Days < longtermdays)
										{
											tshortterm = gainlossunit*nStockQty;
											shortterm += tshortterm;
											totalshortterm += tshortterm;
											tlongterm = 0.0M;
										}
										else
										{
											tlongterm = gainlossunit*nStockQty;
											longterm += tlongterm;
											totallongterm += tlongterm;
											tshortterm = 0.0M;
										}
										Helpers.OutputHelper.PrintGains(first,snext,tshortterm,tlongterm,header);
                                        header = false;
                                        bAtLeastOneTransForThisStock = true;

									}
									s.transactionReconciledFlag = true;
									snext.transactionReconciledFlag = true;
									nStockQty = 0;
									bfirst = true;
									break;

								}
								else if (nStockQty > snext.transactionQty)
								{
									long qty = snext.transactionQty;
									SingleTransaction first = new SingleTransaction(s.transactionType,s.transactionDate,s.transactionStockCode,qty,s.transactionPrice,s.transactionCharges/s.transactionQty*qty,s.transactionLotIdentifier);
									SingleTransaction selltrans = null;
									if (first.IsDisposal())
										selltrans = first;
									else
										selltrans = snext;

									if (selltrans.transactionDate.CompareTo(fromdate) >= 0 && selltrans.transactionDate.CompareTo(todate) <= 0)
									{
										TimeSpan ts = new TimeSpan();
										ts = snext.transactionDate-first.transactionDate;
										if (ts.Days < longtermdays)
										{
											tshortterm = gainlossunit*qty;
											shortterm += tshortterm;
											totalshortterm += tshortterm;
											tlongterm = 0.0M;
										}
										else
										{
											tlongterm = gainlossunit*qty;
											longterm += tlongterm;
											totallongterm += tlongterm;
											tshortterm = 0.0M;
										}
										Helpers.OutputHelper.PrintGains(first,snext,tshortterm,tlongterm,header);
                                        header = false;
                                        bAtLeastOneTransForThisStock = true;
									}
									nStockQty -= qty;
									snext.transactionReconciledFlag = true;
								}
								else if (nStockQty < snext.transactionQty)
								{
									SingleTransaction first = new SingleTransaction(s.transactionType,s.transactionDate,s.transactionStockCode,nStockQty,s.transactionPrice,s.transactionCharges/s.transactionQty*nStockQty,s.transactionLotIdentifier);
									SingleTransaction second = new SingleTransaction(snext.transactionType,snext.transactionDate,snext.transactionStockCode,nStockQty,snext.transactionPrice,snext.transactionCharges/snext.transactionQty*nStockQty,snext.transactionLotIdentifier);
									SingleTransaction selltrans = null;
									if (first.IsDisposal())
										selltrans = first;
									else
										selltrans = second;
                                    int nFrom = selltrans.transactionDate.CompareTo(fromdate);
                                    int nTo = selltrans.transactionDate.CompareTo(todate);
									if ( nFrom >= 0 &&  nTo <= 0)
									{
										TimeSpan ts = new TimeSpan();
										ts = second.transactionDate-first.transactionDate;
										if (ts.Days < longtermdays)
										{
											tshortterm = gainlossunit*nStockQty;
											shortterm += tshortterm;
											totalshortterm += tshortterm;
											tlongterm = 0.0M;
										}
										else
										{
											tlongterm = gainlossunit*nStockQty;
											longterm += tlongterm;
											totallongterm += tlongterm;
											tshortterm = 0.0M;
										}
										Helpers.OutputHelper.PrintGains(first,second,tshortterm,tlongterm,header);
                                        header = false;
                                        bAtLeastOneTransForThisStock = true;
									}
									nStockQty = snext.transactionQty - nStockQty;
									s.transactionReconciledFlag = true;
									s = snext;
									sUnitBrokerage = s.transactionCharges/s.transactionQty;

									thisttype = s.transactionType;
									restartloop = true;
									break;
								}
							}
						}while (restartloop);
					}
					else
						bfirst = true;
				}
                if (bAtLeastOneTransForThisStock)
                {
                    Console.WriteLine("{0,110} {1,12:F2} {2,12:F2}", "", shortterm, longterm);
                    Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------------------------");
                }
			}
            Console.WriteLine("{0,110} {1,12:F2} {2,12:F2}", "", totalshortterm, totallongterm);
            Console.WriteLine("---------------------------------------------------------------------------------------------------------------------------------------------");
			return;
		}// CapitalGains
	
        internal void ResetMatchedTransactions()
        {
            for (int n = 0; n<lastTransactionUsed; n++)
            {
                SingleTransaction s = acTransactions[n];
                s.transactionReconciledFlag = false;
            }
        }
        public void MatchTransactions(string matchthis, IStockMatch eventSink)
        {
            ResetMatchedTransactions();
            eventSink.BeginOperation();
            Hashtable mystocktable;
            if (!SortTransactionsByStockCode(out mystocktable))
                return;

            long nStockQty = 0;
            foreach (String stock in mystocktable.Keys)
            {
                if (matchthis != "" && matchthis != stock)
                    continue;

                ArrayList thisStockTransactions = (ArrayList)mystocktable[stock];
                bool bfirst = true;
                eventSink.BeginStock(stock);
                for (int i = 0; i < thisStockTransactions.Count; i++)
                {
                    SingleTransaction s = (SingleTransaction)thisStockTransactions[i];
                    if (s.transactionReconciledFlag)
                        continue;
                    if (bfirst)
                    {
                        bfirst = false;
                        nStockQty = s.transactionQty;
                        eventSink.BeginMatch(s);
                    }
                    if (s.transactionReconciledFlag)
                        continue;
                    if (nStockQty == 0)
                        continue;

                    // find matching transaction
                    SingleTransaction.eTransactionType thisttype = s.transactionType;
                    bool restartloop = false;
                    do
                    {
                        restartloop = false;
                        bool matchfound = false;
                        for (int j = i + 1; j < thisStockTransactions.Count; j++, matchfound = false)
                        {
                            SingleTransaction snext = (SingleTransaction)thisStockTransactions[j];
                            if (snext.transactionReconciledFlag)
                                continue;
                            if (!snext.IsOppositeType(thisttype))
                                continue;

                            if (SingleTransaction.ReferenceEquals(s, snext))
                                continue;
                            matchfound = true;
                            if (nStockQty == snext.transactionQty)
                            {
                                SingleTransaction first = null;
                                if (nStockQty == s.transactionQty)
                                    first = s;
                                else
                                    first = new SingleTransaction(s.transactionType, s.transactionDate, s.transactionStockCode, nStockQty, s.transactionPrice, s.transactionCharges / s.transactionQty * nStockQty, s.transactionLotIdentifier);

                                eventSink.MatchFound(first, snext, false, false);

                                s.transactionReconciledFlag = true;
                                snext.transactionReconciledFlag = true;
                                nStockQty = 0;
                                bfirst = true;

                                // A BUY transaction has been completely reconciled with one or more SELL transactions.
                                // Now we need to start with the next BUY transaction. Hence break out!
                                break;
                            }
                            else if (nStockQty > snext.transactionQty)
                            {
                                long qty = snext.transactionQty;
                                SingleTransaction first = new SingleTransaction(s.transactionType, s.transactionDate, s.transactionStockCode, qty, s.transactionPrice, s.transactionCharges / s.transactionQty * qty, s.transactionLotIdentifier);

                                eventSink.MatchFound(first, snext, true, false);
                                nStockQty -= qty;
                                snext.transactionReconciledFlag = true;
                            }
                            else if (nStockQty < snext.transactionQty)
                            {
                                SingleTransaction first = new SingleTransaction(s.transactionType, s.transactionDate, s.transactionStockCode, nStockQty, s.transactionPrice, s.transactionCharges / s.transactionQty * nStockQty, s.transactionLotIdentifier);
                                SingleTransaction second = new SingleTransaction(snext.transactionType, snext.transactionDate, snext.transactionStockCode, nStockQty, snext.transactionPrice, snext.transactionCharges / snext.transactionQty * nStockQty, snext.transactionLotIdentifier);

                                eventSink.MatchFound(first, second, false, true);

                                nStockQty = snext.transactionQty - nStockQty;
                                s.transactionReconciledFlag = true;
                                s = snext;

                                // Now the SELL transaction which is at a later date is left with some qty that is not matched. We need to find 
                                // a matching BUY transaction from an earlier date to completely reconcile this SELL transaction. Hence start
                                // from the beginning of the transaction list.
                                SingleTransaction begin = new SingleTransaction(s.transactionType, s.transactionDate, s.transactionStockCode, nStockQty, s.transactionPrice, s.transactionCharges / s.transactionQty * nStockQty, s.transactionLotIdentifier);
                                eventSink.BeginMatch(begin);
                                thisttype = s.transactionType;
                                restartloop = true;
                                break;
                            }
                        } // loop on all transactions following the current one we are looking for a match for.

                        // if there is no matching transaction, the next transaction should still be considered a new one.
                        if (!matchfound)
                        {
                            SingleTransaction sUnmatched = new SingleTransaction(s.transactionType, s.transactionDate, s.transactionStockCode, nStockQty, s.transactionPrice, s.transactionCharges / s.transactionQty * nStockQty, s.transactionLotIdentifier);
                            s.transactionReconciledFlag = true;
                            eventSink.NoMatchFound(sUnmatched);
                            bfirst = true;
                        }
                    } while (restartloop);

                } // loop on all transactions for current stock
                eventSink.EndStock(stock);
            } // loop on all stocks

            eventSink.EndOperation();
        }

        private bool SortTransactionsByStockCode(out Hashtable mystocktable)
        {
            // Sort transactions by stock code.
            // Data structure is a Hashtable of StockCode (Key) to ArrayList of SingleTransaction (value)
            mystocktable = new Hashtable();
            int i = 0;
            bool retval = false;
            int numtrans = lastTransactionUsed;
            for (i = 0; i < numtrans; i++)
            {
                SingleTransaction thisTrans = acTransactions[i];
                ArrayList thisStockTransactions = null;
                String thisStockCode = thisTrans.transactionStockCode;

                // check if this stock already has a hashtable entry.
                if (!mystocktable.ContainsKey(thisTrans.transactionStockCode))
                    mystocktable.Add(thisTrans.transactionStockCode, (thisStockTransactions = new ArrayList()));
                else
                    thisStockTransactions = (ArrayList)mystocktable[thisStockCode];

                thisStockTransactions.Add(thisTrans);
                retval = true;
            } //for

            return retval;
        }

    }
}