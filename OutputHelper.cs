using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers
{
    public class OutputHelper
    {
        public static void PrintGains(SingleTransaction first, SingleTransaction second, decimal shortgain, decimal longgain, bool header)
        {
            if (header)
            {
                Console.WriteLine("{0,6} {1,10} {2,4} {3,5:d} {4,8} {5,18}  || {6,10} {7,4} {8,5:d} {9,8} {10,18} {11,12} {12,12}",
                    "CODE",
                    "DATE",
                    "OTYP",
                    "OQTY",
                    "PRICE",
                    "ORDER REF",
                    "DATE",
                    "OTYP",
                    "OQTY",
                    "PRICE",
                    "ORDER REF",
                    "GAIN ST",
                    "GAIN LT");
            }

            if (first == null || second == null)
                return;

            SingleTransaction one = null;
            SingleTransaction two = null;
            if (first.transactionStockCode != second.transactionStockCode)
            {
                Console.WriteLine("Order {0} stockcode {1} not matching order {2} stockcode {3}", first.transactionLotIdentifier, first.transactionStockCode, second.transactionLotIdentifier, second.transactionStockCode);
                return;
            }
            if (first.transactionDate.CompareTo(second.transactionDate) == 0)
            {
                if (first.transactionType == SingleTransaction.eTransactionType.Buy)
                {
                    one = first;
                    two = second;
                }
                else
                {
                    one = second;
                    two = first;
                }
            }
            else if (first.transactionDate.CompareTo(second.transactionDate) < 0)
            {
                one = first;
                two = second;
            }
            else
            {
                one = second;
                two = first;
            }
            Console.WriteLine("{0,6} {1,10:d} {2,4} {3,5:d} {4,8:F2} {5,18}  || {6,10:d} {7,4} {8,5:d} {9,8:F2} {10,18} {11,12:F2} {12,12:F2}",
                one.transactionStockCode,
                one.transactionDate,
                one.transactionType.ToString(),
                one.transactionQty,
                one.transactionPrice,
                one.transactionLotIdentifier,
                two.transactionDate,
                two.transactionType.ToString(),
                two.transactionQty,
                two.transactionPrice,
                two.transactionLotIdentifier,
                shortgain,
                longgain);

        }

        public static void PrintRoI(SingleTransaction first, SingleTransaction second, double gain, int days, double returnOnInvestment, double annualizedRoI, bool header)
        {
            if (header)
            {
                Console.WriteLine("{0,6} {1,10} {2,4} {3,5} {4,8} {5,18} || {6,10} {7,4} {8,5} {9,8} {10,18} {11,12} {12,4} {13,8} {14,8}",
                    "CODE",                 // 0
                    "DATE",                 // 1
                    "OTYP",                 // 2
                    "OQTY",                 // 3
                    "PRICE",                // 4
                    "ORDER REF",            // 5
                    "DATE",                 // 6
                    "OTYP",                 // 7
                    "OQTY",                 // 8
                    "PRICE",                // 9
                    "ORDER REF",            //10
                    "GAIN LOSS",            //11
                    "DAYS",                 //12
                    "GAIN%",                //13
                    "RoI(Ann.)%");    //14
            }

            if (first == null || second == null)
                return;

            SingleTransaction one = null;
            SingleTransaction two = null;
            if (first.transactionStockCode != second.transactionStockCode)
            {
                Console.WriteLine("Order {0} stockcode {1} not matching order {2} stockcode {3}", first.transactionLotIdentifier, first.transactionStockCode, second.transactionLotIdentifier, second.transactionStockCode);
                return;
            }
            if (first.transactionDate.CompareTo(second.transactionDate) == 0)
            {
                if (first.transactionType == SingleTransaction.eTransactionType.Buy)
                {
                    one = first;
                    two = second;
                }
                else
                {
                    one = second;
                    two = first;
                }
            }
            else if (first.transactionDate.CompareTo(second.transactionDate) < 0)
            {
                one = first;
                two = second;
            }
            else
            {
                one = second;
                two = first;
            }
            Console.WriteLine("{0,6} {1,10:d} {2,4} {3,5} {4,8} {5,18} || {6,10:d} {7,4} {8,5} {9,8} {10,18} {11,12:F2} {12,4} {13,8:F2}% {14,8:F2}%",
                one.transactionStockCode,           // 0
                one.transactionDate,                // 1
                one.transactionType.ToString(),     // 2
                one.transactionQty,                 // 3
                one.transactionPrice,               // 4
                one.transactionLotIdentifier,       // 5
                two.transactionDate,                // 6
                two.transactionType.ToString(),     // 7
                two.transactionQty,                 // 8
                two.transactionPrice,               // 9
                two.transactionLotIdentifier,       //10
                gain,                               //11
                days,                               //12
                returnOnInvestment,                 //13
                annualizedRoI                       //14
                );

        }
    }
}
