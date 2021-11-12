using BLL.DTO.Notification;
using BLL.Interfaces;
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

        public TestNotificationController(INotificationService notificationService, IMapper mapper)
        {
            (_notificationService, _mapper) = (notificationService, mapper);
        }

        [HttpPost]
        public IActionResult Create(CreateNotificationDTO createNotificationDTO)
        {
            Notification notification = _mapper.Map<Notification>(createNotificationDTO);
            notification.DateTimeCreate = DateTime.UtcNow;//Mapster does not mapped this fild
            _notificationService.Create(notification);
            return Created("Default", notification);
        }

        [HttpGet]
        public IActionResult Get()
        {
            throw new NotImplementedException();
        }
    }
}
