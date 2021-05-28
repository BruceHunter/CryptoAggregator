using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CryptoAggregator
{
    public class TaxableTransaction
    {
        public string AssetName { get; set; }
        public DateTime ReceivedDate {get; set;}
        public Decimal CostBasisUSD { get; set; }
        public DateTime DateSold { get; set; }
        public Decimal Proceeds { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Aggregator Process Started...");

            var taxableTransactions = new List<TaxableTransaction>();
            bool gatherTransactions = false;
            var aggregatedTaxableTransactions = new List<TaxableTransaction>();

            // path to the csv file
            string path = "C:\\file.csv";

            string[] lines = System.IO.File.ReadAllLines(path);

            foreach (string line in lines)
            {
                TaxableTransaction transaction = null;

                string[] columns = line.Split(',');

                // find the header of the csv file (2020-Robinhood Crypto 1099)
                if (columns[0].Trim().ToUpper().Equals("ASSET NAME"))
                {
                    gatherTransactions = true;
                }
                else
                {
                    if (gatherTransactions)
                    {
                        if (transaction == null)
                        {
                            transaction = new TaxableTransaction();
                        }

                        transaction.AssetName = columns[0];
                        transaction.ReceivedDate = DateTime.Parse(columns[1]);
                        transaction.CostBasisUSD = Decimal.Parse(columns[2]);
                        transaction.DateSold = DateTime.Parse(columns[3]);
                        transaction.Proceeds = Decimal.Parse(columns[4]);
                    }

                    if (gatherTransactions)
                    {
                        taxableTransactions.Add(transaction);
                    }
                }
            }

            foreach(var trans in taxableTransactions)
            {
                var itemIndex = aggregatedTaxableTransactions.FindIndex(item => item.ReceivedDate == trans.ReceivedDate && item.AssetName == trans.AssetName && item.DateSold == trans.DateSold);

                if (itemIndex >= 0)
                {
                    var item = aggregatedTaxableTransactions.ElementAt(itemIndex);
                    item.CostBasisUSD = item.CostBasisUSD + trans.CostBasisUSD;
                    item.Proceeds = item.Proceeds + trans.Proceeds;
                } 
                else
                {
                    aggregatedTaxableTransactions.Add(trans);
                }
            }

            //Provider: Robinhood Crypto LLC
            var outputFile = new StringBuilder();
            outputFile.Append("ASSET NAME").Append(",").Append("RECEIVED DATE").Append(",").Append("COST BASIS(USD)").Append(",").Append("DATE SOLD").Append(",").Append("PROCEEDS");
            outputFile.AppendLine();
            foreach (var line in aggregatedTaxableTransactions)
            {
                outputFile.Append(line.AssetName).Append(",").Append(line.ReceivedDate.ToShortDateString()).Append(",").Append(line.CostBasisUSD).Append(",").Append(line.DateSold.ToShortDateString()).Append(",").Append(line.Proceeds);
                outputFile.AppendLine();
            }

            Console.WriteLine("Writing Aggregation file csv...");
            File.WriteAllText(@".\AggregatedTransactions.csv", outputFile.ToString());
            Console.WriteLine("Aggregator Process Completed...");
            Console.ReadLine();

        }
    }
}
