using BLL.DTO.Notification;
using BLL.Interfaces;
using BLL.Services;
using DAL.Entity;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Training3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestNotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly NamedPipeClient _namedPipeClient;

        public TestNotificationController(INotificationService notificationService, IMapper mapper,
            NamedPipeClient namedPipeClient)
        {
            (_notificationService, _mapper, _namedPipeClient) = 
                (notificationService, mapper, namedPipeClient);
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
    }
}
