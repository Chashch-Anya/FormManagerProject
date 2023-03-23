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
    //Контроллер для обработки ответов отправляющим
    [Route("api/[controller]")]
    [ApiController]
    public class ResponseController : ControllerBase
    {

        private readonly IConfiguration configuration;

        public ResponseController(IConfiguration config)
        {
            this.configuration = config;
        }

        //Просмотреть информации об ответе по его ID
        //Пример запроса: /api/response/1
        [HttpGet("{repl_id}")]
        public JsonResult Get(int repl_id)
        {
            JsonResult response = new JsonResult("");

            if (GetDbResponse(repl_id, ref response))
                return response;
            else
                return new JsonResult("Not Found");
        }


        //Получить список ответов опрашиваемого
        //Пример запроса: /api/response?user_id=1
        [HttpGet]
        public JsonResult GetByUser(int user_id)
        {
            JsonResult response = new JsonResult("");

            if (GetDbUserResponse(user_id, ref response))
                return response;
            else
                return new JsonResult("Not Found");
        }


        //Записать ответ на определенную форму
        //Пример запроса: /api/response?user_id=1
        [HttpPost]
        public JsonResult Post(int id_form, int id_int, string reply_content)
        {

            DateTime date = DateTime.Now;

            var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
            conn.Open();
            var command = new MySqlCommand(@$"INSERT INTO `project_bd`.`reply` ( `id_form`, `id_int`, `text`, `date`) 
VALUES ( @Form, @Respondent, @Content, @Date);", conn);

            command.Parameters.AddWithValue("@Form", id_form);
            command.Parameters.AddWithValue("@Respondent", id_int);
            command.Parameters.AddWithValue("@Content", reply_content);
            command.Parameters.AddWithValue("@Date", date);

            try
            {
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception)
            {
                return new JsonResult("Post Error");
            }

            return new JsonResult("Post Success");
        }


        //Функция для поиска ответа в БД по его ID
        private bool GetDbResponse(int notif_id, ref JsonResult result)
        {
            try
            {
                var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
                conn.Open();
                var command = new MySqlCommand(@$"select * FROM project_bd.reply as rpl WHERE rpl.id_rpl = @ID", conn);
                command.Parameters.AddWithValue("@ID", notif_id);

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

        //Функция для формирования списка ответов определенного опрашиваемого (по ID опрашиваемого)
        private bool GetDbUserResponse(int user_id, ref JsonResult result)
        {
            try
            {
                var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
                conn.Open();
                var command = new MySqlCommand(@$"select * FROM project_bd.reply as rpl WHERE rpl.id_int = @ID", conn);
                command.Parameters.AddWithValue("@ID", user_id);

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
