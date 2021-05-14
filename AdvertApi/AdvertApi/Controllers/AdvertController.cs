using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvertApi.Services;
using AdvertApi.Models;

namespace AdvertApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AdvertController : ControllerBase
    {
        private readonly IAdvertStorageService _advertStorageService;

        public AdvertController(IAdvertStorageService advertStorageService)
        {
            _advertStorageService = advertStorageService;
        }

        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(404)]
        [ProducesResponseType(201, Type = typeof(CreateAdvertResponse))]
        public async Task<IActionResult> Create(AdvertModel model)
        {
            string recordId;
            try
            {
                recordId = await _advertStorageService.AddAsync(model);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception.Message);
            }

            return StatusCode(201, new CreateAdvertResponse { Id = recordId });
        }

        [HttpPut]
        [Route("Confirm")]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Confirm(ConfirmAdvertModel model)
        {
            try
            {
                await _advertStorageService.ConfirmAsync(model);
                //await RaiseAdvertConfirmedMessage(model);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception exception)
            {
                return StatusCode(500, exception.Message);
            }

            return new OkResult();
        }

        //private async Task RaiseAdvertConfirmedMessage(ConfirmAdvertModel model)
        //{
        //    var topicArn = Configuration.GetValue<string>("TopicArn");
        //    var dbModel = await _advertStorageService.GetByIdAsync(model.Id);

        //    using (var client = new AmazonSimpleNotificationServiceClient())
        //    {
        //        var message = new AdvertConfirmedMessage
        //        {
        //            Id = model.Id,
        //            Title = dbModel.Title
        //        };

        //        var messageJson = JsonConvert.SerializeObject(message);
        //        await client.PublishAsync(topicArn, messageJson);
        //    }
        //}

    }
}
