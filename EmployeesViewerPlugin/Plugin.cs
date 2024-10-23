using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PhoneApp.Domain;
using PhoneApp.Domain.Attributes;
using PhoneApp.Domain.DTO;
using PhoneApp.Domain.Interfaces;

namespace EmployeesLoaderPlugin
{

  [Author(Name = "Ivan Petrov")]
  public class Plugin : IPluggable
  {
    private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
    public IEnumerable<DataTransferObject> Run(IEnumerable<DataTransferObject> args)
    {
      logger.Info("Starting Viewer");
      logger.Info("Type q or quit to exit");
      logger.Info("Available commands: list, add, del");

      var employeesList = args.Cast<EmployeesDTO>().ToList();

      string command = "";

      while(command != "quit" && command != "q")
      {
        Console.Write("> ");
        command = Console.ReadLine().ToLower();

        switch(command)
        {
          case "list":
            int index = 0;
            foreach(var employee in employeesList)
            {
              Console.WriteLine($"{index} Name: {employee.Name} | Phone: {employee.Phone}");
              ++index;
            }
            break;
          case "add":
            Console.Write("Name: ");
            string name = Console.ReadLine();
            if (!name.All(l => char.IsLetter(l) || l == ' ') || name.Length == 0)
            {
                logger.Error("Invalid employee name!");
            } else
              {
              Console.Write("Phone: ");
              string phone = Console.ReadLine();
              if (!phone.All(l => char.IsDigit(l) || l == ' ' || l == '+' || l == '-') || phone.Length == 0)
              {
                logger.Error("Invalid phone number!");
              } else
                {
                  var newEmployee = new EmployeesDTO() { Name = name };
                  newEmployee.AddPhone(phone);
                  employeesList.Add(newEmployee);
                  Console.WriteLine($"{name} added to employees");
                }
              }                        
            break;
          case "del":
            Console.Write("Index of employee to delete: ");
            int indexToDelete;
            if(!Int32.TryParse(Console.ReadLine(), out indexToDelete))
            {
              logger.Error("Not an index or not an int value!");
            } else {
              if(indexToDelete >= 0 && indexToDelete < employeesList.Count())
              {
                employeesList.RemoveAt(indexToDelete);
              }
              else
              {
                logger.Error("No employee was found under this index");
              }
            }
            break;
        }

        Console.WriteLine("");
      }

      return employeesList.Cast<DataTransferObject>();
    }
  }
}
