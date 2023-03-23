using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormManagerBack.Controllers.Manager
{
    //Контроллер для назначения форм опрашиваемым
    [Route("api/[controller]")]
    [ApiController]
    public class FormAppointerController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public FormAppointerController(IConfiguration config)
        {
            this.configuration = config;
        }

        //Назначить определенную форму заданному отправляющему
        //Пример запроса: /api/formappointer?form_id=3&respondent_id=1
        [HttpPost]
        public JsonResult Post(int form_id, int respondent_id)
        {
            
            if(AppointForm(form_id, respondent_id))
            {
                return new JsonResult("Post Success");
            }
            else
            {
                return new JsonResult("Post Error");
            }
            
        }


        //Функция для создания в БД записи о назначении формы опрашиваемому
        public bool AppointForm(int form_id, int interviewee_id)
        {
            var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
            conn.Open();
            var command = new MySqlCommand(@$"INSERT INTO `project_bd`.`intervieweexform` ( `id_form`, `id_int`)
VALUES (@Form, @User);", conn);

            command.Parameters.AddWithValue("@Form", form_id);
            command.Parameters.AddWithValue("@User", interviewee_id);

            try
            {
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
