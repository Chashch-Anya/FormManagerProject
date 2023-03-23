using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FormManagerBack.Controllers.Interviewee
{
    //Контроллер для получения информации о форме отвечающим

    [Route("api/[controller]")]
    [ApiController]
    public class IntervieweeFormController : ControllerBase
    {

        private readonly IConfiguration configuration;

        public IntervieweeFormController(IConfiguration config)
        {
            this.configuration = config;
        }

        //Получение данных о форме по ID формы
        //Пример запроса: /api/Intervieweeform?form_id=1
        [HttpGet]
        public JsonResult Get(int form_id)
        {
            JsonResult response = new JsonResult("");

            if (GetDbForm(form_id, ref response))
                return response;
            else
                return new JsonResult("Not Found");

            
        }

        //Функция для поиска формы в БД
        private bool GetDbForm(int id, ref JsonResult result)
        {
            try
            {
                var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
                conn.Open();
                var command = new MySqlCommand($"select * FROM project_bd.form as form WHERE form.id_form = @ID limit 1", conn);
                command.Parameters.AddWithValue("@ID", id);

                var reader = command.ExecuteReader();
                DataTable res = new DataTable();
                res.Load(reader);
                conn.Close();

                if (res.Rows.Count > 0)
                {
                    result = new JsonResult(res);
                    return true;
                }
                else return false;
            }
            catch
            {
                return false;
            }
        }

    }
}
