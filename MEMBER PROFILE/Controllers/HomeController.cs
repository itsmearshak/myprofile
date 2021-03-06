using MEMBER_PROFILE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;

namespace MEMBER_PROFILE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SqlConnection db;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();


        }
        [HttpPost]
        public IActionResult Index(string name,string email  , string phone ,List<IFormFile> choosefile)
        {
            string pic = "";
            if (choosefile.Count>0)
            {
                var bytes = ReadFully(choosefile[0].OpenReadStream());
                pic = Convert.ToBase64String(bytes);
            }
               
            var db =new SqlConnection(@"server=LAPTOP-1FPV9DFV\ARSHAK;Integrated Security = True;database=memberdata"); 
           
            db.Open();
            var cmd = new SqlCommand($"INSERT INTO dbo.datatable(name,email,phone,image) VALUES ('{name}', '{email}', '{phone}','{pic}')", db);
            cmd.ExecuteNonQuery();
            

            var p = new Profile();



           
            using (var command = new SqlCommand($"SELECT * FROM dbo.datatable WHERE name = '{name}' ", db))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        p.Id = reader.GetInt32(0);
                        p.Pic = reader.GetString(1);
                        p.Name = reader.GetString(2);
                        p.Phone = reader.GetString(3);
                        p.Email = reader.GetString(4);                 }
                }
            }
            db.Close();
            return View("Index3", p);
        }
        public IActionResult Privacy()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Privacy( int id)
        {
            
           
            var db = new SqlConnection(@"server=LAPTOP-1FPV9DFV\ARSHAK;Integrated Security = True;database=memberdata");

            var p = new Profile();
            


            db.Open();
            using (var command = new SqlCommand($"SELECT * FROM dbo.datatable WHERE id = '{id}' ", db))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        p.Id = reader.GetInt32(0);
                        p.Pic = reader.GetString(1);
                        p.Name = reader.GetString(2);
                        p.Phone = reader.GetString(3);
                        p.Email = reader.GetString(4);
                    }
                }
            }
            db.Close();
            return View("Index1",p);
        }
        [HttpPost]
        public IActionResult Index1(string name,string email ,string phone,int id ,List<IFormFile> choosefile)
        {
            string pic = "";
            if (choosefile.Count > 0)
            {
                var bytes = ReadFully(choosefile[0].OpenReadStream());
                pic = Convert.ToBase64String(bytes);
            }
            var db = new SqlConnection (@"server=LAPTOP-1FPV9DFV\ARSHAK;Integrated Security = True;database=memberdata");
            db.Open();
            var cmd = new SqlCommand($"UPDATE dbo.datatable SET  email='{email}',phone='{phone}',image='{pic}',name='{name}' WHERE id='{id}'", db);
            cmd.ExecuteNonQuery();
            db.Close();
            return View("Index2");
        }
        [HttpPost]
        public IActionResult Index3()
        {
            return View("Index");
        }
        [HttpPost]
      
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
