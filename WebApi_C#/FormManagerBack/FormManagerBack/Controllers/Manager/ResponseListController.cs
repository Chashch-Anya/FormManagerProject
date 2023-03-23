using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FormManagerBack.Controllers
{
    //Контроллер, ответственный за отправку информации об ответах на определенную форму
    [Route("api/[controller]")]
    [ApiController]
    public class ResponseListController : ControllerBase
    {

        private readonly IConfiguration configuration;

        public ResponseListController(IConfiguration config)
        {
            this.configuration = config;
        }

        //Получить список ответов на форму
        //Пример запроса: /api/responselist?form_id=1
        [HttpGet]
        public JsonResult Get(int form_id) 
        {

            JsonResult response = new JsonResult("");

            if (GetDbResponseList(form_id, ref response))
                return response;
            else
                return new JsonResult("Not Found");
        }


        //Функция для формирования списка ответов на определенную форму
        private bool GetDbResponseList(int form_id, ref JsonResult result)
        {
            try
            {
                var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
                conn.Open();
                var command = new MySqlCommand(@$"select * FROM project_bd.reply as rpl WHERE rpl.id_form = @ID", conn);
                command.Parameters.AddWithValue("@ID", form_id);

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
