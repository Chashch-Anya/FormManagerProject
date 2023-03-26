using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FormManagerBack.Controllers.Admin
{
    //Контроллер для получения списка должностей
    [Route("api/[controller]")]
    [ApiController]
    public class PositionController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public PositionController(IConfiguration config)
        {
            this.configuration = config;
        }

        //Получить список должностей
        //Пример запроса: /api/position
        [HttpGet]
        public JsonResult Get()
        {
            JsonResult response = new JsonResult("");

            if (GetDbPositions(ref response))
                return response;
            else
                return new JsonResult("Not Found");
        }


        private bool GetDbPositions(ref JsonResult result)
        {
            try
            {
                var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
                conn.Open();
                var command = new MySqlCommand(@$"select pos.id_pos as id, pos.name as positionName FROM project_bd.position as pos", conn);

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
