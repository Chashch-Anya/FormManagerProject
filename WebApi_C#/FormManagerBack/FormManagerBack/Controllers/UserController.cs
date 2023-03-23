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
    //Контроллер, ответственный за авторизацию пользователя в системе
    //Производит поиск аккаунта по трем таблицам в БД

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public UserController(IConfiguration config)
        {
            this.configuration = config;
        }

        //Запрос на поиск данных учетной записи в базе данных
        //Пример запроса: /api/user?email=Ivanon@mail.ru&pass=000000
        [HttpGet]
        public JsonResult Get(string email, string pass)
        {
            JsonResult response = new JsonResult("");
            
            //Поиск по роли администратора
            if (GetDbUserAccount(email, pass, "admin_account", ref response))
                return response;
            
            //Поиск по роли менеджера
            if (GetDbUserAccount(email, pass, "manager_account", ref response))
                return response;

            //Поиск по роли опрашиваемого
            if (GetDbUserAccount(email, pass, "interviewee_account", ref response))
                return response;

            return new JsonResult("Not Found");
        }


        //Функция, осуществяющая поиск учетной записи в заданной таблице
        private bool GetDbUserAccount(string email, string pass, string tableName, ref JsonResult result)
        {
            try
            {
                var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
                conn.Open();
                var command = new MySqlCommand($"select * FROM project_bd.employee as emp join {tableName} as acc on emp.id_emp = acc.id_emp where emp.email = @Email and acc.password = @Pass limit 1", conn);

                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Pass", pass);

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
