using HonestAuto.Models;
using HonestAuto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HonestAuto.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ChatMessageService _messageService;
        private readonly UserManager<User> _userManager;

        public ChatController(ChatMessageService messageService, UserManager<User> userManager)
        {
            _messageService = messageService;
            _userManager = userManager;
        }

        public async Task<IActionResult> ChatWithUser(string receiverId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var messages = await _messageService.GetMessagesForConversationAsync(currentUserId, receiverId);

            var viewModel = new ChatViewModel
            {
                Messages = messages,
                ReceiverId = receiverId
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(ChatViewModel model)
        {
            if (ModelState.IsValid)
            {
                var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var message = new ChatMessage
                {
                    SenderId = senderId,
                    ReceiverId = model.ReceiverId,
                    DateSent = DateTime.UtcNow,
                    Content = model.Content
                };

                await _messageService.SaveMessageAsync(message);

                // You can return a success message or perform other actions here
            }

            // Redirect back to the chat view
            return RedirectToAction("ChatWithUser", new { receiverId = model.ReceiverId });
        }
    }
}