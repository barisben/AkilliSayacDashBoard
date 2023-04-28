using AkilliSayac.Areas.Identity.Data;
using AkilliSayac.Data;
using AkilliSayac.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AkilliSayac.Data
{
    [Authorize]
    public static class AnomalyOperation
    {
        public static void GetAnomaliesFromFile(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {   
            if (db.Anomalies.Count() == 0)
            {
                string filePath = Path.Combine(hostingEnvironment.WebRootPath, @"logs/") + "randanomalylog.txt";
                string pattern = @"(.*)\s-\s(.*)\s-\s(.*)\s-\s(.*)\s-\s(.*)";
                Regex regex = new Regex(pattern);

                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (string line in lines)
                    {
                        Match match = regex.Match(line);
                        if (match.Success)
                        {
                            string dateString = match.Groups[1].Value;
                            string deviceName = match.Groups[2].Value;
                            int anomalyValue = int.Parse(match.Groups[3].Value);
                            string type = match.Groups[4].Value;
                            string message = match.Groups[5].Value;
                            DateTime date = DateTime.ParseExact(dateString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None);


                            Anomaly anomaly = new Anomaly();
                            anomaly.AnomalyTime = date;
                            anomaly.AnomalyValue = anomalyValue;
                            anomaly.AnomalyMessage = message;
                            anomaly.DeviceId = db.Devices.Where(x => x.DeviceName == deviceName).FirstOrDefault().DeviceId;
                            anomaly.AnomalyTypeId = db.AnomalyTypes.Where(x => x.AnomalyTypeName == type).FirstOrDefault().AnomalyTypeId;

                            db.Anomalies.AddAsync(anomaly);
                        }
                    }
                    db.SaveChanges();
                }
            }
        }
    }
}
