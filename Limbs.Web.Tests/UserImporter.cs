using System;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Text;
using Limbs.Web.Entities.DbContext;
using Limbs.Web.Entities.Models;
using Microsoft.SqlServer.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlServerTypes;

namespace Limbs.Web.Tests
{
    [TestClass]
    public class UserImporter
    {
        [TestMethod]
        public void ReadFromExcel()
        {
            Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);
            SqlProviderServices.SqlServerTypesAssemblyName = typeof(SqlGeography).Assembly.FullName;

            try
            {
                using (var context = new ApplicationDbContext())
            {
                    int i = 0;
                    var lines = File.ReadLines("c:\\temp\\requestor.txt", Encoding.UTF8);
                    foreach (var line in lines.Skip(1))
                    {
                        string[] vals = line.Split('\t');
                        if (string.IsNullOrWhiteSpace(vals[0])) continue;

                        var email = vals[0].Clean();
                        var pUser = context.UserModelsT.FirstOrDefault(x => x.Email.Equals(email));
                        if (pUser != null) continue;

                        var user = new UserModel
                        {
                            Email = vals[0].Clean(),
                            IsProductUser = bool.Parse(vals[1].Clean()),
                            UserName = vals[2].Clean(),
                            UserLastName = vals[3].Clean(),
                            ResponsableName = vals[4].Clean(),
                            ResponsableLastName = vals[5].Clean(),
                            Phone = string.IsNullOrWhiteSpace(vals[6].Clean()) ? "-" : vals[6].Clean(),
                            Birth = DateTime.Parse(vals[7].Clean()),
                            Gender = "hombre".Equals(vals[8].Clean()) ? Gender.Hombre : Gender.Mujer,
                            Country = vals[9].Clean(),
                            City = string.IsNullOrWhiteSpace(vals[10].Clean()) ? "-" : vals[10].Clean(),
                            Address = string.IsNullOrWhiteSpace(vals[11].Clean()) ? "-" : vals[11].Clean(),
                            Dni = string.IsNullOrWhiteSpace(vals[12].Clean()) ? "-" : vals[12].Clean(),
                            RegisteredAt = DateTime.Parse(vals[13].Clean()),
                            UserId = vals[0].Clean(),
                            State = "TEST",
                            Address2 = "TEST",
                            LatLng = "0.1,0.1",
                        };

                        if (string.IsNullOrWhiteSpace(user.UserLastName))
                        {
                            var s = vals[2].Clean().Split(' ');
                            user.UserLastName = s.Length > 1 ? s[s.Length - 1] : "-";
                            if (s.Length > 1)
                            {
                                user.UserName = user.UserName.TrimEnd(' ');
                                user.UserName = user.UserName.Remove(user.UserName.LastIndexOf(' ') + 1);
                            }
                        }

                        Console.Write("|" + i++);
                        context.UserModelsT.Add(user);
                    }

                context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public static class StringExtensions {

        public static string Clean(this string source)
        {
            return source.Trim().ToLowerInvariant();
        }
    }
}
