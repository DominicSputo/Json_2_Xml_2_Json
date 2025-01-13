using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvDatabase;
using System.Text.Json;

namespace Json_2_Xml_2_Json
{
    public class Employee
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public List<DateTimeOffset> DatesAvailable { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //string sJSON_FileName = System.IO.Path.GetFullPath("JSON_Single_Record.txt");
            string sJSON_FileName = System.IO.Path.GetFullPath("JSON_Multi_Record.txt");
            try
            {
                JSON_Single_Record(sJSON_FileName);
            }
            catch
            {
                try
                {
                    JSON_Multi_Record(sJSON_FileName);
                }
                catch
                {

                }
            }
        }

        public static void JSON_Single_Record(string sJSON_FileName)
        {
            //Remove all data from CSV File - Cleanup
            CsvCommand.Truncate(System.IO.Path.GetFullPath("CSV.txt"));
            //Retrieve Json as String
            string sJson = System.IO.File.ReadAllText(sJSON_FileName);
            //Cast Json text to Employee class
            Employee cEmployee = JsonSerializer.Deserialize<Employee>(sJson);
            //Turn Dates Available into String seperated by commas
            string sDatseAvailable = "";
            foreach (DateTimeOffset dt in cEmployee.DatesAvailable)
            {   //Create CSV String of List<DateTimeOffset>
                sDatseAvailable += sDatseAvailable + dt + ",";
            } //Remove last Comma
            sDatseAvailable = sDatseAvailable.Substring(0, (sDatseAvailable.Length - 1));
            //Create CSV File using Json Employee Class
            CsvCommand.Insert(new string[] { cEmployee.Name, cEmployee.Age.ToString(), sDatseAvailable }, new string[] { "Name", "Age", "DatesAvailable" }, System.IO.Path.GetFullPath("CSV.txt"), 0);
            //Write Xml
            CsvToXml.WriteXml(System.IO.Path.GetFullPath("CSV.txt"), System.IO.Path.GetFullPath("XML.txt"));
            //Load CSV Data into Employee Class
            CsvDataReader dtr = new CsvDatabase.CsvDataReader(System.IO.Path.GetFullPath("CSV.txt"));
            Employee cWriteEmployeeJson = new Employee();
            while (dtr.Read())
            {
                List<DateTimeOffset> lstDatesAvailable = new List<DateTimeOffset>();
                string[] sarDatesAvailable = dtr["DatesAvailable"].ToString().Split(',');
                lstDatesAvailable.Add(DateTimeOffset.Parse(sarDatesAvailable[0] + ""));
                lstDatesAvailable.Add(DateTimeOffset.Parse(sarDatesAvailable[1] + ""));
                cWriteEmployeeJson = new Employee() { Name = dtr["Name"] + "", Age = int.Parse(dtr["Age"] + ""), DatesAvailable = lstDatesAvailable };
            }
            sJson = JsonSerializer.Serialize(cWriteEmployeeJson);
            //Write JSON String to JSON_Output.txt file
            System.IO.File.WriteAllText(System.IO.Path.GetFullPath("JSON_Output.txt"), sJson);
        }

        public static void JSON_Multi_Record(string sJSON_FileName)
        {
            //Remove all data from CSV File -Cleanup
            CsvCommand.Truncate(System.IO.Path.GetFullPath("CSV.txt"));
            //Retrieve Json as String
            string sJson = System.IO.File.ReadAllText(sJSON_FileName);
            //Cast Json String to List<Employee> class
            var lstEmployee = JsonSerializer.Deserialize<List<Employee>>(sJson);
            string sDatseAvailable = "";
            foreach (Employee e in lstEmployee)
            {
                foreach (DateTimeOffset dt in e.DatesAvailable)
                {   //Create CSV String of List<DateTimeOffset>
                    sDatseAvailable += sDatseAvailable + dt + ",";
                } //Remove last Comma
                sDatseAvailable = sDatseAvailable.Substring(0, (sDatseAvailable.Length - 1));
                CsvCommand.Insert(new string[] { e.Name, e.Age.ToString(), sDatseAvailable }, new string[] { "Name", "Age", "DatesAvailable" }, System.IO.Path.GetFullPath("CSV.txt"), 0);
                sDatseAvailable = "";
            }
            //Write Xml
            CsvToXml.WriteXml(System.IO.Path.GetFullPath("CSV.txt"), System.IO.Path.GetFullPath("XML.txt"));
            //Read CSV File into List<Employee> Class
            CsvDataReader dtr = new CsvDatabase.CsvDataReader(System.IO.Path.GetFullPath("CSV.txt"));
            List<Employee> lstWriteEmployeeJson = new List<Employee>();
            while (dtr.Read())
            {
                List<DateTimeOffset> lstDatesAvailable = new List<DateTimeOffset>();
                string[] sarDatesAvailable = dtr["DatesAvailable"].ToString().Split(',');
                lstDatesAvailable.Add(DateTimeOffset.Parse(sarDatesAvailable[0] + ""));
                lstDatesAvailable.Add(DateTimeOffset.Parse(sarDatesAvailable[1] + ""));
                lstWriteEmployeeJson.Add(new Employee() { Name = dtr["Name"] + "", Age = int.Parse(dtr["Age"] + ""), DatesAvailable = lstDatesAvailable });
            }
            sJson = JsonSerializer.Serialize(lstWriteEmployeeJson);
            //Write JSON String to JSON_Output.txt file
            System.IO.File.WriteAllText(System.IO.Path.GetFullPath("JSON_Output.txt"), sJson);
        }
    }
}