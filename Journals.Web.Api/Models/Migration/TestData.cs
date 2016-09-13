using System;
using System.Collections.Generic;
using FluentMigrator;
using journals.commons.Util;

namespace Journals.Web.Api.Models.Migration {

    [Migration(201609121900)]
    public class TestData : FluentMigrator.Migration {



        public override void Up() {

            var adminPass = AuthUtils.GetSha1HashData("admin");
            var userPass = AuthUtils.GetSha1HashData("user");

            Insert.IntoTable("user").Row(new {
                Login = "admin", Email = "luizhenrique.rolim@gmail.com", Password = adminPass, Publisher = true
            });
            Insert.IntoTable("user").Row(new {
                Login = "user", Email = "luiz@gmail.com", Password = userPass, Publisher = false
            });


            Insert.IntoTable("journal").Row(new {
                Name = "New England Journal of Medicine", Description = @"English 
ISSN: 0028 - 4793
EISSN: 1533 - 4406
ISI Impact Factor: 55.873
Free after 6 months
1993 - present

Only certain sections are free after 6 months.", RegisterDate = DateTime.Now
            });



            Insert.IntoTable("journal").Row(new {
                Name = "BMJ (British Medical Journal)", Description = @"English 
ISSN: 0959-8138
EISSN: 1756-1833
ISI Impact Factor: 17.445
FREE
1988 - 2008

Title was 'British Medical Journal (Clinical research ed.)' until 1987.", RegisterDate = DateTime.Now
            });

        }

        public override void Down() {
        }
    }

}