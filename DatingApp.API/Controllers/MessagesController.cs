using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{

    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;

        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            this._mapper = mapper;
            this._repo = repo;

        }

        // Incharge of getting a specific message
        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {

            // Verifies if user exist
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageFromRepo = await this._repo.GetMessage(id);

            //Verifies we have a message from the database
            if (messageFromRepo == null)
            {
                return NotFound();
            }

            return Ok(messageFromRepo);

        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery]MessageParams messageParams)
        {
            // Verifies if User exist
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            messageParams.UserId = userId;

            var messageFromRepo = await this._repo.GetMessagesForUser(messageParams);

            var messages = this._mapper.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo);

            // Incase we have multiple messagesn in Inbox / Outbox / Unread
            Response.AddPagination(messageFromRepo.CurrentPage, messageFromRepo.PageSize,
             messageFromRepo.TotalCount, messageFromRepo.TotalPages);

            return Ok(messages);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            // Verifies if User exist
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageFromRepo = await this._repo.GetMessageThread(userId, recipientId);

            var messageThread = this._mapper.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo);

            return Ok(messageThread);

        }


        // THIS TAKES CARE OF RECIEVING THE MESSAGES AND STORING IT IN THE DATABASE
        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
            var sender = await this._repo.GetUser(userId);

            // Verifies if User exist
            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            messageForCreationDto.SenderId = userId;

            var recipient = await this._repo.GetUser(messageForCreationDto.RecipientId);

            // Verifies if we have a recipient to reciece a message
            if (recipient == null)
            {
                return BadRequest("Could not find user");
            }
            // Grabs message with all user info
            var message = this._mapper.Map<Message>(messageForCreationDto);

            this._repo.Add(message);



            if (await this._repo.SaveAll())
            {
                // Returns only the message info we need
                var messageToReturn = this._mapper.Map<MessageToReturnDto>(message);
                return CreatedAtRoute("GetMessage", new { userId, id = message.Id }, messageToReturn);
            }
            throw new Exception("creating the message failed on save");
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId)
        {

            // Verifies if User exist
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageFromRepo = await this._repo.GetMessage(id);


            if (messageFromRepo.SenderId == userId)
            {
                messageFromRepo.SenderDeleted = true;
            }

            if (messageFromRepo.RecipientId == userId)
            {
                messageFromRepo.RecipientDeleted = true;
            }

            if (messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)
            {
                this._repo.Delete(messageFromRepo);
            }

            if (await this._repo.SaveAll())
            {
                return NoContent();
            }

            throw new Exception("Error deleting the message");

        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarMessageAsRead(int userId, int id)
        {
            // Verifies if User exist
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var message = await this._repo.GetMessage(id);

            if (message.RecipientId != userId)
            {
                return Unauthorized();
            }

            message.IsRead = true;
            message.DateRead = DateTime.Now;

            await this._repo.SaveAll();

            return NoContent();

        }

    }
}