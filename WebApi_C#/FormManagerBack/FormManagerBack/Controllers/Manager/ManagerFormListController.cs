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
    //Контроллер, позволяющий менеджеру работать со своими формами
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerFormListController : ControllerBase
    {

        private readonly IConfiguration configuration;

        public ManagerFormListController(IConfiguration config)
        {
            this.configuration = config;
        }

        //Получить список форм по ID менеджера
        //Пример запроса: /api/managerformlist?id_mngr=1
        [HttpGet]
        public JsonResult Get(int id_mngr)
        {
            var res = new JsonResult("");
            if(GetDbForm(id_mngr, ref res))
            {
                return res;
            }
            else
            {
                return new JsonResult("Not Found");
            }

        }

        //Функция для формирования списка форм, созданных определенным менеджером
        private bool GetDbForm(int id, ref JsonResult result)
        {
            try
            {
                var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
                conn.Open();
                var command = new MySqlCommand($"select * FROM project_bd.form as form WHERE form.id_mngr = {id}", conn);
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
