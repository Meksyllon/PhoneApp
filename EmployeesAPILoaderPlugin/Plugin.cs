using PhoneApp.Domain.Attributes;
using PhoneApp.Domain.DTO;
using PhoneApp.Domain.Interfaces;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EmployeesAPILoaderPlugin
{
    [Author(Name = "Maksim Martynov")]
    public class Plugin : IPluggable
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public IEnumerable<DataTransferObject> Run(IEnumerable<DataTransferObject> args)
        {
            logger.Info("Loading employees from API");
            var streamReadData = GetDataFromAPI();

            dynamic employeesDynamic = JObject.Parse(streamReadData);
            var employeesList = employeesDynamic.users;

            var resultList = args.ToList();
            foreach (var employee in employeesList)
            {
                var employeeDto = new EmployeesDTO() { Name = $"{employee.firstName} {employee.lastName}" };
                var phone = employee.phone.ToString();
                employeeDto.AddPhone(phone);
                resultList.Add(employeeDto);
            }

            logger.Info($"Loaded {resultList.Count() - args.Count()} employees");

            return resultList;
        }

        private static string GetDataFromAPI()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var request = (HttpWebRequest)WebRequest.Create($"https://dummyjson.com/users");
            var response = (HttpWebResponse)request.GetResponse();
            var stream = response.GetResponseStream();
            var streamReader = new StreamReader(stream);

            string streamReadData = streamReader.ReadToEnd();
            response.Close();

            return streamReadData;
        }
    }
}
