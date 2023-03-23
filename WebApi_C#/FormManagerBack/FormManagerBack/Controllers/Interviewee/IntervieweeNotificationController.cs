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
    //Контроллер, отвечающий за обработку уведомлений со стороны опрашиваемого 

    [Route("api/[controller]")]
    [ApiController]
    public class IntervieweeNotificationController : ControllerBase
    {

        private readonly IConfiguration configuration;

        public IntervieweeNotificationController(IConfiguration config)
        {
            this.configuration = config;
        }

        //Получить уведомления по ID опрашиваемого
        //Пример запроса: api/intervieweenotification/1
        [HttpGet("{user_id}")]
        public JsonResult Get(int user_id)
        {
            JsonResult response = new JsonResult("");

            if (GetDbNotification(user_id, ref response))
                return response;
            else
                return new JsonResult("Not Found");
        }

        //Изменить статус уведомления при прочтении
        //Пример запроса: /api/intervieweenotification/1?status_id=2
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

        //Создание нового уведомления, привязанного к определенной форме
        //Виды контента:
        // ID: 1  ---  Name: NewForm
        // ID: 2  ---  Name: DelayRequest
        // ID: 3  ---  Name: Deadline
        // ID: 4  ---  Name: DelayConfirmed
        //Статусы:
        // ID: 1  ---  Name: NotRead
        // ID: 2  ---  Name: Read
        // Пример запроса: /api/intervieweenotification?id_form=1&id_send=1&id_cont=3
        [HttpPost]
        public JsonResult Post(int id_form, int id_send, int id_cont)
        {
            int id_stat = 1;
            DateTime date = DateTime.Now;

            var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
            conn.Open();
            var command = new MySqlCommand(@$"INSERT INTO `project_bd`.`notification` ( `id_form`, `id_stat`, `id_send`, `id_cont`, `date`) 
VALUES ( @Form, @Status, @Sender, @Content, @Date);", conn);

            command.Parameters.AddWithValue("@Form", id_form);
            command.Parameters.AddWithValue("@Status", id_stat);
            command.Parameters.AddWithValue("@Sender", id_send);
            command.Parameters.AddWithValue("@Content", id_cont);
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


        //Функция для поиска уведомлений в БД, по ID опрашиваемого
        private bool GetDbNotification(int user_id, ref JsonResult result)
        {
            try
            {
                var conn = new MySqlConnection(configuration.GetConnectionString("MainDB"));
                conn.Open();
                var command = new MySqlCommand(@$"select content.Name, content.text, status.id_stat, status.name, body.id_ntf, body.id_form, body.id_send
FROM project_bd.notification as body join status on body.id_stat = status.id_stat join content on body.id_cont = content.id_cont 
WHERE body.id_send = @User_ID", conn);
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
