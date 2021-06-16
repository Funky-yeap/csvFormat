using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;
using System.Globalization;
using System.Threading;

namespace csvFormat
{
    /*To DO:
     * ReadFile
     * Fields
     * Get unique names and dates
     * Make a dictionary (key: name, value: List<float> hours)
     * WriteFile
    */
    public class Employee_entry
    {
        [CsvHelper.Configuration.Attributes.Index(0)]
        public string Name { get; set; }
        private DateTime _date;
        [CsvHelper.Configuration.Attributes.Index(1)]
        public DateTime Date { get => _date; set => _date = Convert.ToDateTime(value); }
        [CsvHelper.Configuration.Attributes.Index(2)]
        public float Hours { get; set; }

    }
    class Program
    {
        static void Main(string[] args)
        {
            //for float format
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            //Reading file with CsvHelper
            IEnumerable<Employee_entry> records;
            using (var reader = new StreamReader("acme_worksheet.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                records = csv.GetRecords<Employee_entry>().ToList();
            }
            //Getting unique names and dates
            IEnumerable<DateTime> dates = Enumerable.Empty<DateTime>();
            IEnumerable<string> names = Enumerable.Empty<string>();
            foreach (var item in records)
            {
                dates = dates.Concat(new[] { item.Date });
                names = names.Concat(new[] { item.Name });
            }
            var unique_dates = dates.Distinct();
            var unique_names = names.Distinct();

            //dictionary (key: string name, value: List<float> hours)
            Dictionary<string, List<float>> dicWorkHours = new Dictionary<string, List<float>>();
            
            foreach (var name in unique_names)
            {
                //Employee working hours 
                List<float> tempHours = new List<float>();
                foreach (var date in unique_dates)
                {

                    var employeeByDates = from record in records
                                     where record.Name == name && record.Date == date
                                     select record;
                  
                    try
                    {
                        tempHours.Add(employeeByDates.First().Hours);
                    }
                    catch (Exception)
                    {
                        tempHours.Add(0);
                    }
                }
                //add values to dicWorkHours
                dicWorkHours.Add(name, tempHours);
            }

            /*
             * Creating new format for new CSV-file
             */
            //Writing headers with dates 
            string newText = "Name / Date";
            foreach (var date in unique_dates)
            {
                newText += "," + date.ToString("yyyy-MM-dd");
            }
            newText += "\n";
            //Writing headers with dates 
            foreach (var name in unique_names)
            {
                newText += name;
                foreach (var hours in dicWorkHours[name])
                {
                    newText += "," + hours;
                }
                newText += "\n";
            }
            //Writing new File
            File.WriteAllText("newCsv.csv", newText);

            
        }
       

    }


}
