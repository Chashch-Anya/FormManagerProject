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
    //Контроллер, ответственный за получение опрашиваемым списка форм, которые ему были назначены

    [Route("api/[controller]")]
    [ApiController]
    public class IntervieweeFormListController : ControllerBase
    {

        private readonly IConfiguration configuration;

        public IntervieweeFormListController(IConfiguration config)
        {
            this.configuration = config;
        }

        //Получение списка форм, назначенных опрашиваемому
        //Пример запроса: /api/Intervieweeformlist?user_id=1
        [HttpGet]
        public JsonResult Get(int user_id)
        {
            var res = new JsonResult("");
            if (GetDbForm(user_id, ref res))
            {
                return res;
            }
            else
            {
                return new JsonResult("Not Found");
            }
            /*return new JsonResult("GetResult");*/
        }

        //Функция для поиска форм в базе данных по ID опрашиваемого
        private bool GetDbForm(int id, ref JsonResult result)
        {
            try
            {
                var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
                conn.Open();
                var command = new MySqlCommand(@$"select *
FROM project_bd.form as form join intervieweexform as appointer on form.id_form = appointer.id_form
WHERE appointer.id_int = @ID", conn);
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
