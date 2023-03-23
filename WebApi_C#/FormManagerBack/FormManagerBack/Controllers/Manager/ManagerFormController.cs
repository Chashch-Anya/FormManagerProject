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
    //Контроллер, ответственный за работу менеджера форм с определенной формой

    [Route("api/[controller]")]
    [ApiController]
    public class ManagerFormController : ControllerBase
    {

        private readonly IConfiguration configuration;

        public ManagerFormController(IConfiguration config)
        {
            this.configuration = config;
        }

        //Получить данные о форме по её ID
        //Пример запроса: /api/managerform?form_id=1
        [HttpGet]
        public JsonResult Get(int form_id)
        {
            JsonResult response = new JsonResult("");

            if (GetDbForm(form_id, ref response))
                return response;
            else
            return new JsonResult("Not Found");
        }

        //Создание новой формы
        //Пример запроса: /api/managerform?id_mngr=1&title=TestForm&deadline=2023-03-23&description=HelloWorld&content={"h2": "Hello World"}
        [HttpPost]
        public JsonResult Post(int id_mngr, string title, string deadline, string description, string content)
        {
            // "INSERT INTO `project_bd`.`form` (`id_form`, `id_mngr`, `date`, `name`, `deadline`, `description`, `content`) VALUES ('2', '1', '2016-03-20 23:00:00', 'TestForm', '2023-03-20 23:00:00', 'This is a form to test API requests', '{\"h2\": \"Hello World\"}');";

            DateTime date = DateTime.Now;

            var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
            conn.Open();
            var command = new MySqlCommand($"INSERT INTO `project_bd`.`form` ( `id_mngr`, `date`, `name`, `deadline`, `description`, `content`) VALUES ( @manager_id, @date, @title, @deadline, @discription, @content);", conn);
            command.Parameters.AddWithValue("@manager_id", id_mngr);
            command.Parameters.AddWithValue("@date", date);
            command.Parameters.AddWithValue("@title", title);
            command.Parameters.AddWithValue("@deadline", DateTime.Parse(deadline).Date);
            command.Parameters.AddWithValue("@discription", description);
            command.Parameters.AddWithValue("@content", content);

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

        //Редактировать форму
        //Пример запроса: /api/managerform?form_id=3&id_mngr=1&date=2023-02-20&title=TestForm&deadline=2023-03-26&description=HelloWorld&content={"h2": "Hello World"}
        [HttpPut]
        public JsonResult Put(int form_id, int id_mngr, string date, string title, string deadline, string description, string content)
        {
            //UPDATE `project_bd`.`form` SET `name` = 'TestForm2', `description` = 'This is a PUT test' WHERE(`id_form` = '2');
            var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
            conn.Open();
            var command = new MySqlCommand(@"UPDATE `project_bd`.`form`
SET id_mngr = @manager_id,
name = @title,
date = @date,
deadline = @deadline,
description = @desc,
content = @content
WHERE id_form = @id;", conn);


            command.Parameters.AddWithValue("@manager_id", id_mngr);
            command.Parameters.AddWithValue("@date", DateTime.Parse(date));
            command.Parameters.AddWithValue("@title", title);
            command.Parameters.AddWithValue("@deadline", DateTime.Parse(deadline));
            command.Parameters.AddWithValue("@desc", description);
            command.Parameters.AddWithValue("@content", content);
            command.Parameters.AddWithValue("@id", form_id);

            try
            {
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch
            {
                return new JsonResult("Put Error");
            }

            return new JsonResult("Put Successful");
        }

        //Удаление формы
        //Пример запроса: /managerform/2
        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            var temp = new JsonResult("");
            if (GetDbForm(id, ref temp))
            {
                if (DeleteDbForm(id))
                {
                    return new JsonResult("Delete Form Success");
                }
                else
                {
                    return new JsonResult("Delete Form Error");
                }
            }
            else
            {
                return new JsonResult($"Form Not Found");
            }

        }

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

        private bool DeleteDbForm(int form_id)
        {
            var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
            conn.Open();
            var command = new MySqlCommand("DELETE FROM `project_bd`.`form` as form WHERE form.id_form = @ID", conn);
            command.Parameters.AddWithValue("@ID", form_id);

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
