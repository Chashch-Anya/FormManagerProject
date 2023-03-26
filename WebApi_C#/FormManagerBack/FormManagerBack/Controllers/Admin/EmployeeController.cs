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
    //Контроллер для работы со списком сотрудников
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public EmployeeController(IConfiguration config)
        {
            this.configuration = config;
        }

        //Получить информацию о сотруднике
        //Пример запроса: /api/employee/1
        [HttpGet("{emp_id}")]
        public JsonResult Get(int emp_id)
        {
            JsonResult response = new JsonResult("");

            if (GetDbEmployee(emp_id, ref response))
                return response;
            else
                return new JsonResult("Not Found");
        }


        //Получить полный список сотрудников
        //Пример запроса: /api/employee
        [HttpGet]
        public JsonResult GetAll()
        {
            JsonResult response = new JsonResult("");

            if (GetDbEmployee(ref response))
                return response;
            else
                return new JsonResult("Not Found");
        }


        //Создать нового сотрудника
        //Пример запроса: /api/employee?name=ViktorKomarov&id_pos=1&email=VKomarov@gmal.com
        [HttpPost]
        public JsonResult Post(string name, int id_pos, string email)
        {
            if (CreateEmployee(name, id_pos, email))
                return new JsonResult("Post Succsess");
            else
                return new JsonResult("Post Fail");

        }

        //Редактировать данные сотрудника
        //Пример запроса: /api/employee/2?name=ViktorKomarov [Updated]&id_pos=1&email=VKomarov@gmal.com
        [HttpPut("{emp_id}")]
        public JsonResult Put(int emp_id,string name, int id_pos, string email)
        {
            if (UpdateEmployee(emp_id, name, id_pos, email))
                return new JsonResult("Put Succsess");
            else
                return new JsonResult("Put Fail");
        }

        //Удалить сотрудника
        //Присер запроса:
        [HttpDelete("{emp_id}")]
        public JsonResult Delete(int emp_id)
        {
            var temp = new JsonResult("");
            if (GetDbEmployee(emp_id, ref temp))
            {
                if (DeleteEmployee(emp_id))
                {
                    return new JsonResult("Delete Employee Success");
                }
                else
                {
                    return new JsonResult("Delete Employee Error");
                }
            }
            else
            {
                return new JsonResult($"Employee Not Found");
            }
        }

        private bool GetDbEmployee(int id, ref JsonResult result)
        {
            try
            {
                var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
                conn.Open();
                var command = new MySqlCommand(@$"select emp.id_emp, emp.fullname, position.name as position, emp.email
FROM project_bd.employee as emp join position on emp.id_pos = position.id_pos
WHERE emp.id_emp = @ID limit 1", conn);
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


        private bool GetDbEmployee( ref JsonResult result)
        {
            try
            {
                var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
                conn.Open();
                var command = new MySqlCommand(@$"select emp.id_emp, emp.fullname, position.name as position, emp.email
FROM project_bd.employee as emp join position on emp.id_pos = position.id_pos", conn);
                

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

        private bool CreateEmployee(string name, int position_id, string email)
        {

            var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
            conn.Open();
            var command = new MySqlCommand(@$"INSERT INTO `project_bd`.`employee` ( `fullname`, `id_pos`, `email`) 
VALUES ( @Name, @Position, @Email);", conn);

            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@Position", position_id);
            command.Parameters.AddWithValue("@Email", email);

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


        private bool UpdateEmployee(int id, string name, int position_id, string email)
        {
            var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
            conn.Open();
            var command = new MySqlCommand(@"UPDATE `project_bd`.`employee` as emp
SET emp.fullname = @Name,
emp.id_pos = @Position,
emp.email = @Email
WHERE emp.id_emp = @ID;", conn);

            command.Parameters.AddWithValue("@ID", id);
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@Position", position_id);

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

        private bool DeleteEmployee(int id)
        {
            
            var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
            conn.Open();
            var command = new MySqlCommand("DELETE FROM `project_bd`.`employee` as emp WHERE emp.id_emp = @ID", conn);
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
