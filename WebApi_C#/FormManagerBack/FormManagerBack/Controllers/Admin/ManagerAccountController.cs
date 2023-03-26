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
    //Контроллер для работы с учётными записями менеджера форм
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerAccountController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public ManagerAccountController(IConfiguration config)
        {
            this.configuration = config;
        }

        //Получить информацию об аккаунте опрашиваемого
        //Пример запроса: /api/manageraccount/1
        [HttpGet("{acc_id}")]
        public JsonResult Get(int acc_id)
        {
            JsonResult response = new JsonResult("");

            if (GetDbManager(acc_id, ref response))
                return response;
            else
                return new JsonResult("Not Found");
        }

        //Получить список аккаутов менеджеров
        //Пример запроса: /api/manageraccount
        [HttpGet]
        public JsonResult GetAll()
        {
            JsonResult response = new JsonResult("");

            if (GetDbManager(ref response))
                return response;
            else
                return new JsonResult("Not Found");
        }

        //Создать новую учетную запись менеджера
        //Пример запроса: 
        [HttpPost]
        public JsonResult Post(int id_emp, string pass)
        {
            if (CreateManager(id_emp, pass))
                return new JsonResult("Post Succsess");
            else
                return new JsonResult("Post Fail");

        }


        //Редактировать данные учётной записи менеджера
        //Пример запроса: 
        [HttpPut("{acc_id}")]
        public JsonResult Put(int acc_id, int emp_id, string pass)
        {
            if (UpdateManager(acc_id, emp_id, pass))
                return new JsonResult("Put Succsess");
            else
                return new JsonResult("Put Fail");
        }

        //Удалить аккаунт опрашиваемого
        //Присер запроса:
        [HttpDelete("{acc_id}")]
        public JsonResult Delete(int acc_id)
        {
            var temp = new JsonResult("");
            if (GetDbManager(acc_id, ref temp))
            {
                if (DeleteManager(acc_id))
                {
                    return new JsonResult("Delete Manager Success");
                }
                else
                {
                    return new JsonResult("Delete Manager Error");
                }
            }
            else
            {
                return new JsonResult($"Manager Not Found");
            }
        }


        private bool GetDbManager(int id, ref JsonResult result)
        {
            try
            {
                var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
                conn.Open();
                var command = new MySqlCommand(@$"select acc.id_mngr as account_id, acc.password as pass, emp.fullname, position.name as position, emp.email
FROM project_bd.employee as emp join position on emp.id_pos = position.id_pos join manager_account as acc on acc.id_emp = emp.id_emp
WHERE acc.id_mngr = @ID limit 1", conn);
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


        private bool GetDbManager(ref JsonResult result)
        {
            try
            {
                var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
                conn.Open();
                var command = new MySqlCommand(@$"select acc.id_mngr as account_id, acc.password as pass, emp.fullname, position.name as position, emp.email
FROM project_bd.employee as emp join position on emp.id_pos = position.id_pos join manager_account as acc on acc.id_emp = emp.id_emp", conn);

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

        private bool CreateManager(int employee_id, string password)
        {

            var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
            conn.Open();
            var command = new MySqlCommand(@$"INSERT INTO `project_bd`.`manager_account` ( `id_emp`, `password`) 
VALUES ( @Employee_ID, @Pass);", conn);

            command.Parameters.AddWithValue("@Employee_ID", employee_id);
            command.Parameters.AddWithValue("@Pass", password);

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

        private bool UpdateManager(int account_id, int id_employee, string password)
        {
            var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
            conn.Open();
            var command = new MySqlCommand(@"UPDATE `project_bd`.`manager_account` as acc
SET acc.id_emp = @NewEmployee,
acc.password = @Pass
WHERE acc.id_mngr = @ID;", conn);

            command.Parameters.AddWithValue("@NewEmployee", id_employee);
            command.Parameters.AddWithValue("@Pass", password);
            command.Parameters.AddWithValue("@ID", account_id);

            try
            {
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool DeleteManager(int id)
        {

            var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
            conn.Open();
            var command = new MySqlCommand("DELETE FROM `project_bd`.`manager_account` as acc WHERE acc.id_mngr = @ID", conn);
            command.Parameters.AddWithValue("@ID", id);

            try
            {
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch
            {
                return false;
            }

            return true;
        }

    }
}
