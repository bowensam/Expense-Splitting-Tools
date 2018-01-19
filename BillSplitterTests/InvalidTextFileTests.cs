using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CampingTools;
using System.IO;

namespace BillSplitterTests
{
    [TestClass]
    public class InvalidTextFileTests
    {
        [TestMethod]
        public void FileWithValidData ()
        {
            BillSplitter b = new BillSplitter();
            String fileName = "validInput.txt";
            String[] validData = new string[21] {"3", "2", "10.00", "20.00", "4", "15.00", "15.01", "3.00", "3.01", "3", "5.00", "9.00", "4.00",
                                                    "2", "2", "8.00", "6.00", "2", "9.20", "6.75", "0" };
            CreateTextFile(fileName, validData); //Creates a valid text file

            bool actual = b.IsTextFileInvalid(fileName);

            Assert.IsFalse(actual, "Incorrectly determined a valid file as invalid.");
        }

        [TestMethod]
        public void FileDoesNotExist ()
        {
            BillSplitter b = new BillSplitter();
            String fileName = ".txt"; //Refers to a file that does not exist

            bool actual = b.IsTextFileInvalid(fileName);

            Assert.IsTrue(actual, "Incorrectly determined a non-existing file as valid.");
        }

        [TestMethod]
        public void FileWithNoEndZero ()
        {
            BillSplitter b = new BillSplitter();
            String fileName = "invalidInputA.txt";
            String[] invalidDataA = new string[7] { "2", "2", "8.00", "6.00", "2", "9.20", "6.75" }; //No "0" at the end
            CreateTextFile(fileName, invalidDataA); //Creates an invalid text file that does not end with a "0" line

            bool actual = b.IsTextFileInvalid(fileName);

            Assert.IsTrue(actual, "Incorrectly determined a non-zero-terminating file as valid.");
        }

        [TestMethod]
        public void FileWithMissingData ()
        {
            BillSplitter b = new BillSplitter();
            String fileName = "invalidInputB.txt";
            String[] invalidDataB = new string[8] { "2", "2", "8.00", " ", "2", "9.20", "", "0" }; //Missing data
            CreateTextFile(fileName, invalidDataB); //Creates an invalid text file with missing data

            bool actual = b.IsTextFileInvalid(fileName);

            Assert.IsTrue(actual, "Incorrectly determined a file with missing data as valid.");
        }
        
        [TestMethod]
        public void FileWithNoData ()
        {
            BillSplitter b = new BillSplitter();
            String fileName = "validInputC.txt";
            String[] invalidDataC = new string[1] { null }; //Empty file
            CreateTextFile(fileName, invalidDataC); //Creates an invalid text file with no data

            bool actual = b.IsTextFileInvalid(fileName);

            Assert.IsTrue(actual, "Incorrectly determined an empty file as valid.");
        }

        [TestMethod]
        public void FileWithNegativeValues ()
        {
            BillSplitter b = new BillSplitter();
            String fileName = "invalidInputD.txt";
            String[] invalidDataD = new string[8] { "2", "2", "8.00", "-6.00", "2", "9.20", "6.75", "0" }; //Negative values
            CreateTextFile(fileName, invalidDataD); //Creates an invalid text file with negative values

            bool actual = b.IsTextFileInvalid(fileName);

            Assert.IsTrue(actual, "Incorrectly determined a file with negative values as valid.");
        }

        [TestMethod]
        public void FileWithABC ()
        {
            BillSplitter b = new BillSplitter();
            String fileName = "invalidInputE.txt";
            String[] invalidDataE = new string[8] { "2", "2", "8.00", "6.00A", "2", "9.20", "6.75", "0" }; //Non-numerical data
            CreateTextFile(fileName, invalidDataE); //Creates an invalid text file with non-numerical data

            bool actual = b.IsTextFileInvalid(fileName);

            Assert.IsTrue(actual, "Incorrectly determined a file with non-numerical data as valid.");
        }

        private void CreateTextFile (String fileName, String[] data)
        {
            String filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            StreamWriter file = new StreamWriter(filePath);

            foreach (String value in data){
                file.WriteLine(value);
            }
            file.Close();
        }
    }
}
