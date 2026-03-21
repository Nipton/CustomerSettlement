using AccountsReceivable.Models;
using AccountsReceivable.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace AccountsReceivable.Services
{
    public enum PathFile
    {
        nomenclature,
        unit,
        category,
    }
    class FileIOService
    {
        
        private readonly string PATH = $"{Environment.CurrentDirectory}\\companiesList.xml";
        private readonly string PATH_ORGANIZATION = $"{Environment.CurrentDirectory}\\organization.xml";
        //private readonly string PATH_NOMENCLATURE = $"{Environment.CurrentDirectory}\\nomenclature.txt";
        //private readonly string PATH_UNIT = $"{Environment.CurrentDirectory}\\unit.txt";
        private static int MaxID;      
        public void SaveData(CompanyOld company)
        {
            XDocument xDocument;

            if (!File.Exists(PATH))
            {

                MaxID = 1;
                company.ID = 1;
                xDocument = new XDocument(new XElement("Companies",
                    new XElement("CompanyOld",
                    new XElement("ID", MaxID),
                    new XElement("Name", company.Name),
                    new XElement("ShortName", company.ShortName),
                    new XElement("Inn", company.Inn),
                    new XElement("Kpp", company.Kpp),
                    new XElement("LegalAddress", company.LegalAddress),
                    new XElement("ActualAddress", company.ActualAddress),
                    new XElement("Ogrn", company.Ogrn),
                    new XElement("Bank", company.Bank),
                    new XElement("Rs", company.Rs),
                    new XElement("Bik", company.Bik),
                    new XElement("Ks", company.Ks),
                    new XElement("Phone", company.Phone),
                    new XElement("Category", company.Category),
                    new XElement("Position", company.Position),
                    new XElement("DirectorFullName", company.DirectorFullName))));
            }
            else
            {
                MaxID += 1;
                company.ID = MaxID;
                xDocument = XDocument.Load(PATH);
                xDocument.Root?.Add(new XElement("CompanyOld",
                    new XElement("ID", MaxID),
                    new XElement("Name", company.Name),
                    new XElement("ShortName", company.ShortName),
                    new XElement("Inn", company.Inn),
                    new XElement("Kpp", company.Kpp),
                    new XElement("LegalAddress", company.LegalAddress),
                    new XElement("ActualAddress", company.ActualAddress),
                    new XElement("Ogrn", company.Ogrn),
                    new XElement("Bank", company.Bank),
                    new XElement("Rs", company.Rs),
                    new XElement("Bik", company.Bik),
                    new XElement("Ks", company.Ks),
                    new XElement("Phone", company.Phone),
                    new XElement("Category", company.Category),
                    new XElement("Position", company.Position),
                    new XElement("DirectorFullName", company.DirectorFullName)));
            }
            xDocument.Save(PATH);
        }

        public ObservableCollection<CompanyOld> LoadData()
        {
            bool fileExists = File.Exists(PATH);
            if (!fileExists)
            {
                return new ObservableCollection<CompanyOld>();
            }
            else
            {
                XDocument xDocument = XDocument.Load(PATH);

                var tempList = xDocument.Root?.Elements("CompanyOld").Select(x => new CompanyOld
                {
                    ID = x.Element("ID")?.Value == null ? 0 : int.Parse(x.Element("ID")?.Value),
                    Name = x.Element("Name")?.Value,
                    ShortName = x.Element("ShortName")?.Value,
                    Inn = x.Element("Inn")?.Value,
                    Kpp = x.Element("Kpp")?.Value,
                    LegalAddress = x.Element("LegalAddress")?.Value,
                    ActualAddress = x.Element("ActualAddress")?.Value,
                    Ogrn = x.Element("Ogrn")?.Value,
                    Bank = x.Element("Bank")?.Value,
                    Rs = x.Element("Rs")?.Value,
                    Bik = x.Element("Bik")?.Value,
                    Ks = x.Element("Ks")?.Value,
                    Phone = x.Element("Phone")?.Value,
                    Category = x.Element("Category")?.Value,
                    Position = x.Element("Position")?.Value,
                    DirectorFullName = x.Element("DirectorFullName")?.Value
                }).ToList();


                MaxID = tempList.LastOrDefault().ID;
                return new ObservableCollection<CompanyOld>(tempList);
            }
        }

        public void EditData(CompanyOld company)
        {
            XDocument xDocument = XDocument.Load(PATH);
            var editableCompany = xDocument.Root?.Elements("CompanyOld").FirstOrDefault(x => x.Element("ID")?.Value == company.ID.ToString());

            if (editableCompany != null)
            {
                editableCompany.Element("Name").Value = company.Name ?? "";
                editableCompany.Element("ShortName").Value = company.ShortName ?? "";
                editableCompany.Element("Inn").Value = company.Inn ?? "";
                editableCompany.Element("Kpp").Value = company.Kpp ?? "";
                editableCompany.Element("LegalAddress").Value = company.LegalAddress ?? "";
                editableCompany.Element("ActualAddress").Value = company.LegalAddress ?? "";
                editableCompany.Element("Ogrn").Value = company.Ogrn ?? "";
                editableCompany.Element("Bank").Value = company.Bank ?? "";
                editableCompany.Element("Rs").Value = company.Rs ?? "";
                editableCompany.Element("Bik").Value = company.Bik ?? "";
                editableCompany.Element("Ks").Value = company.Ks ?? "";
                editableCompany.Element("Phone").Value = company.Phone ?? "";
                editableCompany.Element("Category").Value = company.Category ?? "";
                editableCompany.Element("Position").Value = company.Position ?? "";
                editableCompany.Element("DirectorFullName").Value = company.DirectorFullName ?? "";
                xDocument.Save(PATH);
            }
        }
        public void RemoveData(ObservableCollection<CompanyOld> entireCollection, List<CompanyOld> selectedList)
        {
            XDocument xDocument = XDocument.Load(PATH);
            foreach (CompanyOld company in selectedList)
            {
                var deletedCompany = xDocument.Root?.Elements("CompanyOld").FirstOrDefault(x => x.Element("ID")?.Value == company.ID.ToString());
                if(deletedCompany != null)
                {
                    deletedCompany.Remove();
                    entireCollection.Remove(company);
                }
            }
            MaxID = entireCollection.LastOrDefault().ID;
            xDocument.Save(PATH);
        }

        public void SaveOrganization(CompanyOld company)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CompanyOld));
            using(FileStream fs = new FileStream(PATH_ORGANIZATION, FileMode.Create))
            {
                serializer.Serialize(fs, company);
            }
        }
        public CompanyOld LoadOrganization()
        {
            CompanyOld company = new CompanyOld();
            bool fileExists = File.Exists(PATH_ORGANIZATION);
            if (!fileExists)
            {
                return new CompanyOld();
            }
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CompanyOld));
            using(FileStream fs = new FileStream(PATH_ORGANIZATION, FileMode.OpenOrCreate))
            {
                company = xmlSerializer.Deserialize(fs) as CompanyOld;
            }
            return company;
        }
        public Company LoadOrganization2()
        {
            Company company = new Company();
            bool fileExists = File.Exists(PATH_ORGANIZATION);
            if (!fileExists)
            {
                return new Company();
            }
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Company));
            using (FileStream fs = new FileStream(PATH_ORGANIZATION, FileMode.OpenOrCreate))
            {
                company = xmlSerializer.Deserialize(fs) as Company;
            }
            return company;
        }

        public void AddToTextFile(string nomenclature, PathFile path)
        {
            using(StreamWriter writer = new StreamWriter($"{Environment.CurrentDirectory}\\{path}.txt", true))
            {
                writer.WriteLine(nomenclature);
            }
        }
        public List<string> LoadFromTextFile(PathFile path)
        {
             
            List<string> list = new List<string> ();
            if (!File.Exists($"{Environment.CurrentDirectory}\\{path}.txt"))
            {
                return list;
            }
            using (StreamReader reader = new StreamReader($"{Environment.CurrentDirectory}\\{path}.txt"))
            {
                while(reader.Peek() != -1)
                {
                    list.Add(reader.ReadLine());
                }
            }
            return list;
        }

        public void DeleteFromTextFile(List<string> list, PathFile path)
        {
            using (StreamWriter writer = new StreamWriter($"{Environment.CurrentDirectory}\\{path}.txt", false))
            {
                foreach (var item in list)
                {
                    writer.WriteLine(item);
                }
            }
        }
    }
}
