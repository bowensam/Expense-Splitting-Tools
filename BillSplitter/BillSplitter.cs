using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingTools
{
    /// <summary>
    /// The BillSplitter class obtains individual expenses from a user-designated text file and calculate
    /// any applicable money each person must pay or be paid. Results are stored in an output text file 
    /// within the same directory as the input file.
    /// </summary>
    /// <remarks>
    /// In the output text file: Positive values denote the participant owes money to the group. While 
    /// negative values (enclosed with brackets without negative signs) denote the group owes money to 
    /// the participant.
    /// </remarks>
    public class BillSplitter
    {
        static void Main(string[] args)
        {
            BillSplitter b = new BillSplitter();

            //Obtain input to a valid text file.
            bool invalidFile = true;
            String fileName, filePath;
            do
            {
                fileName = Console.ReadLine();

                //Ensure file name (user input) is not empty or white space.
                if (String.IsNullOrWhiteSpace(fileName))
                {
                    Console.WriteLine("Please enter the file name of the text file.\n");
                    continue;
                }

                //Ensure file name (user input) refers to a Text File (.txt).
                else if (!fileName.EndsWith(".txt"))
                {
                    Console.WriteLine("The file name must end with \".txt\". Please try again.\n");
                    continue;
                }

                //Test if the file name (user input) refers to a valid text file that exists.
                invalidFile = b.IsTextFileInvalid(fileName);
            } while (invalidFile);


            //Opening input file to read and creating output file.
            filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            StreamReader record = new StreamReader(filePath);
            StreamWriter newRecord = new StreamWriter(filePath + ".out");


            //Reading data from input file (record), each camping trip is processed one at a time by each loop.
            String line;
            while ((line = record.ReadLine()) != "0")
            {
                //Calculate the "Total Money Each Individual Spent" (expenses <decimal array>) by adding up their receipts and storing them individually.
                int totalParticipant = Int32.Parse(line);
                decimal[] expenses = new decimal[totalParticipant];
                for (int participant = 0; participant < totalParticipant; participant++)
                {
                    line = record.ReadLine();
                    int totalReceipt = Int32.Parse(line);
                    for (int receipt = 1; receipt <= totalReceipt; receipt++)
                    {
                        line = record.ReadLine();
                        expenses[participant] += Decimal.Parse(line);
                    }
                }

                //Calculate the "Total Expense for The Group" (totalExpense <decimal>) by summating each individual expenses from the array.
                decimal totalExpense = 0;
                foreach (decimal individualExpense in expenses)
                    totalExpense += individualExpense;

                //Calculate the "Share of Expense" (share <decimal>) each individual should pay based on the total group expense.
                decimal share = totalExpense / totalParticipant;

                //Calculate any applicable "Adjustment" (adjustment <decimal>) for each individual (based on how much each individual should pay, and how much they already paid).
                foreach (decimal individualExpense in expenses)
                {
                    decimal adjustment = share - individualExpense;

                    if (adjustment < 0) //Group owes individual money. Additional formatting is required before output.
                    {
                        adjustment = adjustment * -1; //When adjustment is negative, the output is enclosed by brackets without the negative sign (in accordance with output format specification).
                        newRecord.WriteLine("($" + Math.Round(adjustment, 2) + ")");
                    }

                    else //(adjustment > 0) Individual owes group money.
                        newRecord.WriteLine("$" + Math.Round(adjustment, 2));
                }
                newRecord.WriteLine(); //Data for this trip has been completely analyzed. A blank line is added to separate from the next trip (in accordance with output format specification).
            } //End of one trip

            newRecord.Close(); //All adjustments were documented, closing output file
        }

        /// <summary>
        /// The IsTextFileInvaild method checks whether the input file name refers to an invalid text file.
        /// </summary>
        /// <remarks>
        /// The following are considered an invalid text file:
        /// - The text file does not exist.
        /// - The text file is being used by another process.
        /// - The text file is empty or contains blank lines.
        /// - The text file contains negative or non-numerical data.
        /// </remarks>
        /// <param name="fileName">The file name of the text file.</param>
        /// <returns>
        /// <c>true</c> if the file name refers to an invalid text file; Otherwise, <c>false</c>.
        /// </returns>
        public bool IsTextFileInvalid (String fileName)
        {
            //Tests if file name refers to a file that exists and can be opened.
            //Note: The text file must be located in the same folder as the .exe.
            String filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            StreamReader test;
            try
            {    
                test = new StreamReader(filePath);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File cannot be found. Please try again.\n");
                return true;
            }
            catch (IOException)
            {
                Console.WriteLine("File is being used by another process. Please try again.\n");
                return true;
            }


            //Tests if text file is properly formatted.
            //1) Checks if there is an empty or blank line before "0" is reached. If so, the text file is invalid.
            String line;
            while ((line = test.ReadLine()) != "0")
            {
                if (String.IsNullOrWhiteSpace(line))
                {
                    Console.WriteLine("File is empty or incorrectly formatted. Please use another file.\n");
                    return true;
                }
                else
                {
                    //2) Checks if the line contains negative or non-numerical data. If so, the text file is invalid.
                    try
                    {
                        if (Int32.Parse(line) < 0)
                        {
                            Console.WriteLine("File contains negative integers. Please use another file.\n");
                            return true; //Not positive integers
                        }
                    }
                    catch (FormatException) //Not integers
                    {
                        try
                        {
                            if (Decimal.Parse(line) < 0)
                            {
                                Console.WriteLine("File contains negative numbers. Please use another file.\n");
                                return true; //Not positive numbers
                            }
                        }
                        catch (FormatException) //Not numbers
                        {
                            Console.WriteLine("File contains non-numeric data. Please use another file.\n");
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
