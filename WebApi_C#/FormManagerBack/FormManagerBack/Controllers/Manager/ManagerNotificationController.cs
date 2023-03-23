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
    //Контроллер для работы менеджера с уведомлениями

    [Route("api/[controller]")]
    [ApiController]
    public class ManagerNotificationController : ControllerBase
    {

        private readonly IConfiguration configuration;

        public ManagerNotificationController(IConfiguration config)
        {
            this.configuration = config;
        }

        //Получить список уведомлений для менеджера
        //Пример запроса: 
        [HttpGet]
        public JsonResult Get(int user_id)
        {
            JsonResult response = new JsonResult("");

            if (GetDbNotification(user_id, ref response))
                return response;
            else
                return new JsonResult("Not Found");
        }


        //Изменить статус уведомления при прочтении
        //Пример запроса: /api/managernotification/1?status_id=2
        [HttpPut("{notification_id}")]
        public JsonResult Put(int notification_id, int status_id)
        {

            var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
            conn.Open();
            var command = new MySqlCommand(@"UPDATE `project_bd`.`notification` as notif
SET notif.id_stat = @Stat
WHERE notif.id_ntf = @ID;", conn);

            command.Parameters.AddWithValue("@Stat", status_id);
            command.Parameters.AddWithValue("@ID", notification_id);


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


        //Сформировать список уведомлений, отправленных менеджеру
        private bool GetDbNotification(int user_id, ref JsonResult result)
        {
            try
            {
                var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
                conn.Open();
                var command = new MySqlCommand(@$"select content.Name, content.text, status.id_stat, status.name, body.id_ntf, body.id_form, body.id_send
FROM project_bd.notification as body join status on body.id_stat = status.id_stat join content on body.id_cont = content.id_cont join form on body.id_form = form.id_form
WHERE form.id_mngr = @User_ID", conn);
                command.Parameters.AddWithValue("@User_ID", user_id);

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
