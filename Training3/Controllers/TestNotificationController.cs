using BLL.DTO.Notification;
using BLL.Interfaces;
using BLL.Interfaces.NamedPipe;
using BLL.Services;
using DAL.Entity;
using DAL_NS.Entity;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Training3.NotificationServiceConfiguration;

namespace Training3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestNotificationController : ControllerBase
    {
        //private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly INamedPipeClientService _namedPipeClient;
        private readonly NotificationServiceProcess _notificationServiceProcess;

        public TestNotificationController(/*INotificationService notificationService,*/ IMapper mapper,
            INamedPipeClientService namedPipeClient, NotificationServiceProcess notificationServiceProcess)
        {
            (_mapper, _namedPipeClient) =
                (mapper, namedPipeClient);
            _notificationServiceProcess = notificationServiceProcess;
        }

        [HttpPost]
        public IActionResult Create(CreateNotificationDTO createNotificationDTO)
        {
            Notification notification = _mapper.Map<Notification>(createNotificationDTO);
            notification.DateTimeCreate = DateTime.UtcNow;//Mapster does not mapped this fild
            //_notificationService.Create(notification);
            if (_namedPipeClient.SendNotification(notification))
                return Created("Default", notification);
            else
                return this.StatusCode(500);
        }

        [HttpGet]
        public IActionResult Get()
        {
            throw new NotImplementedException();
        }

        [HttpGet("CheckProblemNotifications")]
        public IActionResult CheckProblemNotifications(int? pageLength = null, int? pageNumber = null)
        {
            return Ok(_namedPipeClient.CheckProblemNotification(pageLength, pageNumber));
        }

        [HttpGet("Re_sendProblemNotifications")]
        public IActionResult Re_sendProblemNotifications(int? take)
        {
            if (_namedPipeClient.Re_sendProblemNotifications(take ?? -1))
                return Ok();
            else
                return StatusCode(500);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Notification notification)
        {
            if(_namedPipeClient.UpdateProblemNotification(notification))
            {
                return Ok();
            }    
            else
            {
                return StatusCode(500);
            }
        }

        [HttpGet("StartNotificationService")]
        public IActionResult StartNotificationService()
        {
            return Ok(_notificationServiceProcess.Start());
        }

        [HttpGet("KillNotificationService")]
        public IActionResult KillNotificationService()
        {
            _notificationServiceProcess.KillProcess();
            return Ok();
        }
    }
}
