using AkilliSayac.Areas.Identity.Data;
using AkilliSayac.Data;
using AkilliSayac.Interfaces;
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
    public class LogOperation : ILogOperation
    {
        public static void GetLogsFromFile(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            string folderPath = Path.Combine(hostingEnvironment.WebRootPath, @"logs/");
            if (db.Logs.Count() == 0)
            {
                GetSystemLogsFromFile(folderPath + "logsnx.txt", db);
                GetOtherLogsFromFile(folderPath + "cihaztanigunlugu.txt", db);
            }
        }

        private static void GetSystemLogsFromFile(string filePath, ApplicationDbContext db)
        {
            string pattern = @"(.*)\sP.*-..'(\b.{5}\b)':\s(.*)";
            Regex regex = new Regex(pattern);

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    Match match = regex.Match(line);
                    if (match.Success)
                    {
                        string format = "MMM d HH:mm:ss";

                        string dateString = match.Groups[1].Value;
                        dateString = dateString.Replace("  ", " ");
                        string deviceName = match.Groups[2].Value;
                        string message = match.Groups[3].Value;
                        DateTime date = DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None);


                        Log log = new Log();
                        log.LogMessage = message;
                        log.LogTime = date;
                        log.DeviceId = db.Devices.Where(x => x.DeviceName == deviceName).FirstOrDefault().DeviceId;
                        log.LogTypeId = db.LogTypes.Where(x => x.LogTypeName == "Sistem").FirstOrDefault().LogTypeId;
                        log.LogStatus = "Sistem";
                        log.LogStatusBadge = "badge bg-warning text-dark";

                        db.Logs.AddAsync(log);
                    }
                }
                db.SaveChanges();
            }
        }

        private static void GetOtherLogsFromFile(string filePath, ApplicationDbContext db)
        {
            string pattern = @"=""\s(.*)....""\s=""\s(.*)""\s=""\s(.*)""";
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
                        int messageNumber = int.Parse(match.Groups[2].Value);
                        string message = match.Groups[3].Value;
                        DateTime date = DateTime.ParseExact(dateString, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                        Log log = new Log();
                        log.LogTime = date;
                        log.DeviceId = 1;
                        log.LogTypeId = db.LogTypes.Where(x => x.LogMessageNumber == messageNumber).FirstOrDefault().LogTypeId;
                        log.LogMessage = message;
                        if (messageNumber == 2112)
                        {
                            log.LogStatusBadge = "badge bg-success";
                            log.LogStatus = "Başarılı";
                        }
                        else
                        {
                            log.LogStatusBadge = "badge bg-danger";
                            log.LogStatus = "Arıza";
                        }

                        db.Logs.Add(log);
                    }
                }
                db.SaveChanges();
            }
        }
    }
}
